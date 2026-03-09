using Microsoft.EntityFrameworkCore;
using WorkOutAPI.Data;
using WorkOutAPI.Models;

namespace WorkOutAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext dbContext;
        public UserRepository(AppDbContext dbContext){
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<User>> GetList(int page = 1, int size = 10)
        {
            return await dbContext.Users.AsNoTracking().OrderBy(i => i.Id).Skip((page - 1) * size).Take(size).ToListAsync();
        }

        public async Task<User?> GetById(int id)
        {
            return await dbContext.Users.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<User?> GetByEmail(string email)
        {
            return await dbContext.Users.AsNoTracking().FirstOrDefaultAsync(i => i.Email == email);
        }

        public async Task Add(User user)
        {
            await dbContext.Users.AddAsync(user);
        }

        public async Task Update(User user)
        {
            dbContext.Users.Update(user);
        }

        public async Task Delete(int id)
        {
            var user = await dbContext.Users.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);

            if(user != null)
            {
                dbContext.Users.Remove(user);
            }

        }

    }
}