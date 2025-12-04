using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ECommerceAPI.Repos
{
    public interface IRepository<T> where T : class
    {
       Task AddAsync(T entity, CancellationToken cancellationToken = default);
       void Update(T entity);
       void Delete(T entity);
       Task CommitAsync(CancellationToken cancellationToken = default);

       Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? expression = null,
                                        Expression<Func<T, object>>[]? includes = null,
                                        bool asNoTracking = true,
                                        CancellationToken cancellationToken = default
                                        );

      Task<T?> GetOneAsync(Expression<Func<T, bool>>? expression = null,
                                        Expression<Func<T, object>>[]? includes = null,
                                        bool asNoTracking = true,
                                        CancellationToken cancellationToken = default
                                        );
    }
}
