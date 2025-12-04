using ECommerceAPI.DataConnection;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ECommerceAPI.Repos
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ECommerceDbContext _context;// = new ECommerceAPIDbContext();
        private readonly DbSet<T> _dbSet;
        public Repository(ECommerceDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task AddAsync(T entity , CancellationToken cancellationToken =default)
        {
           await _dbSet.AddAsync(entity , cancellationToken);       
        }
        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }
        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }
        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
           await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T , bool>>? expression = null , 
                                        Expression<Func<T , object>>[]? includes = null,
                                        bool asNoTracking = true ,
                                        CancellationToken cancellationToken = default
                                        )
        {
            var entites = _dbSet.AsQueryable();
            if (expression is not null)
            {
                entites = entites.Where(expression);
            }
            if (includes is not null)
            {
                foreach (var include in includes)
                {
                    entites = entites.Include(include);
                }
            }
            if (asNoTracking)
            {
                entites = entites.AsNoTracking();
            }
            return await entites.ToListAsync(cancellationToken);
        }

        public async Task<T?> GetOneAsync(Expression<Func<T, bool>>? expression = null,
                                        Expression<Func<T, object>>[]? includes = null ,
                                        bool asNoTracking = true,
                                        CancellationToken cancellationToken = default
                                        )
        {
            var entity = (await GetAllAsync(expression, includes, asNoTracking, cancellationToken)).FirstOrDefault();
            return entity;
        }
    }
}
