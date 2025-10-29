using AddressBookManagement.Commons.Enums;
using AddressBookManagement.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using AddressBookManagement.ViewModels;

namespace AddressBookManagement.Datas.Repositories.Implements
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;
        private readonly bool _isSoftDeletable;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
            //Check if the class extends from Common class
            _isSoftDeletable = typeof(Common).IsAssignableFrom(typeof(T));
        }
        
        //Generate query command that check for delete flag
        private IQueryable<T> GetQueryable(bool includeDeleted = false)
        {
            IQueryable<T> query = _dbSet;

            if (_isSoftDeletable && !includeDeleted)
            {
                var parameter = Expression.Parameter(typeof(T), "e");
                var deleteFlagProp = Expression.Call(
                    typeof(EF),
                    nameof(EF.Property),
                    new[] { typeof(DeleteStatus) },
                    parameter,
                    Expression.Constant(nameof(Common.DeleteFlag))
                );
                var deletedValue = Expression.Constant(DeleteStatus.Deleted);
                var condition = Expression.NotEqual(deleteFlagProp, deletedValue);
                var lambda = Expression.Lambda<Func<T, bool>>(condition, parameter);

                query = query.Where(lambda);
            }

            return query;
        }


        public async Task<List<T>> GetAllAsync()
        {
            return await GetQueryable().ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await GetQueryable().FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }

        public async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await GetQueryable().Where(predicate).ToListAsync();
        }

        public async Task<T> AddAsync(T entity)
        {
            _dbSet.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity is null) return;

            if (_isSoftDeletable)
            {
                var common = entity as Common;
                if (common != null)
                {
                    common.DeleteFlag = DeleteStatus.Deleted;
                    _dbSet.Update(entity);
                }
            }
            else
            {
                _dbSet.Remove(entity);
            }

            await _context.SaveChangesAsync();
        }

        public async Task RestoreAsync(int id)
        {
            if (!_isSoftDeletable) return;

            var entity = await _dbSet.FindAsync(id);
            if (entity is Common common && common.DeleteFlag == DeleteStatus.Deleted)
            {
                common.DeleteFlag = DeleteStatus.Active;
                _dbSet.Update(entity);
                await _context.SaveChangesAsync();
            }
        }

        public IQueryable<T> Query(bool includeDeleted = false)
        {
            return GetQueryable(includeDeleted);
        }

        public async Task<PageResult<T>> GetPagedAsync(int pageIndex, int pageSize, string? sortBy = null, string? sortDirection = "ASC", List<Expression<Func<T, bool>>>? filters = null)
        {
            IQueryable<T> query = GetQueryable();

            //Filter handling
            if (filters != null)
            {
                foreach (var filter in filters)
                {
                    query = query.Where(filter);
                }
            }

            var totalItems = await query.CountAsync();

            //Sort By handling
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                query = query.OrderBy($"{sortBy} {sortDirection}");
            }

            //Paging handling
            var items = await query
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PageResult<T>
            {
                Items = items,
                TotalItems = totalItems,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }
    }
}
