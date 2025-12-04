using ECommece_API.DTOs.Request;
using ECommece_API.DTOs.Response;
using ECommerceAPI.Models;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ECommece_API.Areas.Identity.Controllers
{
    [Route("[Area]/[controller]")]
    [ApiController]
    [Area("Identity")]
    public class ProfileController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> GetInfo()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            //var userVM = new ApplicationUserVM
            //{
            //    FullName = $"{user.FirstName} {user.LastName}",
            //    Address = user.Address,
            //    PhoneNumber = user.PhoneNumber,
            //    Email = user.Email
            //};
            //TypeAdapterConfig<ApplicationUser, ApplicationUserVM>.NewConfig()
            //    .Map(dest => dest.FullName, src => $"{src.FirstName} {src.LastName}");
            var userVM = user.Adapt<ApplicationUserResponse>();
            return Ok(userVM);
        }
        [HttpPost("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile(ApplicationUserRequest applicationUserRequest)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound (new ReturnModelResponse
                {
                    ReturnCode = 404 ,
                    ReturnMessage = "User not found."
                });
            }
            var names = applicationUserRequest.FullName?.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (names != null && names.Length >= 2)
            {
                user.FirstName = names[0];
                user.LastName = names[1];
            }
            else
            {
                user.FirstName = string.Empty;
                user.LastName = string.Empty;
            }
            user.Address = applicationUserRequest.Address;
            user.PhoneNumber = applicationUserRequest.PhoneNumber;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                string errors = string.Empty;
                foreach(var error in result.Errors)
                {
                    errors += $"{error.Description} \n";
                }
                return BadRequest (new ReturnModelResponse
                {
                    ReturnCode = 400 ,
                    ReturnMessage = errors
                });
            }
            else
            {
                return Ok(new ReturnModelResponse
                {
                    ReturnCode = 200 ,
                    ReturnMessage = "Profile Updated Successfully"
                });
            }
        }
        [HttpPost("UpdatePassword")]
        public async Task<IActionResult> UpdatePassword(ApplicationUserRequest applicationUserRequest)
        {
            if (string.IsNullOrEmpty(applicationUserRequest.CurrentPassword) || string.IsNullOrEmpty(applicationUserRequest.NewPassword))
            {
                return BadRequest(new ReturnModelResponse
                {
                    ReturnCode = 400,
                    ReturnMessage = "Current password and new password are required."
                });
            }
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound( new ReturnModelResponse
                {
                    ReturnCode = 404 ,
                    ReturnMessage = "User not found."
                });
            }
            var result = await _userManager.ChangePasswordAsync(user, applicationUserRequest.CurrentPassword, applicationUserRequest.NewPassword);
            if (result.Succeeded)
            {
                return Ok(new ReturnModelResponse {
                    ReturnCode = 200 ,
                    ReturnMessage = "Password updated successfully."
                });
            }
            else
            {
                return BadRequest(new ReturnModelResponse
                {
                    ReturnCode = 400,
                    ReturnMessage = string.Join(", ",result.Errors.Select(er=>er.Description))
                });
            }
        }
    }
}
