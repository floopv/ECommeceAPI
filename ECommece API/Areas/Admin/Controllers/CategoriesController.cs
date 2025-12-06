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
    [Authorize(Roles = $"{ConstantData.Super_Admin_Role} , {ConstantData.Admin_Role} , {ConstantData.Employee_Role}")]

    public class CategoriesController : ControllerBase
    {
        private readonly IRepository<Category> _categoryRepository;//= new Repository<Category>();
        public CategoriesController(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            //var categories = _context.Categories.AsQueryable();
            var categories = await _categoryRepository.GetAllAsync(cancellationToken: cancellationToken);
            if (categories is null)
            {
                return NotFound(new ReturnModelResponse
                {
                    ReturnCode = 404 ,
                    ReturnMessage = "No Categories Found"
                });
            }
            return Ok(categories);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category, CancellationToken cancellationToken)
        {
            //_context.Categories.Add(category);
            await _categoryRepository.AddAsync(category, cancellationToken);
            //_context.SaveChanges();
            await _categoryRepository.CommitAsync(cancellationToken);
            return CreatedAtAction(nameof(GetOne), new { id = category.Id }, category);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(int id, CancellationToken cancellationToken)
        {
            //var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            var category = await _categoryRepository.GetOneAsync(c => c.Id == id, cancellationToken: cancellationToken);
            if (category == null)
                return NotFound(new ReturnModelResponse
                {
                    ReturnCode = 404 ,
                    ReturnMessage = "Category Not Found"
                });

            return Ok(category);
        }
        [Authorize(Roles = $"{ConstantData.Super_Admin_Role} , {ConstantData.Admin_Role}")]

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id ,Category category, CancellationToken cancellationToken)
        {
            var existingCategory = await _categoryRepository.GetOneAsync(c => c.Id == id ,asNoTracking : true);
            if (existingCategory == null)
                return NotFound(new ReturnModelResponse
                {
                    ReturnCode = 404 ,
                    ReturnMessage = "Category Not Found"
                });
            //_context.Categories.Update(category);
            category.Id = id;
            _categoryRepository.Update(category);
            //_context.SaveChanges();
            await _categoryRepository.CommitAsync(cancellationToken: cancellationToken);
            return Ok(new ReturnModelResponse
            {
                ReturnCode = 200 ,
                ReturnMessage = "Category Updated Successfully"
            });
        }
        [Authorize(Roles = $"{ConstantData.Super_Admin_Role} , {ConstantData.Admin_Role}")]


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            //var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            var category = await _categoryRepository.GetOneAsync(c => c.Id == id, cancellationToken: cancellationToken);
            if (category == null)
                return NotFound(new ReturnModelResponse
                {
                    ReturnCode = 404 ,
                    ReturnMessage = "Category Not Found"
                });
            //_context.Categories.Remove(category);
            _categoryRepository.Delete(category);
            //_context.SaveChanges();
            await _categoryRepository.CommitAsync(cancellationToken: cancellationToken);
            return NoContent();
        }
    }
}
