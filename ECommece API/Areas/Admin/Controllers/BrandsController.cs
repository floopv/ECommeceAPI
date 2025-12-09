using ECommece_API.DTOs.Request;
using ECommece_API.DTOs.Response;
using ECommerceAPI.Repos;
using ECommerceAPI.Utilities;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommece_API.Areas.Admin.Controllers
{
    [Route("[Area]/[controller]")]
    [ApiController]
    [Area("Admin")]
    [Authorize(Roles = $"{ConstantData.Super_Admin_Role} ,{ConstantData.Admin_Role} , {ConstantData.Employee_Role}")]

    public class BrandsController : ControllerBase
    {
        private readonly IRepository<Brand> _brandRepository;

        public BrandsController(IRepository<Brand> brandRepository)
        {
            _brandRepository = brandRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            //var brands = _context.Brands.AsQueryable();
            var brands = await _brandRepository.GetAllAsync();
            if (brands is null)
            {
                return NotFound(new ReturnModelResponse
                {
                    ReturnCode = 404,
                    ReturnMessage = "No brands found."
                });
            }
            return Ok(brands);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(int id)
        {
            //var brand = _context.Brands.FirstOrDefault(b => b.Id == id);
            var brand = await _brandRepository.GetOneAsync(b => b.Id == id);
            if (brand == null)
                return NotFound(new ReturnModelResponse
                {
                    ReturnCode = 404,
                    ReturnMessage = "Brand not found."
                });
            return Ok(brand);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateBrandRequest createBrandRequest)
        {
            //var brand = new Brand
            //{
            //    Name = createBrandVM.Name,
            //    Description = createBrandVM.Description,
            //    Status = createBrandVM.Status
            //};
            var brand = createBrandRequest.Adapt<Brand>();
            if (createBrandRequest.FormImg is not null)
            {
                if (createBrandRequest.FormImg.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + "-" + createBrandRequest.FormImg.FileName;
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\BrandImages\\", fileName);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        createBrandRequest.FormImg.CopyTo(stream);
                    }
                    brand.Img = fileName;
                }
            }
            //_context.Brands.Add(brand);
            await _brandRepository.AddAsync(brand);
            //_context.SaveChanges();
            await _brandRepository.CommitAsync();
            return CreatedAtAction(nameof(GetOne), new { id = brand.Id }, brand);
        }
       [Authorize(Roles = $"{ConstantData.Super_Admin_Role} , {ConstantData.Admin_Role}")]

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateBrandRequest UpdateBrandVM)
        {
            var brandInDB = await _brandRepository.GetOneAsync(c => c.Id == id, asNoTracking: true);

            if (brandInDB is null)
            {
                return NotFound(new ReturnModelResponse
                {
                    ReturnCode = 404,
                    ReturnMessage = "Brand not found."
                });
            }
            var brand = UpdateBrandVM.Adapt<Brand>();
            brand.Id = id;
            if (UpdateBrandVM.FormImg is not null)
            {
                if (UpdateBrandVM.FormImg.Length > 0)
                {
                    //var fileName = Guid.NewGuid().ToString() + Path.GetExtension(img.FileName);
                    var fileName = Guid.NewGuid().ToString() + "-" + UpdateBrandVM.FormImg.FileName;
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\BrandImages\\", fileName);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        UpdateBrandVM.FormImg.CopyTo(stream);
                    }
                    brand.Img = fileName;
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\BrandImages\\", brandInDB.Img);

                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }
            }
            else
            {
                brand.Img = brandInDB.Img;
            }
            //_context.Brands.Update(brand);
            //_context.SaveChanges();
            _brandRepository.Update(brand);
            await _brandRepository.CommitAsync();
            return Ok(new ReturnModelResponse
            {
                ReturnCode = 200,
                ReturnMessage = "Brand updated successfully."
            });
        }
        [Authorize(Roles = $"{ConstantData.Super_Admin_Role} , {ConstantData.Admin_Role}")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            //var brand = _context.Brands.FirstOrDefault(b => b.Id == id);
            var brand = await _brandRepository.GetOneAsync(b => b.Id == id);
            if (brand == null)
                return NotFound(new ReturnModelResponse
                {
                    ReturnCode = 404,
                    ReturnMessage = "Brand not found."
                });
            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\BrandImages\\", brand.Img);

            if (System.IO.File.Exists(oldPath))
            {
                System.IO.File.Delete(oldPath);
            }
            //_context.Brands.Remove(brand);
            _brandRepository.Delete(brand);
            //_context.SaveChanges();
            await _brandRepository.CommitAsync();
            return NoContent();
        }
    }
}
