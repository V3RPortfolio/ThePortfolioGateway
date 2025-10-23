namespace Package.Database.EntityFramework.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        void Save();
        bool Commit();
        void DetachAll();
    }
}
