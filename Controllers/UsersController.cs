using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkOutAPI.Data;
using WorkOutAPI.DTO;
using WorkOutAPI.Models;
using WorkOutAPI.Repositories;

namespace WorkOutAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private IUserRepository userRepository;
        private IExerciseRepository exerciseRepository;
        private IUnityOfWork unityOfWork;
        public UsersController(IUserRepository userRepository, IExerciseRepository exerciseRepository, IUnityOfWork unityOfWork)
        {
            this.userRepository = userRepository;
            this.exerciseRepository = exerciseRepository;
            this.unityOfWork = unityOfWork;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetList([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            if(page < 0 || size < 0)
            {
                return BadRequest("Invalid Page or Size parameters");
            }

            if(size > 100)
            {
                return BadRequest("Size should be positive and less or equal than 100");
            }

            var list = await userRepository.GetList(page, size);
            var result = list.Select(i => new UserGetDTO(i.Id, i.Username, i.Email, i.Role, i.Schedule));
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await userRepository.GetById(id);
            
            if(user == null)
            {
                return NotFound("User not found");
            }
            
            var result = new UserGetDTO(user.Id, user.Username, user.Email, user.Role, user.Schedule);
            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] UserUpdateDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            bool isOwner = User.FindFirst(ClaimTypes.Email)?.Value == id.ToString();
            if(!User.IsInRole("Admin") && !isOwner)
            {
                return Forbid("You are not authorized");
            }
            
            var user = await userRepository.GetById(id);
            if(user == null)
            {
                return NotFound("User not found");
            }

            user.Username = model.Username ?? user.Username;

            if(model.Schedule != null)
            {
                var schedule = new List<Exercise>();
                foreach(var exerciseId in model.Schedule)
                {
                    var exercise = await exerciseRepository.GetById(exerciseId);

                    if(exercise == null)
                    {
                        return NotFound($"Exercise with Id {exerciseId} not found");
                    }

                    schedule.Add(exercise);
                }

                user.Schedule = schedule;
            }

            await userRepository.Update(user);
            await unityOfWork.Commit();

            return Ok($"User {user.Username} successfully updated");
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Remove(int id)
        {
            bool isOwner = User.FindFirst(ClaimTypes.Email)?.Value == id.ToString();
            if(!User.IsInRole("Admin") && !isOwner)
            {
                return Forbid("You are not authorized");
            }

            var user = await userRepository.GetById(id);
            
            if(user == null)
            {
                return NotFound("User not found");
            }

            await userRepository.Delete(id);
            await unityOfWork.Commit();

            return  Ok($"User {user.Username} successfully removed");
        }
        
    }
}