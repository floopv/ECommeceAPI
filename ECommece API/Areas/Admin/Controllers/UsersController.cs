using ECommece_API.DTOs.Response;
using ECommerceAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECommece_API.Areas.Admin.Controllers
{
    [Route("[Area]/[controller]")]
    [ApiController]
    [Area("Admin")]
    [Authorize(Roles = ConstantData.Super_Admin_Role)]

    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userManager.Users.ToList();
            if (users == null)
            {
                return NotFound(new ReturnModelResponse
                {
                    ReturnCode = 404,
                    ReturnMessage = "No users found."
                });
            }
            return Ok(users);
        }
        [HttpPut("LockUnLock/{id}")]
        public async Task<IActionResult> LockUnLock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound(new ReturnModelResponse
                {
                    ReturnCode = 404,
                    ReturnMessage = "User not found."
                });
            }
            if (await _userManager.IsInRoleAsync(user, ConstantData.Super_Admin_Role))
            {
                return BadRequest(new ReturnModelResponse
                {
                    ReturnCode = 400,
                    ReturnMessage = "Cannot lock/unlock a Super Admin user."
                });
            }
            if (user.LockoutEnd != null && user.LockoutEnd > DateTime.UtcNow)
            {
                user.LockoutEnd = null;
                await _userManager.UpdateAsync(user);
                return Ok(new ReturnModelResponse
                {
                    ReturnCode = 200,
                    ReturnMessage = "User unlocked successfully."
                });
            }
            else
            {
                user.LockoutEnd = DateTime.UtcNow.AddMinutes(10);
                await _userManager.UpdateAsync(user);
                return Ok(new ReturnModelResponse
                {
                    ReturnCode = 200,
                    ReturnMessage = "User locked successfully for 10 minutes."
                });
            }
        }
    }
}
