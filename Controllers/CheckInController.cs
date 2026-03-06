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

            var result = await checkInRepository.GetListByUserId(user.Id, page, size);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CheckInCreateDTO model)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            //To do: Get User by Claim
            var user = await userRepository.GetById(model.UserId);

            if(user == null)
            {
                return NotFound("User not found");
            }

            var checkIn = new CheckIn()
            {
                UserId = user.Id,
                User = user,
                Date = model.Date
            };

            await checkInRepository.Add(checkIn);
            await unityOfWork.Commit();
            return Created($"/checkin/{checkIn.Id}", $"CheckIn Successfully created");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(int id)
        {
            var checkIn = await checkInRepository.GetById(id);

            if(checkIn == null)
            {
                return NotFound("User not found");
            }

            await checkInRepository.Delete(id);
            await unityOfWork.Commit();
            return Ok($"CheckIn {id} sucessfully removed");
        }
    }
}