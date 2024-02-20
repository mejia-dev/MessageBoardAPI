using MessageBoardApi.ViewModels;
using MessageBoardApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace MessageBoardApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private IConfiguration _config;
        private readonly MessageBoardApiContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountsController(IConfiguration config, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, MessageBoardApiContext db)
        {
            _config = config;
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterViewModel registration)
        {
            ApplicationUser userExists = await _userManager.FindByEmailAsync(registration.Email);
            if (userExists != null)
            {
                return BadRequest(new { status = "Error", message = "Email has already been used for an account." });
            }

            ApplicationUser newUser = new ApplicationUser() { UserName = registration.UserName, Email = registration.Email };
            IdentityResult result = await _userManager.CreateAsync(newUser, registration.Password);

            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                return BadRequest(result.Errors);
            }

        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] LoginViewModel loginRequest)
        {

            #nullable enable
            ApplicationUser? requestedUser = await _userManager.FindByEmailAsync(loginRequest.Email);
            #nullable disable

            if (requestedUser == null)
            {
                return BadRequest(new { status = "Error", message = $"Unable to sign in. Please check credentials and try again. ERROR1" });
            }

            Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(requestedUser, loginRequest.Password, isPersistent: true, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:SecretKey"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var Sectoken = new JwtSecurityToken(_config["JwtSettings:ValidIssuer"],
                _config["JwtSettings:ValidIssuer"],
                null,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

                var token = new JwtSecurityTokenHandler().WriteToken(Sectoken);
                return Ok(token);
            }
            else
            {
                return BadRequest(new { status = "Error", message = $"Unable to sign in. Please check credentials and try again. ERROR2" });
            }
        }
    }
}