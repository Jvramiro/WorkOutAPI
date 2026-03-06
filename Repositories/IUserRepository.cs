using WorkOutAPI.Models;

namespace WorkOutAPI.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetList(int page = 1, int size = 10);
        Task<User?> GetById(int id);
        Task Add(User user);
        Task Update(User user);
        Task Delete(int id);
    }
}