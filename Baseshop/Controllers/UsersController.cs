using Baseshop.Dtos;
using Baseshop.Interface;
using Baseshop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Baseshop.Controllers
{
    //[Authorize(Roles = "System,Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly WebContext _context;
        private readonly IUsersService _usersService;

        public UsersController(WebContext context,IUsersService usersService)
        {
            _context = context;
            _usersService = usersService;
        }

        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsersDto>>> GetUsers([FromQuery] string? account, [FromQuery] string? email, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            account = string.IsNullOrWhiteSpace(account) ? null : account.Trim();
            email = string.IsNullOrWhiteSpace(email) ? null : email.Trim();

            var result = await _usersService.GetUsers(account, email, startDate, endDate);

            if (result == null)
            {
                return Ok(new List<UsersDto>());
            }

            return Ok(result);
        }

        // POST: api/users
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UsersCreateDto user)
        {

            await _usersService.CreateUser(user);

            return CreatedAtAction(nameof(GetUserById), new { id = user.Account }, user);
        }

        // GET: api/users/{id}
        // 說明：依識別碼查詢單一使用者。此為 API 標準設計，取代原先的 Edit(GET) 與 Delete(GET) 畫面邏輯
        [HttpGet("{id}")]
        public async Task<ActionResult<UsersDto>> GetUserById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("識別碼不得為空");
            }

            var user = await _usersService.EditGetUser(id);

            if (user == null)
            {
                return NotFound($"找不到帳號為 {id} 的使用者");
            }

            return Ok(user);
        }

        // PUT: api/users/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(string id, [FromBody] UsersEditDto user)
        {
            if (id != user.Account)
            {
                return BadRequest("網址 ID 與資料內容的帳號不符");
            }

            var userName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            await _usersService.EditUser(id, user, userName);

            return NoContent(); // 成功修改，標準回傳 204 No Content
        }

        // DELETE: api/users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound($"找不到帳號為 {id} 的使用者，無法刪除");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent(); // 成功刪除，標準回傳 204 No Content
        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Account == id);
        }
    }
}
