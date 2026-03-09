using Microsoft.EntityFrameworkCore;
using WorkOutAPI.Data;
using WorkOutAPI.Models;

namespace WorkOutAPI.Repositories
{
    public class CheckInRepository : ICheckInRepository
    {
        private readonly AppDbContext dbContext;
        public CheckInRepository(AppDbContext dbContext){
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<CheckIn>> GetListByUserId(int id, int page = 1, int size = 10)
        {
            return await dbContext.CheckIns.Include(i => i.User).OrderBy(i => i.Id).Where(i => i.UserId == id).Skip((page - 1) * size).Take(size).ToListAsync();
        }

        public async Task<CheckIn?> GetById(int id)
        {
            return await dbContext.CheckIns.Include(i => i.User).FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task Add(CheckIn checkIn)
        {
            await dbContext.CheckIns.AddAsync(checkIn);
        }

        public async Task Delete(int id)
        {
            var checkIn = await dbContext.CheckIns.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);

            if(checkIn != null)
            {
                dbContext.CheckIns.Remove(checkIn);
            }
        }

    }
}