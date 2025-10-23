using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Package.Database.EntityFramework.Interfaces
{
    public interface IRepository<TEntity, TContext> : IUnitOfWork, IDisposable 
        where TEntity : class
        where TContext: IContext
    {
        DbSet<TEntity> GetDbSet();
        GenericContext<TContext> GetContext();
        IQueryable<TEntity> GetQueryable(Expression<Func<TEntity, bool>> filter = null);

        IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "");

        TEntity GetByID(object id);

        void Insert(TEntity entity);

        void Delete(object id);

        void Delete(TEntity entityToDelete);

        void Update(TEntity entityToUpdate);
    }
}
