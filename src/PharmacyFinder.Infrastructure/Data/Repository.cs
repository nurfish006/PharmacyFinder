using Microsoft.EntityFrameworkCore;
using PharmacyFinder.Core.Interfaces;
using System.Linq.Expressions;

namespace PharmacyFinder.Infrastructure.Data
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T> GetByIdAsync(int id) => await _dbSet.FindAsync(id);
        public virtual async Task<T> GetByIdAsync(string id) => await _dbSet.FindAsync(id);
        public virtual async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();
        
        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
            => await _dbSet.Where(predicate).ToListAsync();

        public virtual async Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate)
            => await _dbSet.SingleOrDefaultAsync(predicate);

        public virtual async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);
        public virtual async Task AddRangeAsync(IEnumerable<T> entities) => await _dbSet.AddRangeAsync(entities);
        
        public virtual void Update(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Remove(T entity) => _dbSet.Remove(entity);
        public virtual void RemoveRange(IEnumerable<T> entities) => _dbSet.RemoveRange(entities);
        
        public virtual async Task<int> CountAsync() => await _dbSet.CountAsync();
        
        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
            => await _dbSet.AnyAsync(predicate);
    }
}