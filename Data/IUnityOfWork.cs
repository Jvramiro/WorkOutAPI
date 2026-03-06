namespace PetShopAPI.Data
{
    public interface IUnityOfWork
    {
        Task Commit();
    }
}