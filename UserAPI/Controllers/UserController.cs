using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UserAPI.Model;
using UserAPI.Providers.Interface;

namespace UserAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IJWTProvider _jwtprovider;
        private readonly UserManager<IdentityUser> _usermanager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public UserController(IJWTProvider jwtprovider, UserManager<IdentityUser> usermanager,
                              SignInManager<IdentityUser> signInManager)
        {
            _jwtprovider = jwtprovider;
            _usermanager = usermanager;
            _signInManager = signInManager;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] Register register)
        {
            var user = new IdentityUser
            {
                UserName = register.UserName
            };
            var registerResult = await _usermanager.CreateAsync(user, register.Password);

            var registerModel = new
            {
                Message = registerResult.Succeeded ? "Registration Has Been Successfully Completed" : "Registration Failed!!!",
                IsRegistrationSuccessful = registerResult.Succeeded
            };

            return Ok(registerModel);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] Login login)
        {
            var jwToken = string.Empty;
            var loginResult = await _signInManager.PasswordSignInAsync(login.UserName,
                                                                       login.Password,
                                                                       login.RememberMe,
                                                                       false);
            var loginSucceeded = loginResult.Succeeded;

            if (loginSucceeded)
            {
                jwToken = await _jwtprovider.GetTokenAsync(login.UserName);
            }

            var loginModel = new
            {
                Message = loginResult.Succeeded ? "Login Is Successful" : "Login Failed!!!",
                Token = jwToken,
                IsLoginSuccessful = loginResult.Succeeded
            };

            return Ok(loginModel);
        }

        [HttpPost("PasswordReset")]
        public async Task<IActionResult> PasswordReset([FromBody] PasswordReset passwordReset)
        {
            var findbyNameResult = await _usermanager.FindByNameAsync(passwordReset.UserName);
            var passwordResetToken = await _usermanager.GeneratePasswordResetTokenAsync(findbyNameResult);
            var passwordResetResult = await _usermanager.ResetPasswordAsync(findbyNameResult,
                                                        passwordResetToken, passwordReset.NewPassword);

            var passwordResetModel = new
            {
                Message = passwordResetResult.Succeeded ? "Password Reset Is Successful" : "Password Reset Failed!!!",
                IsPasswordResetSuccessful = passwordResetResult.Succeeded
            };

            return Ok(passwordResetModel);
        }

        [HttpPost("LogOut")]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();

            var logOutModel = new
            {
                Message = "Logged Out Successfully",
                IsLoggedOutSuccessfully = true
            };

            return Ok(logOutModel);
        }
    }
}