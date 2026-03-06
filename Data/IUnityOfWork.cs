namespace WorkOutAPI.Data
{
    public interface IUnityOfWork
    {
        Task Commit();
    }
}