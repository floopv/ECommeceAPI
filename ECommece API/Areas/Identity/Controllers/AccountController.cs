using ECommece_API.DTOs.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommece_API.Areas.Identity.Controllers
{
    [Route("[Area]/[controller]")]
    [ApiController]
    [Area("Identity")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public AccountController(UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterRequest registerRequest)
        {
            //ApplicationUser applicationUser = registerVM.Adapt<ApplicationUser>();
            ApplicationUser applicationUser = new ApplicationUser
            {
                FirstName = registerRequest.FirstName,
                LastName = registerRequest.LastName,
                UserName = registerRequest.UserName,
                Email = registerRequest.Email
            };
            var result = await _userManager.CreateAsync(applicationUser, registerRequest.Password);
            if (!result.Succeeded)
            {
                string errors = string.Empty;
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description} \n";
                }
                return BadRequest(new
                {
                    msg=errors
                });
            }
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(applicationUser);
            var link = Url.Action(nameof(ConfirmEmail), "Account", new { Area = "Identity", token, userId = applicationUser.Id }, Request.Scheme);
            string htmlMessage = $@"
<div style='font-family: Arial, sans-serif; background-color: #f7f7f7; padding: 20px;'>
    <div style='max-width: 600px; margin: auto; background: white; border-radius: 8px; box-shadow: 0 2px 8px rgba(0,0,0,0.1); overflow: hidden;'>
        <div style='background-color: #007bff; color: white; padding: 15px; text-align: center;'>
            <h2 style='margin: 0;'>Ecommerce 520</h2>
        </div>
        <div style='padding: 20px;'>
            <h3 style='color: #333;'>Confirm your email address</h3>
            <p style='color: #555; line-height: 1.6;'>
                Hi <strong>{registerRequest.FirstName}</strong>,<br><br>
                Thank you for registering with <strong>Ecommerce 520</strong>.<br>
                Please click the button below to confirm your email address.
            </p>

            <div style='text-align: center; margin: 30px 0;'>
                <a href='{link}'
                   style='background-color: #007bff; color: white; padding: 12px 25px;
                          text-decoration: none; border-radius: 6px; display: inline-block;'>
                    Confirm Email
                </a>
            </div>

            <p style='font-size: 14px; color: #777;'>
                If you did not create this account, please ignore this email.
            </p>
        </div>
        <div style='background-color: #f0f0f0; text-align: center; padding: 10px; font-size: 12px; color: #999;'>
            © 2025 Ecommerce 520. All rights reserved.
        </div>
    </div>
</div>";

            await _emailSender.SendEmailAsync(registerRequest.Email, "Ecommerce Confirmation Mail", htmlMessage);
            return Ok(new
            {
                msg = "Registeration Successful"
            });
        }
        [HttpPost("ConfirmEmail")]

        public async Task<IActionResult> ConfirmEmail(string token, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                return NotFound(new
                {
                    msg = "Invalid User"
                });
            }
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    msg = "Email Confirmation Failed"
                });
            }
            else
            {
                return Ok(new
                {
                    msg = "Email Confirmed Successfully"
                });
            }
        }
    }
}
