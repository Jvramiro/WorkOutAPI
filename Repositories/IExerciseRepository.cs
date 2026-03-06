using WorkOutAPI.Models;

namespace WorkOutAPI.Repositories
{
    public interface IExerciseRepository
    {
        Task<IEnumerable<Exercise>> GetList(int page = 1, int size = 30);
        Task<Exercise?> GetById(int id);
        Task Add(Exercise exercise);
        Task Update(Exercise exercise);
        Task Delete(int id);
    }
}