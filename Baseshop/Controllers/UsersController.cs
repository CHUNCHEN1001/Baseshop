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
    [Authorize(Roles = "System,Admin")]
    public class UsersController : Controller
    {
        private readonly WebContext _context;
        private readonly IUsersService _usersService;

        public UsersController(WebContext context,IUsersService usersService)
        {
            _context = context;
            _usersService = usersService;
        }

        // GET: Users
        public async Task<IActionResult> Index(string account, string email, DateTime? startDate, DateTime? endDate)
        {
            var result = await _usersService.GetUsers(account, email, startDate, endDate);

            return View(result);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsersCreateDto user)
        {
            if (ModelState.IsValid)
            {
                await _usersService.CreateUser(user);

                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _usersService.EditGetUser(id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UsersEditDto user)
        {
            if (id != user.Account)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var userName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

                await _usersService.EditUser(id, user, userName);
                
                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await (from a in _context.Users
                              where a.Account == id
                              select new UsersDto
                              {
                                  Account = a.Account,
                                  UserName = a.UserName,
                                  Email = a.Email,
                                  Password = a.Password,
                                  Role = a.Role,
                                  LastUpdatedTime = a.LastUpdatedTime,
                                  LastUpdatedBy = a.LastUpdatedBy
                              }).SingleOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Account == id);
        }
    }
}
