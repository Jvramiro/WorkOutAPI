namespace WorkOutAPI.Data
{
    public interface IUnitOfWork
    {
        Task Commit();
    }
}