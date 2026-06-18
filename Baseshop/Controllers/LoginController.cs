using Baseshop.Dtos;
using Baseshop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Baseshop.Controllers
{
    [AllowAnonymous]
    [ApiController] 
    [Route("api/[controller]")] 
    public class LoginController : ControllerBase
    {
        private readonly WebContext _webContext;
        private readonly IConfiguration _configuration;

        public LoginController(WebContext webContext, IConfiguration configuration)
        {
            _webContext = webContext;
            _configuration = configuration;
        }

        [HttpPost("jwtLogin")]
        public ApiResponseDto jwtLogin(LoginDto value)
        {
            var user = (from a in _webContext.Users
                        where a.Account == value.Account
                        && a.Password == value.Password
                        select a).SingleOrDefault();

            if (user == null)
            {
                return new ApiResponseDto
                {
                    Status = 0,
                    Message = "帳號密碼錯誤"
                };
            }
            else
            {

                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Email, user.Account),
                    new Claim("FullName", user.UserName),
                    new Claim(ClaimTypes.Role, user.Role),
                };

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:KEY"]));

                var jwt = new JwtSecurityToken
                (
                    issuer: _configuration["JWT:Issuer"],
                    audience: _configuration["JWT:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
                );

                var token = new JwtSecurityTokenHandler().WriteToken(jwt);


                return new ApiResponseDto
                {
                    Data = token,
                    Status = 1
                };

            }
        }

        // POST: api/Login
        [HttpPost]
        public IActionResult Authenticate([FromBody] LoginDto value) // 4. 明確指定從 Body 接收 JSON
        {
            // 注意：[ApiController] 會自動處理 !ModelState.IsValid，此處不需手動檢查

            var employee = _webContext.Users.FirstOrDefault(e =>
                e.Account.ToLower() == value.Account.ToLower() &&
                e.Password == value.Password);

            if (employee != null)
            {
                // TODO: 建議在 Web API 改用 JWT Token。
                // 這裡先回傳成功訊息與使用者基本資訊
                return Ok(new
                {
                    Message = "登入成功",
                    User = new { employee.Account, employee.UserName, employee.Role }
                    // Token = "產生的 JWT Token 放在這裡"
                });
            }

            return BadRequest(new { Message = "帳號或密碼錯誤" }); // 5. 回傳 400 錯誤與 JSON 訊息
        }

        // POST: api/Login/Create
        [HttpPost("Create")]
        public IActionResult Create([FromBody] GuestCreateDto value)
        {
            // 1. 檢查帳號是否重複
            if (_webContext.Users.Any(u => u.Account == value.Account))
            {
                return BadRequest(new { Message = "此帳號已存在" });
            }

            var user = new User
            {
                Account = value.Account,
                UserName = value.UserName,
                Email = value.Email,
                Password = value.Password, 
                Role = "Normal"
            };

            _webContext.Users.Add(user);
            _webContext.SaveChanges();

            return Ok(new { Message = "註冊成功" }); // 6. 回傳 200 成功
        }
    }
}
