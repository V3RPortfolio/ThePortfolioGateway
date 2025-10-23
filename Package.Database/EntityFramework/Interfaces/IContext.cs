using Microsoft.EntityFrameworkCore;

namespace Package.Database.EntityFramework.Interfaces
{
    public interface IContext
    {
        void OnModelCreating(ModelBuilder modelBuilder);
    }
}
