using AddressBookManagement.ViewModels;
using System.Linq.Expressions;

namespace AddressBookManagement.Datas.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
        IQueryable<T> Query(bool includeDeleted = false);
        Task RestoreAsync(int id);
        Task<PageResult<T>> GetPagedAsync(
            int pageIndex,
            int pageSize,
            string? sortBy = null,
            string? sortDirection = "ASC",
            List<Expression<Func<T, bool>>>? filters = null);
    }
}
