using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using DinoPizza.Abstract;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DinoPizza.DataAccessLayer
{
    public class RepositoryGenericSql<TEntity, TKey>
        : IRepository<TEntity, TKey>
        where TEntity : MyEntity<TKey>
    {
        private readonly DinoDBContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public RepositoryGenericSql(DinoDBContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>(); // Products, Brands, Category
        }

        public void Create(TEntity entity)
        {
            _dbSet.Add(entity);
            _context.SaveChanges();
        }

        public void Delete(TKey id)
        {
            var entity = Read(id);
            if (entity != null) 
            {
                _dbSet.Remove(entity);
                _context.SaveChanges();
                return;
            }
            throw new InvalidOperationException($"Entity with key '{id}' not found");
        }

        public DbSet<TEntity> FromDbSet()
        {
            return _dbSet;
        }

        public IQueryable<TEntity> GetAll()
        {
            return _dbSet
                .AsNoTracking();
        }

        public int GetCount()
        {
            return _dbSet.Count();
        }

        public IQueryable<TEntity> GetPage(int page, int pageSize)
        {
            var query = _dbSet
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            return query;
        }

        public TEntity Read(TKey id)
        {
            return _dbSet.Find(id);
        }

        public void Update(TEntity model)
        {
            var key = model.GetId();
            var entity = Read(key);

            // все поля заменить
            _context.Entry(entity).CurrentValues.SetValues(model);
            _context.SaveChanges();
        }

        public IQueryable<TEntity> GetAdvanced(
            IQueryable<TEntity> query,
            params Expression<Func<TEntity, object>>[] includeProperties)
        {
            query = Include(query, includeProperties);
            return query;
        }

        public IQueryable<TEntity> GetAdvanced(
            IQueryable<TEntity> query,
            Func<TEntity, bool> predicate, 
            params Expression<Func<TEntity, object>>[] includeProperties)
        {
            query = IncludeWithPredicate(query, predicate, includeProperties);
            return query;
        }

        private IQueryable<TEntity> IncludeWithPredicate(
            IQueryable<TEntity> query,
            Func<TEntity, bool> predicate,
            params Expression<Func<TEntity, object>>[] includeProperties)
        {
            query = Include(query, includeProperties);
            return query.Where(predicate).AsQueryable();
        }

        private IQueryable<TEntity> Include(
            IQueryable<TEntity> query,
            params Expression<Func<TEntity, object>>[] includeProperties)
        {
            return includeProperties
                .Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
        }

        public IQueryable<TEntity> PageSplit(IQueryable<TEntity> query, int page, int pageSize)
        {
            var queryPage = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            return queryPage;
        }

        public TEntity Read(TKey id, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var query = _dbSet.AsNoTracking();
            var result = includeProperties
                .Aggregate(query, (current, includeProperty) => current.Include(includeProperty))
                .AsEnumerable()
                .FirstOrDefault(x => x.GetId().Equals(id));

            return result;
        }
    }
}
