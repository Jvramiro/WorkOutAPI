namespace WorkOutAPI.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext dbContext;
        public UnitOfWork(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task Commit()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}