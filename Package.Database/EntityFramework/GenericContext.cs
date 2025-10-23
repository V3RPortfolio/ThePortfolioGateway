using Microsoft.EntityFrameworkCore;
using Package.Database.EntityFramework.Interfaces;

namespace Package.Database.EntityFramework
{
    public class GenericContext<TContext> : DbContext where TContext: IContext
    {
        readonly IContext _context;
        public GenericContext(DbContextOptions<GenericContext<TContext>> options, TContext context):base(options) {
            _context = context;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            _context.OnModelCreating(modelBuilder);
        }
    }
}
