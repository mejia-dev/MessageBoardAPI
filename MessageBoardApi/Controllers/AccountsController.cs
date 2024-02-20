using MessageBoardApi.ViewModels;
using MessageBoardApi.Models;
using Microsoft.AspNetCore.Mvc;
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

        // public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, MessageBoardApiContext db)
        // {

        // }

        public AccountsController(IConfiguration config, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, MessageBoardApiContext db)
        {
            _config = config;
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel registration)
        {
            ApplicationUser userExists = await _userManager.FindByEmailAsync(registration.Email);
            if (userExists != null)
            {
                return BadRequest(new { status = "Error", message = "Email has already been used for an account." });
            }

            ApplicationUser newUser = new ApplicationUser() { Email = registration.Email};
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
            //your logic for login process
            //If login usrename and password are correct then proceed to generate token
            if (!ModelState.IsValid)
            {
                return Unauthorized();
            }
            else
            {
                Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(loginRequest.Email, loginRequest.Password, isPersistent: true, lockoutOnFailure: false);
                if (result.Succeeded)
                {

                    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                    var Sectoken = new JwtSecurityToken(_config["Jwt:Issuer"],
                    _config["Jwt:Issuer"],
                    null,
                    expires: DateTime.Now.AddMinutes(120),
                    signingCredentials: credentials);

                    var token = new JwtSecurityTokenHandler().WriteToken(Sectoken);

                    return Ok(token);
                }
                else
                {
                    return Unauthorized();
                }
            }
        }


        // [HttpPost]
        // public async Task<IActionResult> Register(RegisterViewModel model)
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         return View(model);
        //     }
        //     else
        //     {
        //         ApplicationUser newUser = new ApplicationUser { UserName = model.Email };
        //         IdentityResult result = await _userManager.CreateAsync(newUser, model.Password);
        //         if (result.Succeeded)
        //         {
        //             return RedirectToAction("Index");
        //         }
        //         else
        //         {
        //             foreach (IdentityError error in result.Errors)
        //             {
        //                 ModelState.AddModelError("", error.Description);
        //             }
        //             return View(model);
        //         }
        //     }
        // }
    }
}