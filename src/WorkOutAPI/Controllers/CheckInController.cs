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
    public class CheckInController : ControllerBase
    {
        private IUserRepository userRepository;
        private ICheckInRepository checkInRepository;
        private IUnityOfWork unityOfWork;
        public CheckInController(IUserRepository  userRepository, ICheckInRepository checkInRepository, IUnityOfWork unityOfWork)
        {
            this.userRepository = userRepository;
            this.checkInRepository = checkInRepository;
            this.unityOfWork = unityOfWork;
        }

        [HttpGet("{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetListByUserId(int userId, [FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            if(page < 0 || size < 0)
            {
                return BadRequest("Invalid Page or Size parameters");
            }

            if(size > 100)
            {
                return BadRequest("Size should be positive and less or equal than 100");
            }

            var user = await userRepository.GetById(userId);

            if(user == null)
            {
                return BadRequest("User not found");
            }

            var list = await checkInRepository.GetListByUserId(user.Id, page, size);
            var result = list.Select(i => new CheckInGetDTO(i.Id, i.User?.Username ?? "", i.Date));
            return Ok(result);
        }

        [HttpGet("self")]
        [Authorize]
        public async Task<IActionResult> GetListBySelfId([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            if(page < 0 || size < 0)
            {
                return BadRequest("Invalid Page or Size parameters");
            }

            if(size > 100)
            {
                return BadRequest("Size should be positive and less or equal than 100");
            }

            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            var user = !string.IsNullOrEmpty(email) ? await userRepository.GetByEmail(email) : null;

            if(user == null)
            {
                return BadRequest("User not found");
            }

            var list = await checkInRepository.GetListByUserId(user.Id, page, size);
            var result = list.Select(i => new CheckInGetDTO(i.Id, i.User?.Username ?? "", i.Date));
            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CheckInCreateDTO model)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            if(!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
            {
                return BadRequest();
            }

            var checkIn = new CheckIn()
            {
                UserId = userId,
                Date = model.Date
            };

            await checkInRepository.Add(checkIn);
            await unityOfWork.Commit();
            return Created($"/checkin/{checkIn.Id}", $"CheckIn Successfully created");
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Remove(int id)
        {
            var checkIn = await checkInRepository.GetById(id);

            if(checkIn == null)
            {
                return NotFound("User not found");
            }

            bool isOwner = User.FindFirst(ClaimTypes.NameIdentifier)?.Value == checkIn.UserId.ToString();
            if(!User.IsInRole("Admin") && !isOwner)
            {
                return Forbid();
            }

            await checkInRepository.Delete(id);
            await unityOfWork.Commit();
            return Ok($"CheckIn {id} sucessfully removed");
        }
    }
}