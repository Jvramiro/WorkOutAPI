namespace WorkOutAPI.Data
{
    public class UnityOfWork : IUnityOfWork
    {
        private readonly AppDbContext dbContext;
        public UnityOfWork(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task Commit()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}