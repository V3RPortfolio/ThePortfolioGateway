
using AuthenticationService.Domain.Log;
using Microsoft.EntityFrameworkCore;
using Package.Database.EntityFramework.Interfaces;

namespace AuthenticationService.Database.EntityFramework.Context
{
    public class AuthContext : IContext
    {
        public void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LogDto>();
        }
    }
}
