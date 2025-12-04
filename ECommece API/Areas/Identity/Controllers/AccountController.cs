using ECommece_API.DTOs.Request;
using ECommece_API.DTOs.Response;
using ECommerceAPI.Repos;
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
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IRepository<ApplicationUserOTP> _applicationUserOTPRepository;

        public AccountController(UserManager<ApplicationUser> userManager, IEmailSender emailSender, SignInManager<ApplicationUser> signInManager, IRepository<ApplicationUserOTP> applicationUserOTPRepository)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _signInManager = signInManager;
            _applicationUserOTPRepository = applicationUserOTPRepository;
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
                return BadRequest(new ReturnModelResponse
                {
                    ReturnCode = 400 ,
                    ReturnMessage = errors
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
            return Ok(new ReturnModelResponse
            {
                ReturnCode = 200 ,
                ReturnMessage = "Registeration successful , check your email to confirm"
            });
        }
        [HttpPost("ConfirmEmail")]

        public async Task<IActionResult> ConfirmEmail(string token, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                return NotFound(new ReturnModelResponse
                {
                    ReturnCode = 404 ,
                    ReturnMessage = "User Not Found"
                });
            }
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return BadRequest(new ReturnModelResponse
                {
                    ReturnCode = 400,
                    ReturnMessage = "Email Confirmation Failed"
                });
            }
            else
            {
                return Ok(new ReturnModelResponse
                {
                    ReturnCode = 200,
                    ReturnMessage = "Email Confirmed Successfully"
                });
            }
        }

        [HttpPost("ResendEmailConfirmation")]
        public async Task<IActionResult> ResendEmailConfirmation(ResesndEmailConfirmationRequest resendEmailConfirmationRequest)
        {
            if (resendEmailConfirmationRequest == null)
            {
                return BadRequest(new ReturnModelResponse
                {
                    ReturnCode = 400,
                    ReturnMessage = "Invalid Inputs"
                });
            }
            var user = await _userManager.FindByEmailAsync(resendEmailConfirmationRequest.EmailOrUserName) ?? await _userManager.FindByNameAsync(resendEmailConfirmationRequest.EmailOrUserName);
            if (user is null)
            {
                return NotFound(new ReturnModelResponse
                {
                    ReturnCode = 404,
                    ReturnMessage = "User not found"
                });
            }
            if (user.EmailConfirmed)
            {
                return BadRequest(new ReturnModelResponse
                {
                    ReturnCode = 400,
                    ReturnMessage = "Email is already confirmed"
                });
            }
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action(nameof(ConfirmEmail), "Account", new { Area = "Identity", token, userId = user.Id }, Request.Scheme);
            string htmlMessage = $@"
<div style='font-family: Arial, sans-serif; background-color: #f7f7f7; padding: 20px;'>
    <div style='max-width: 600px; margin: auto; background: white; border-radius: 8px; box-shadow: 0 2px 8px rgba(0,0,0,0.1); overflow: hidden;'>
        <div style='background-color: #007bff; color: white; padding: 15px; text-align: center;'>
            <h2 style='margin: 0;'>Ecommerce 520</h2>
        </div>
        <div style='padding: 20px;'>
            <h3 style='color: #333;'>Confirm your email address</h3>
            <p style='color: #555; line-height: 1.6;'>
                Hi <strong>{user.FirstName}</strong>,<br><br>
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

            await _emailSender.SendEmailAsync(user.Email, "Ecommerce Confirmation Mail", htmlMessage);

            return Ok(new ReturnModelResponse
            {
                ReturnCode = 200,
                ReturnMessage = "Conftrmation email resent successfully"
            });
        }

        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordRequest forgetPasswordRequest)
        {
            if (forgetPasswordRequest == null)
            {
                return BadRequest(new ReturnModelResponse
                {
                    ReturnCode = 400,
                    ReturnMessage = "Invalid Inputs"
                });
            }
            var user = await _userManager.FindByEmailAsync(forgetPasswordRequest.EmailOrUserName) ?? await _userManager.FindByNameAsync(forgetPasswordRequest.EmailOrUserName);
            if (user is null)
            {
                return NotFound(new ReturnModelResponse
                {
                    ReturnCode = 404,
                    ReturnMessage = "User not found"
                });
            }
            if (!user.EmailConfirmed)
            {
                return BadRequest(new ReturnModelResponse
                {
                    ReturnCode = 400,
                    ReturnMessage = "Confirm your Email First"
                });
            }
            var existingOTP = await _applicationUserOTPRepository.GetAllAsync(otp => otp.ApplicationUserId == user.Id);
            var last24HrsOTP = existingOTP.Count(otp => otp.CreatedAt > DateTime.Now.AddHours(-24));
            if (last24HrsOTP >= 5)
            {
                return BadRequest(new ReturnModelResponse
                {
                    ReturnCode = 400,
                    ReturnMessage = "You have reached the maximum number of OTP requests for today. Please try again later."
                });
            }
            foreach (var otp in existingOTP)
            {
                otp.IsValid = false;
                _applicationUserOTPRepository.Update(otp);
                await _applicationUserOTPRepository.CommitAsync();
            }
            var OTP = new Random().Next(1000, 9999).ToString();
            ApplicationUserOTP applicationUserOTP = new ApplicationUserOTP(user.Id, OTP);
            await _applicationUserOTPRepository.AddAsync(applicationUserOTP);
            await _applicationUserOTPRepository.CommitAsync();
            string htmlMessage = $@"
<div style='font-family: Arial, sans-serif; background-color: #f7f7f7; padding: 20px;'>
    <div style='max-width: 600px; margin: auto; background: white; border-radius: 8px; box-shadow: 0 2px 8px rgba(0,0,0,0.1); overflow: hidden;'>
        <div style='background-color: #007bff; color: white; padding: 15px; text-align: center;'>
            <h2 style='margin: 0;'>Ecommerce 520</h2>
        </div>
        <div style='padding: 20px;'>
            <h3 style='color: #333;'>Reset your Password</h3>
            <p style='color: #555; line-height: 1.6;'>
                Hi <strong>{user.FirstName}</strong>,<br><br>
                Thank you for registering with <strong>Ecommerce 520</strong>.<br>
                Please use the OTP below to reset your password.
            </p>

        <div style='text-align: center; margin: 30px 0;'>
                <p style='color: red; padding: 12px 25px;
                          border-radius: 6px; display: inline-block; font-size: 20px; letter-spacing: 3px;'>
                    {OTP}
                </p>
            </div>

            <p style='font-size: 14px; color: #777;'>
                If you did not Request to reset your password, please ignore this email.
            </p>
        </div>
        <div style='background-color: #f0f0f0; text-align: center; padding: 10px; font-size: 12px; color: #999;'>
            © 2025 Ecommerce 520. All rights reserved.
        </div>
    </div>
</div>";

            await _emailSender.SendEmailAsync(user.Email, "Ecommerce Reset Password", htmlMessage);

            return Ok(new ReturnModelResponse
            {
                ReturnCode = 200,
                ReturnMessage = "OTP sent to your registered email"
            });
        }

        [HttpPost("ValidateOTP")]
        public async Task<IActionResult> ValidateOTP(ValidateOTPRequest validateOTPRequest)
        {
            if (validateOTPRequest == null)
            {
                return BadRequest(new ReturnModelResponse
                {
                    ReturnCode = 400,
                    ReturnMessage = "Invalid Inputs"
                });
            }
            var user = await _userManager.FindByIdAsync(validateOTPRequest.ApplicationUserId);
            if (user is null)
            {
                return NotFound(new ReturnModelResponse
                {
                    ReturnCode = 404,
                    ReturnMessage = "User not found"
                });
            }
            var userOTP = await _applicationUserOTPRepository.GetOneAsync(otp => otp.ApplicationUserId == validateOTPRequest.ApplicationUserId &&
            otp.OTP == validateOTPRequest.OTP &&
            otp.IsValid && 
            otp.ExpireAt > DateTime.UtcNow
            );
            if (userOTP is null)
            {
                return BadRequest(new ReturnModelResponse
                {
                    ReturnCode = 400,
                    ReturnMessage = "Invalid OTP"
                });
            }
            userOTP.IsValid = false;
            userOTP.CanResetPassword = true;
            _applicationUserOTPRepository.Update(userOTP);
            await _applicationUserOTPRepository.CommitAsync();
            //TempData["OTPValidateUerId"] = user.Id;
            return Ok(new ReturnModelResponse
            {
                ReturnCode = 200,
                ReturnMessage = "OTP Validated Successfully"
            });
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest resetPasswordRequest)
        {
            if (resetPasswordRequest == null)
            {
                return BadRequest(new ReturnModelResponse
                {
                    ReturnCode = 400,
                    ReturnMessage = "Invalid Inputs"
                });
            }


            var user = await _userManager.FindByIdAsync(resetPasswordRequest.ApplicationUserId);
            if (user is null)
            {
                return NotFound(new ReturnModelResponse
                {
                    ReturnCode = 404 ,
                    ReturnMessage = "User Not Found"
                });
            }
            var CanResetPass = await _applicationUserOTPRepository.GetOneAsync(otp => otp.ApplicationUserId == resetPasswordRequest.ApplicationUserId
          && otp.CanResetPassword);
            if (CanResetPass is null)
            {
                return BadRequest(new ReturnModelResponse
                {
                    ReturnCode = 400 , 
                    ReturnMessage = "Unauthorized Access to Reset Password"
                });
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, resetPasswordRequest.NewPassword);
            if (!result.Succeeded)
            {
                string errors = string.Empty;
                foreach (var error in result.Errors)
                {
                    errors += $"{error} \n";
                }
                return BadRequest(new ReturnModelResponse
                {
                    ReturnCode = 400 ,
                    ReturnMessage = errors
                });
            }
            CanResetPass.CanResetPassword = false;
            _applicationUserOTPRepository.Update(CanResetPass);
            await _applicationUserOTPRepository.CommitAsync();
            return Ok(new ReturnModelResponse
            {
                ReturnCode = 200,
                ReturnMessage = "Password Is Reset Successfully"
            });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            if (loginRequest is not null)
            {
                var user = await _userManager.FindByEmailAsync(loginRequest.UserNameOrEmail) ?? await _userManager.FindByNameAsync(loginRequest.UserNameOrEmail);
                if (user is not null)
                {
                    //var isPasswordValid = await _userManager.CheckPasswordAsync(user , loginVM.Password);
                    var signInResult = await _signInManager.PasswordSignInAsync(user, loginRequest.Password, loginRequest.RememberMe, true);
                    if (signInResult.Succeeded)
                    {
                        return Ok(new ReturnModelResponse { 
                            ReturnCode = 200 ,
                            ReturnMessage = "Login Successful"
                        });
                    }
                    else
                    {
                        if (signInResult.IsLockedOut)
                        {
                           return BadRequest (new ReturnModelResponse
                           {
                               ReturnCode = 400 ,
                               ReturnMessage = "Your account is locked due to multiple failed login attempts. Please try again later."
                           });
                        }
                        else if (!user.EmailConfirmed)
                        {
                            return BadRequest(new ReturnModelResponse
                            {
                                ReturnCode = 400,
                                ReturnMessage = "Please confirm your email before logging in."
                            });
                        }
                        else
                        {
                            return BadRequest(new ReturnModelResponse
                            {
                                ReturnCode = 400,
                                ReturnMessage = "Invalid Credentials , Login Failed"
                            });
                        }
                    }
                }
                else
                {
                    return BadRequest(new ReturnModelResponse
                    {
                        ReturnCode = 400,
                        ReturnMessage = "User not found , Login Failed"
                    });
                }
            }
            else
            {
                return BadRequest(new ReturnModelResponse
                {
                    ReturnCode = 400,
                    ReturnMessage = "Invalid Inputs"
                });
            }
        }
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new ReturnModelResponse
            {
                ReturnCode = 200,
                ReturnMessage = "Logout Successful"
            });
        }
    }
}
