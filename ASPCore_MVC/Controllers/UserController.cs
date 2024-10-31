using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

namespace ASPCore_MVC.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (await _userManager.IsInRoleAsync(await _userManager.FindByNameAsync(User.Identity.Name), "Admin")){
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
        [HttpGet]
        public IActionResult AssignRole()
        {
            ViewBag.Users = new List<SelectListItem>();
            ViewBag.Roles = new List<SelectListItem>();

            foreach(var u in _userManager.Users)
            {
                ViewBag.Users.Add(new SelectListItem { Text = u.UserName, Value = u.UserName });
            }
            foreach (var r in _roleManager.Roles)
            {
                ViewBag.Roles.Add(new SelectListItem { Text = r.Name, Value = r.Name });
            }
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
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
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return BadRequest("Both Email and password are required");
            }
            var user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                return Ok(result);
            }
            //logs errors
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
                return RedirectToAction("Index", "Home");
            }
            return BadRequest(ModelState);
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return BadRequest("Both Email and password are required");
            }

            var res = await _signInManager.PasswordSignInAsync(email, password, false, false);
            if (res.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return BadRequest("Sign in failed. Check login or password.");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}
