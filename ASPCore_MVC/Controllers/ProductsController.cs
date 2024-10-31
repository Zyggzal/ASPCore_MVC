using ASPCore_MVC.DbContext;
using ASPCore_MVC.Models;
using ASPCore_MVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ASPCore_MVC.Controllers
{
    public class ProductsController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IProductsService _productsService;
        public ProductsController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager, IProductsService productsService, ProductsContext pcontext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _productsService = productsService;
            _productsService.Context = pcontext;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _productsService.Read());
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            return View(await _productsService.Read(id));
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,Description")]Product product)
        {
            await _productsService.Create(product);
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            return View(await _productsService.Read(id));
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update([Bind("Id,Name,Price,Description")] Product product)
        {
            Console.WriteLine(product.Id);
            await _productsService.Update(product);
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            return View(id);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFunc(int id)
        {
            await _productsService.Delete(id);
            return RedirectToAction("Index");
        }
    }
}