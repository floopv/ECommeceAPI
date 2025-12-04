using ECommerceAPI.DataConnection;
using ECommerceAPI.Models;

namespace ECommerceAPI.Repos
{
    public class ProductRepository : Repository<Product> , IProductRepository
    {
        private readonly ECommerceDbContext _context;//= new ECommerceAPIDbContext();

        public ProductRepository(ECommerceDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task AddRangeAsync(IEnumerable<Product> products , CancellationToken cancellationToken = default)
        {
            await _context.Products.AddRangeAsync(products , cancellationToken);
        }
    }
}
