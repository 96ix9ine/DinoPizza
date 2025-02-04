using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DinoPizza.Abstract
{
    public interface IRepository<T, TKey> 
        where T : MyEntity<TKey>
    {
        DbSet<T> FromDbSet();

        int GetCount();

        void Create(T entity);

        T Read(TKey id);

        T Read(TKey id, params Expression<Func<T, object>>[] includeProperties);

        void Update(T entity);

        void Delete(TKey id);

        IQueryable<T> GetAll();

        IQueryable<T> GetPage(int page, int pageSize);


        IQueryable<T> GetAdvanced(IQueryable<T> query, params Expression<Func<T, object>>[] includeProperties);

        IQueryable<T> GetAdvanced(IQueryable<T> query, Func<T, bool> predicate, params Expression<Func<T, object>>[] includeProperties);

        IQueryable<T> PageSplit(IQueryable<T> query, int page, int pageSize);
    }
}
