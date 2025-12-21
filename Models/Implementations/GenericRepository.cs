// Models/Implementations/GenericRepository.cs (ĐÃ SỬA LỖI CIRCULAR REFERENCE)
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.Interfaces;
using System.Linq.Expressions;

namespace QUIZ_GAME_WEB.Models.Implementations
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly QuizGameContext _context;

        public GenericRepository(QuizGameContext context)
        {
            _context = context;
        }

        public IQueryable<T> GetQueryable()
        {
            return _context.Set<T>().AsQueryable();
        }

        public async Task AddAsync(T entity) => await _context.Set<T>().AddAsync(entity);

        public void Add(T entity) => _context.Set<T>().Add(entity);

        public void Update(T entity) => _context.Set<T>().Update(entity);

        public void Delete(T entity) => _context.Set<T>().Remove(entity);

       
        public virtual async Task<T?> GetByIdAsync(int id)
        {
            // FindAsync không support AsNoTracking, phải dùng FirstOrDefaultAsync
            var keyProperty = _context.Model.FindEntityType(typeof(T))?.FindPrimaryKey()?.Properties.FirstOrDefault();
            if (keyProperty == null)
                return await _context.Set<T>().FindAsync(id); // Fallback

            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, keyProperty.Name);
            var constant = Expression.Constant(id);
            var equality = Expression.Equal(property, constant);
            var lambda = Expression.Lambda<Func<T, bool>>(equality, parameter);

            return await _context.Set<T>()
                .AsNoTracking() // QUAN TRỌNG: Không track entity
                .FirstOrDefaultAsync(lambda);
        }

        public async Task<IEnumerable<T>> GetAllAsync() => await _context.Set<T>().ToListAsync();

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
            => await _context.Set<T>().Where(predicate).ToListAsync();

        public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        {
            return predicate == null
                ? await _context.Set<T>().CountAsync()
                : await _context.Set<T>().CountAsync(predicate);
        }
    }
}