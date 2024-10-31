using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace ASPCore_MVC.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class APIUserController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public APIUserController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;

        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(string email, string password)
        {
            Console.WriteLine("We're in the Create function!!");
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return BadRequest("Both Email and password are required");
            }
            var user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                return Ok("Registered successfully");
            }
            //logs errors
            return BadRequest(result.Errors);
        }
        [HttpPost("auth")]
        public async Task<IActionResult> Auth(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return BadRequest("Both Email and password are required");
            }

            var res = await _signInManager.PasswordSignInAsync(email, password, false, false);
            if (res.Succeeded)
            {
                return Ok("Authorised successfully");
            }
            else
            {
                return BadRequest("Authorisation failed. Check login or password.");
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("roles")]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (await _userManager.IsInRoleAsync(await _userManager.FindByNameAsync(User.Identity.Name), "Admin"))
            {
                if (string.IsNullOrWhiteSpace(roleName))
                {
                    return BadRequest("Empty role name.");
                }
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    var res = await _roleManager.CreateAsync(new IdentityRole(roleName));
                    if (res.Succeeded)
                    {
                        return Ok($"Role {roleName} was created");
                    }
                    else
                    {
                        return BadRequest(Json(res));
                    }

                }
                else
                {
                    return BadRequest("Role with such name already exists.");
                }
            }
            return BadRequest("Admin only");
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("roles/assign")]
        public async Task<IActionResult> AssignRole(string name, string roleName)
        {
            if (await _userManager.IsInRoleAsync(await _userManager.FindByNameAsync(User.Identity.Name), "Admin"))
            {
                if (string.IsNullOrWhiteSpace(roleName) || string.IsNullOrWhiteSpace(roleName))
                {
                    return BadRequest("Empty user or role name.");
                }
                var user = await _userManager.FindByNameAsync(name);
                if (await _roleManager.RoleExistsAsync(roleName) && user != null)
                {
                    var res = await _userManager.AddToRoleAsync(user, roleName);

                    if (res.Succeeded)
                    {
                        return Ok($"{name} is now {roleName}");
                    }
                    else
                    {
                        return BadRequest(Json(res));
                    }

                }
                else
                {
                    return BadRequest("Role or user with such name doesn't exists.");
                }
            }
            return BadRequest("Admin only");
        }
    }
}
