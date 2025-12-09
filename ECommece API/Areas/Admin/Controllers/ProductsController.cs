using ECommece_API.DTOs.Request;
using ECommece_API.DTOs.Response;
using ECommerceAPI.Repos;
using ECommerceAPI.Utilities;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ECommece_API.Areas.Admin.Controllers
{
    [Route("[Area]/[controller]")]
    [ApiController]
    [Area("Admin")]
    [Authorize(Roles = $"{ConstantData.Super_Admin_Role} , {ConstantData.Admin_Role} , {ConstantData.Employee_Role}")]

    public class ProductsController : ControllerBase
    {
        private readonly IRepository<Product> _productRepo;
        private readonly IRepository<ProductSubImage> _productSubImgRepo;
        private readonly IRepository<ProductColor> _productColorRepo;
        private readonly IRepository<Category> _categoryRepo;
        private readonly IRepository<Brand> _brandRepo;

        public ProductsController(IRepository<Product> productRepo, IRepository<ProductSubImage> productSubImgRepo, IRepository<ProductColor> productColorRepo, IRepository<Category> categoryRepo, IRepository<Brand> brandRepo)
        {
            _productRepo = productRepo;
            _productSubImgRepo = productSubImgRepo;
            _productColorRepo = productColorRepo;
            _categoryRepo = categoryRepo;
            _brandRepo = brandRepo;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            //var products = _context.Products.Include(p=>p.Category).Include(p=>p.Brand).AsQueryable();
            var products = await _productRepo.GetAllAsync(includes: [p => p.Category, p => p.Brand]);
            if (products is null)
            {
                return NotFound(new ReturnModelResponse
                {
                    ReturnCode = 404,
                    ReturnMessage = "No products found."
                });
            }
            var productsResponse = products.Select(p=> new ProductResponse
            {
                Id =  p.Id,
                Name = p.Name,
                Description = p.Description,
                MainImg = p.MainImg,
                Price = p.Price,
                CategoryName = p.Category.Name,
                BrandName = p.Brand.Name
            });
            return Ok(productsResponse.AsEnumerable());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductRequest createProductRequest) { 
            Product product = new Product
            {
                Name = createProductRequest.Name,
                Price = createProductRequest.Price,
                BrandId = createProductRequest.BrandId,
                CategoryId = createProductRequest.CategoryId ,
                Description = createProductRequest.Description
            };
            if (createProductRequest.Img is not null)
            {
                if (createProductRequest.Img.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + "-" + createProductRequest.Img.FileName;
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\ProductImages\\", fileName);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        createProductRequest.Img.CopyTo(stream);
                    }
                    product.MainImg = fileName;
                }
            }
            //_context.Products.Add(product);
            await _productRepo.AddAsync(product);
            //_context.SaveChanges();
            await _productRepo.CommitAsync();
            foreach (var subImg in createProductRequest.SubImgs)
            {
                if (subImg is not null)
                {
                    if (subImg.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + "-" + subImg.FileName;
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\ProductSubImgs", fileName);
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            subImg.CopyTo(stream);
                        }
                        ProductSubImage productSubImage = new ProductSubImage
                        {
                            Img = fileName,
                            ProductId = product.Id
                        };
                        //_context.ProductSubImages.Add(productSubImage);
                        await _productSubImgRepo.AddAsync(productSubImage);
                        await _productSubImgRepo.CommitAsync();
                    }
                }
            }
            if (createProductRequest.Colors is not null && createProductRequest.Colors.Count > 0)
            {
                createProductRequest.Colors = createProductRequest.Colors.Distinct().ToList();
                foreach (var color in createProductRequest.Colors)
                {
                    ProductColor productColor = new ProductColor
                    {
                        Color = color,
                        ProductId = product.Id
                    };
                    //_context.ProductColors.Add(productColor);
                    await _productColorRepo.AddAsync(productColor);
                    await _productColorRepo.CommitAsync();
                }
            }
            //_context.SaveChanges();
            return Ok(new ReturnModelResponse
            {
                ReturnCode = 200,
                ReturnMessage = "Product created successfully."
            });
        }
        [Authorize(Roles = $"{ConstantData.Super_Admin_Role} , {ConstantData.Admin_Role}")]

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(int id)
        {
            //var product = _context.Products.Include(p=>p.ProductColors).Include(p=>p.ProductSubImages).FirstOrDefault(b => b.Id == id);
            var product = await _productRepo.GetOneAsync(expression: b => b.Id == id, includes: [p => p.ProductColors, p => p.ProductSubImages]);
            if (product == null)
                return NotFound(new ReturnModelResponse
                {
                    ReturnCode = 404,
                    ReturnMessage = "Product not found."
                });
            var productDto = new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                MainImg = product.MainImg,
                Colors = product.ProductColors?.Select(c => c.Color).ToList() ?? new List<string>(),
                SubImages = product.ProductSubImages?.Select(s => s.Img).ToList() ?? new List<string>(),
                CategoryName = product.Category?.Name,
                BrandName = product.Brand?.Name
            };

            var categories = (await _categoryRepo.GetAllAsync())
                .Select(c => new CategoryResponse { Id = c.Id, Name = c.Name })
                .ToList();

            var brands = (await _brandRepo.GetAllAsync())
                .Select(b => new BrandResponse { Id = b.Id, Name = b.Name })
                .ToList();

            var response = new ProductWithOptionsResponse
            {
                Product = productDto,
                Categories = categories,
                Brands = brands
            };

            return Ok(response);
        }
        [Authorize(Roles = $"{ConstantData.Super_Admin_Role} , {ConstantData.Admin_Role}")]

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(int id ,UpdateProductRequest updateProductRequest)
        {
            //var productInDb = _context.Products.Include(p=>p.ProductColors).AsNoTracking().FirstOrDefault(b => b.Id == product.Id);
            var productInDb = await _productRepo.GetOneAsync(expression: b => b.Id == id,
                includes: [p => p.ProductColors],
                asNoTracking: true);
            if (productInDb == null)
            {
                return NotFound(new ReturnModelResponse
                {
                    ReturnCode = 404,
                    ReturnMessage = "Product not found."
                });
            }
            productInDb.Name = updateProductRequest.Name;
            productInDb.Price = updateProductRequest.Price;
            productInDb.Description = updateProductRequest.Description;
            if (updateProductRequest.MainImg is not null)
            {
                if (updateProductRequest.MainImg.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + "-" + updateProductRequest.MainImg.FileName;
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\ProductImages\\", fileName);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        updateProductRequest.MainImg.CopyTo(stream);
                    }
                    productInDb.MainImg = fileName;

                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\ProductImages\\", productInDb.MainImg);

                    if (System.IO.File.Exists(oldPath))

                        System.IO.File.Delete(oldPath);
                }
            }
            //else
            //{
            //    productInDb.MainImg = productInDb.MainImg;
            //}

            foreach (var subImg in updateProductRequest.SubImages)
            {
                if (subImg is not null)
                {
                    if (subImg.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + "-" + subImg.FileName;
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\ProductSubImgs", fileName);
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            subImg.CopyTo(stream);
                        }
                        ProductSubImage productSubImage = new ProductSubImage
                        {
                            Img = fileName,
                            ProductId = productInDb.Id
                        };
                        //_context.ProductSubImages.Add(productSubImage);
                        await _productSubImgRepo.AddAsync(productSubImage);
                        await _productSubImgRepo.CommitAsync();
                    }
                }
                //else
                //    product.ProductSubImages = productInDb.ProductSubImages;
            }

            if (updateProductRequest.Colors is not null)
            {
                updateProductRequest.Colors = updateProductRequest.Colors.Distinct().ToList();
                //var colorsInDb = _context.ProductColors.Where(pc=>pc.ProductId == product.Id).Select(pc => pc.Color).ToList();
                var productColors = await _productColorRepo.GetAllAsync(expression: pc => pc.ProductId == productInDb.Id);
                var colorsInDb = productColors.Select(pc => pc.Color).ToList();
                var colorsToRemove = colorsInDb.Except(updateProductRequest.Colors);
                foreach (var item in colorsToRemove)
                {
                    //_context.ProductColors.Remove(new ProductColor { Color = item, ProductId = product.Id });
                    _productColorRepo.Delete(new ProductColor { Color = item, ProductId = productInDb.Id });
                    await _productColorRepo.CommitAsync();
                }
                var colorsToAdd = updateProductRequest.Colors.Except(colorsInDb);
                foreach (var item in colorsToAdd)
                {
                    //_context.ProductColors.Add(new ProductColor { Color = item, ProductId = product.Id });
                    await _productColorRepo.AddAsync(new ProductColor { Color = item, ProductId = productInDb.Id });
                    await _productColorRepo.CommitAsync();
                }
            }

            //_context.Products.Update(product);
            _productRepo.Update(productInDb);
            //_context.SaveChanges();
            await _productRepo.CommitAsync();
            return Ok(new ReturnModelResponse
            {
                ReturnCode = 200,
                ReturnMessage = "Product updated successfully."
            });
        }
        [Authorize(Roles = $"{ConstantData.Super_Admin_Role} , {ConstantData.Admin_Role}")]

        [HttpDelete("DeleteSubImage/{productId}/{img}")]
        public async Task<IActionResult> DeleteSubImage(int? productId, string? img)
        {
            Console.WriteLine($"Looking for ProductId={productId}, Img={img}");

            if (productId is not null && img is not null)
            {
                //var productSubImg = _context.ProductSubImages.FirstOrDefault(psi=>psi.ProductId == productId && psi.Img == img);
                var productSubImg = await _productSubImgRepo.GetOneAsync(expression: psi => psi.ProductId == productId && psi.Img == img);
                if (productSubImg is null)
                {
                    return NotFound(new ReturnModelResponse
                    {
                        ReturnCode = 404,
                        ReturnMessage = "Sub image not found."
                    });
                }
                //_context.ProductSubImages.Remove(productSubImg);
                _productSubImgRepo.Delete(productSubImg);
                //_context.SaveChanges();
                await _productSubImgRepo.CommitAsync();
            }
            else
                return BadRequest(new ReturnModelResponse
                {
                    ReturnCode = 400,
                    ReturnMessage = "Invalid product ID or image name."
                });

            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\ProductSubImgs", img);
            if (System.IO.File.Exists(oldPath))
            {
                System.IO.File.Delete(oldPath);
            }
            //_context.SaveChanges();
            return Ok(new ReturnModelResponse
            {
                ReturnCode = 200,
                ReturnMessage = "Sub image deleted successfully."
            });
        }
        [Authorize(Roles = $"{ConstantData.Super_Admin_Role} , {ConstantData.Admin_Role}")]

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            //var product = _context.Products.Include(p=>p.ProductSubImages).FirstOrDefault(b => b.Id == id);
            var product = await _productRepo.GetOneAsync(expression: b => b.Id == id,
                includes: [p => p.ProductSubImages]);
            if (product == null)
                return NotFound(new ReturnModelResponse
                {
                    ReturnCode = 404,
                    ReturnMessage = "Product not found."
                });
            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\ProductImages\\", product.MainImg);

            if (System.IO.File.Exists(oldPath))
            {
                System.IO.File.Delete(oldPath);
            }
            if (product.ProductSubImages.Count > 0)
            {
                foreach (var subImg in product.ProductSubImages)
                {
                    var subImgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images\\ProductSubImgs", subImg.Img);
                    if (System.IO.File.Exists(subImgPath))
                    {
                        System.IO.File.Delete(subImgPath);
                    }
                }
            }
            //_context.Products.Remove(product);
            _productRepo.Delete(product);
            //_context.SaveChanges();
            await _productRepo.CommitAsync();
            return NoContent();
        }
    }
}
