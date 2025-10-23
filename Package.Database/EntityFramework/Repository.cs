using Microsoft.EntityFrameworkCore;
using Package.Database.EntityFramework.Interfaces;
using System.Linq.Expressions;

namespace Package.Database.EntityFramework
{
    public class Repository<TEntity, TContext> : IRepository<TEntity, TContext> 
        where TEntity : class
        where TContext : IContext
    {
        internal GenericContext<TContext> _context;
        internal DbSet<TEntity> _dbSet;

        public Repository(GenericContext<TContext> context)
        {
            this._context = context;
            this._dbSet = context.Set<TEntity>();
        }

        public virtual GenericContext<TContext> GetContext()
        {
            return _context;
        }

        public virtual DbSet<TEntity> GetDbSet()
        {
            return _dbSet;
        }

        public virtual IQueryable<TEntity> GetQueryable(Expression<Func<TEntity, bool>> filter = null)
        {
            if(filter != null) return _dbSet.Where(filter);
            return _dbSet;
        }

        public virtual IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        public virtual TEntity GetByID(object id)
        {
            return _dbSet.Find(id);
        }

        public virtual void Insert(TEntity entity)
        {
            _dbSet.Add(entity);
        }

        public virtual void Delete(object id)
        {
            TEntity entityToDelete = _dbSet.Find(id);
            Delete(entityToDelete);
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (_context.Entry(entityToDelete).State == EntityState.Detached)
            {
                _dbSet.Attach(entityToDelete);
            }
            _dbSet.Remove(entityToDelete);
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            _dbSet.Attach(entityToUpdate);
            _context.Entry(entityToUpdate).State = EntityState.Modified;
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public bool Commit()
        {
            var success = false;
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.SaveChanges();
                    transaction.Commit();
                    DetachAll();
                    success = true;
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    Console.WriteLine(ex.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine(ex.Message);
                }
            }

            return success;

        }

        public void DetachAll()
        {
            foreach (var dbEntityEntry in _context.ChangeTracker.Entries().ToArray())
            {
                if (dbEntityEntry.Entity != null)
                {
                    dbEntityEntry.State = EntityState.Detached;
                }
            }
        }
    }
}
