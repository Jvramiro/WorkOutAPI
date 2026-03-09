using WorkOutAPI.Models;

namespace WorkOutAPI.Repositories
{
    public interface ICheckInRepository
    {
        Task<IEnumerable<CheckIn>> GetListByUserId(int id, int page = 1, int size = 10);
        Task<CheckIn?> GetById(int id);
        Task Add(CheckIn checkIn);
        Task Delete(int id);
    }
}