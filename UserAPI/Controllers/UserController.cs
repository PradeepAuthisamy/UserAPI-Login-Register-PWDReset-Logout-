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
                Message = registerResult.Succeeded ? "Registration has been successfully completed" : "Registration Failed!!!",
                IsRegistrationSuccessful = registerResult.Succeeded
            };

            return Ok(registerModel);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] Login login)
        {
            var loginResult = await _signInManager.PasswordSignInAsync(login.UserName,
                                                                       login.Password,
                                                                       login.RememberMe,
                                                                       false);
            var jwtTokenResult = await _jwtprovider.GetTokenAsync(login.UserName);

            var loginModel = new
            {
                Message = loginResult.Succeeded ? "Login is Successful" : "Login Failed!!!",
                Token = jwtTokenResult,
                IsLoginSuccessful = loginResult.Succeeded
            };

            return Ok(loginModel);
        }

        [HttpPost("LogOut")]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();

            var logOutModel = new
            {
                IsLoggedOutSuccessfully = true
            };

            return Ok(logOutModel);
        }
    }
}