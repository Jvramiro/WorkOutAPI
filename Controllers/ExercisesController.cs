using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkOutAPI.Data;
using WorkOutAPI.DTO;
using WorkOutAPI.Repositories;

namespace WorkOutAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExercisesController : ControllerBase
    {
        private IExerciseRepository exerciseRepository;
        private IUnityOfWork unityOfWork;
        public ExercisesController(IExerciseRepository exerciseRepository, IUnityOfWork unityOfWork)
        {
            this.exerciseRepository = exerciseRepository;
            this.unityOfWork = unityOfWork;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetList([FromQuery] int page = 1, [FromQuery] int size = 30)
        {
            if(page < 0 || size < 0)
            {
                return BadRequest("Invalid Page or Size parameters");
            }

            if(size > 100)
            {
                return BadRequest("Size should be positive and less or equal than 100");
            }

            var result = await exerciseRepository.GetList(page, size);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var exercise = await exerciseRepository.GetById(id);

            if(exercise == null)
            {
                return NotFound("Exercise not found");
            }

            return Ok(exercise);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] ExerciseUpdateDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var exercise = await exerciseRepository.GetById(id);

            if(exercise == null)
            {
                return NotFound("Exercise not found");
            }

            exercise.Name = model.Name ?? exercise.Name;
            exercise.Group = model.Group ?? exercise.Group;

            await exerciseRepository.Update(exercise);
            await unityOfWork.Commit();
            
            return Ok($"Exercise {exercise.Name} successfully updated");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Remove(int id)
        {
            var exercise = await exerciseRepository.GetById(id);

            if(exercise == null)
            {
                return NotFound("Exercise not found");
            }

            await exerciseRepository.Delete(id);
            await unityOfWork.Commit();
            
            return Ok($"Exercise {exercise.Name} successfully removed");
        }
    }
}