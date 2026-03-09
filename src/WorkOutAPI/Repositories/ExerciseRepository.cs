using Microsoft.EntityFrameworkCore;
using WorkOutAPI.Data;
using WorkOutAPI.Models;

namespace WorkOutAPI.Repositories
{
    public class ExerciseRepository : IExerciseRepository
    {
        private readonly AppDbContext dbContext;
        public ExerciseRepository(AppDbContext dbContext){
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<Exercise>> GetList(int page = 1, int size = 30)
        {
            return await dbContext.Exercises.AsNoTracking().OrderBy(i => i.Id).Skip((page - 1) * size).Take(size).ToListAsync();
        }

        public async Task<Exercise?> GetById(int id)
        {
            return await dbContext.Exercises.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task Add(Exercise exercise)
        {
            await dbContext.Exercises.AddAsync(exercise);
        }

        public async Task Update(Exercise exercise)
        {
            dbContext.Exercises.Update(exercise);
        }

        public async Task Delete(int id)
        {
            var exercise = await dbContext.Exercises.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);

            if(exercise != null)
            {
                dbContext.Exercises.Remove(exercise);
            }
        }

    }
}