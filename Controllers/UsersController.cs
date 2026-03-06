using Microsoft.AspNetCore.Mvc;
using WorkOutAPI.Data;
using WorkOutAPI.DTO;
using WorkOutAPI.Repositories;

namespace WorkOutAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private IUserRepository userRepository;
        private IUnityOfWork unityOfWork;
        public UsersController(IUserRepository  userRepository, IUnityOfWork unityOfWork)
        {
            this.userRepository = userRepository;
            this.unityOfWork = unityOfWork;
        }

        [HttpGet]
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
        public async Task<IActionResult> Update(int id, UserUpdateDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            
            var user = await userRepository.GetById(id);
            
            if(user == null)
            {
                return NotFound("User not found");
            }

            await userRepository.Update(user);
            await unityOfWork.Commit();

            return Ok($"User {user.Username} successfully updated");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(int id)
        {
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