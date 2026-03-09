using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
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
        private TokenService tokenService;
        public LoginController(IUserRepository userRepository, IUnityOfWork unityOfWork, TokenService tokenService)
        {
            this.userRepository = userRepository;
            this.unityOfWork = unityOfWork;
            this.tokenService = tokenService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO model)
        {
            var user = await userRepository.GetByEmail(model.Email);

            if(user == null)
            {
                return NotFound("Invalid e-mail or password");
            }

            bool isValid = PasswordHashService.VerifyPassword(model.Password, user.PasswordHash);
            if (!isValid)
            {
                return NotFound("Invalid e-mail or password");
            }

            var token = tokenService.GenerateToken(user);
            var refreshToken = tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            await userRepository.Update(user);
            await unityOfWork.Commit();

            var result = new TokenDTO(token, refreshToken);
            return Ok(result);
        }

        [HttpPost("refresh")]
        [Authorize]
        public async Task<IActionResult> Refresh([FromBody] string refreshToken)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            var user = !string.IsNullOrEmpty(email) ? await userRepository.GetByEmail(email) : null;

            if(user == null || user.RefreshToken != refreshToken)
            {
                return Unauthorized("Invalid refreshToken");
            }

            var newToken = tokenService.GenerateToken(user);
            var newRefreshToken = tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            await userRepository.Update(user);
            await unityOfWork.Commit();

            var result = new TokenDTO(newToken, newRefreshToken);
            return Ok(result);
        }
    }
}