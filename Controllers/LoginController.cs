using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WorkOutAPI.Data;
using WorkOutAPI.DTO;
using WorkOutAPI.Repositories;
using WorkOutAPI.Services;

namespace WorkOutAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private IUserRepository userRepository;
        private IUnityOfWork unityOfWork;
        public LoginController(IUserRepository  userRepository, IUnityOfWork unityOfWork)
        {
            this.userRepository = userRepository;
            this.unityOfWork = unityOfWork;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO model)
        {
            var user = await userRepository.GetByEmail(model.Email);

            if(user == null)
            {
                return NotFound("Invalid user or password");
            }

            bool isValid = PasswordHashService.VerifyPassword(model.Password, user.PasswordHash);
            if (!isValid)
            {
                return NotFound("Invalid user or password");
            }

            var token = TokenService.GenerateToken(user);
            var refreshToken = TokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            await userRepository.Update(user);
            await unityOfWork.Commit();

            var result = new TokenDTO(token, refreshToken);
            return Ok(result);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] string refreshToken)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            var user = !string.IsNullOrEmpty(email) ? await userRepository.GetByEmail(email) : null;

            if(user == null || user.RefreshToken != refreshToken)
            {
                return Unauthorized("Invalid refreshToken");
            }

            var newToken = TokenService.GenerateToken(user);
            var newRefreshToken = TokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            await userRepository.Update(user);
            await unityOfWork.Commit();

            var result = new TokenDTO(newToken, newRefreshToken);
            return Ok(result);
        }
    }
}