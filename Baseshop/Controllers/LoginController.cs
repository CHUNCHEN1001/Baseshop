using Baseshop.Dtos;
using Baseshop.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Baseshop.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly WebContext _webContext;

        public LoginController(WebContext webContext)
        {
            _webContext = webContext;
        }

        public IActionResult Index()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return View();
        }

        [HttpPost]
        public IActionResult Index(LoginDto value)
        {
            if (!ModelState.IsValid)
            {
                return View(value);
            }

            var employee = _webContext.Users.FirstOrDefault(e => e.Account.ToLower() == value.Account.ToLower() && e.Password == value.Password);

            if (employee != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, employee.Account),
                    new Claim(ClaimTypes.Name, employee.UserName),
                    new Claim(ClaimTypes.Role, employee.Role)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Products");
            }
            else
            {
                ViewBag.Message = "帳號或密碼錯誤";
                return View(value);
            }
        }

        // GET: /Login/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Login/Create
        [HttpPost]
        public IActionResult Create(GuestCreateDto value)
        {
            if (ModelState.IsValid)
            {
                // 1. 檢查帳號是否重複
                if (_webContext.Users.Any(u => u.Account == value.Account))
                {
                    ModelState.AddModelError("Account", "此帳號已存在");
                    return View(value);
                }

                // 2. 轉換 Dto 為 Model 並存檔 (建議實務上要進行密碼雜湊 Hash)
                var user = new User // 假設您的實體名稱是 User
                {
                    Account = value.Account,
                    UserName = value.UserName,
                    Email = value.Email,
                    Password = value.Password,
                    Role = "Normal"
                };

                _webContext.Users.Add(user);
                _webContext.SaveChanges();

                // 註冊成功後導回登入頁，或直接幫他登入
                return RedirectToAction(nameof(Index));
            }
            return View(value);
        }

    }
}
