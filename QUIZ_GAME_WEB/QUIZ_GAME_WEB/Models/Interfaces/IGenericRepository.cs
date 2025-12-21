// Models/Interfaces/IGenericRepository.cs
using System.Linq.Expressions;

namespace QUIZ_GAME_WEB.Models.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
        IQueryable<T> GetQueryable();

        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}