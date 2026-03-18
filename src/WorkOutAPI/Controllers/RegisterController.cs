using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkOutAPI.Data;
using WorkOutAPI.DTO;
using WorkOutAPI.Models;
using WorkOutAPI.Repositories;
using WorkOutAPI.Services;

namespace WorkOutAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegisterController : ControllerBase
    {
        private IUserRepository userRepository;
        private IUnitOfWork UnitOfWork;
        public RegisterController(IUserRepository  userRepository, IUnitOfWork UnitOfWork)
        {
            this.userRepository = userRepository;
            this.UnitOfWork = UnitOfWork;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] UserRegisterDTO model)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            bool hasEmailConflict = await userRepository.GetByEmail(model.Email) != null;
            if (hasEmailConflict)
            {
                return Conflict("Email already registered");
            }

            string passwordHash = PasswordHashService.HashPassword(model.Password);
            string refreshToken = Guid.NewGuid().ToString();

            var user = new User()
            {
                Username = model.Username,
                Email = model.Email,
                PasswordHash = passwordHash,
                Role = Enums.Role.Default,
                RefreshToken = refreshToken
            };

            await userRepository.Add(user);
            await UnitOfWork.Commit();
            return Created($"/users/{user.Id}", $"User Successfully created");
        }

    }
}