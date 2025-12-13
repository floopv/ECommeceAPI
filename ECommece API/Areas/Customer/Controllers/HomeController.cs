using ECommece_API.DTOs.Request;
using ECommece_API.DTOs.Response;
using ECommerceAPI.Repos;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ECommece_API.Areas.Customer.Controllers
{
    [Route("[Area]/[controller]")]
    [ApiController]
    [Area("Customer")]
    public class HomeController : ControllerBase
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductSubImage> _productSubImageRepository;
        private readonly IRepository<ProductColor> _productColorRepository;


        public HomeController(IRepository<Product> productRepository, IRepository<ProductSubImage> productSubImageRepository, IRepository<ProductColor> productColorRepository)
        {
            _productRepository = productRepository;
            _productSubImageRepository = productSubImageRepository;
            _productColorRepository = productColorRepository;
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(filterProductRequest filter, int page = 1)
        {
            //var products = _context.Products.Include(p => p.Category).AsQueryable();
            var products = await _productRepository.GetAllAsync(includes: [p => p.Category , p=>p.ProductColors , p=>p.ProductSubImages]);
            if (filter.Name is not null)
            {
                products = products.Where(p => p.Name.Contains(filter.Name));
            }
            if (filter.MinPrice is not null)
            {
                products = products.Where(p => p.Price - p.Price * (p.Discount / 100) >= filter.MinPrice);
            }
            if (filter.MaxPrice is not null)
            {
                products = products.Where(p => p.Price - p.Price * (p.Discount / 100) <= filter.MaxPrice);
            }
            if (filter.CategoryId is not null)
            {
                products = products.Where(p => p.CategoryId == filter.CategoryId);
            }
            if (filter.BrandId is not null)
            {
                products = products.Where(p => p.BrandId == filter.BrandId);
            }
            if (filter.IsHot)
            {
                products = products.Where(p => p.Discount >= 50);
            }
            products = products.Skip((page - 1) * 8).Take(8);

            var productsResponse = products.Adapt<List<ProductResponse>>();

            return Ok(productsResponse);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(int id)
        {
            var product = await _productRepository.GetOneAsync(p => p.Id == id);
            if (product == null)
            {
                return NotFound(new ReturnModelResponse
                {
                    ReturnCode = 404 ,
                    ReturnMessage = "Product Not Found"
                });
            }
            var relatedProducts = await _productRepository.GetAllAsync(p => p.CategoryId == product.CategoryId && p.Id != product.Id);
            var viewModel = new ProductWithRelatedProductsResponse
            {
                Product = product,
                RelatedProducts = relatedProducts.ToList()
            };
            return Ok(viewModel);
        }
    }
}
