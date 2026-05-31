using Baseshop.Dtos;
using Baseshop.Interface;
using Baseshop.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Baseshop.Services
{
    public class UsersService : IUsersService
    {
        private readonly WebContext _context;

        public UsersService(WebContext context)
        {
            _context = context;
        }

        public async Task CreateUser(UsersCreateDto user)
        {
            User insert = new User()
            {
                Account = user.Account,
                UserName = user.UserName,
                Email = user.Email,
                Password = user.Password,
                Role = user.Role
            };
            _context.Users.Add(insert);

            await _context.SaveChangesAsync();
        }

        public async Task<UsersEditDto> EditGetUser(string id)
        {
            var user = from a in _context.Users
                             where a.Account == id
                             select new UsersEditDto
                             {
                                 Account = a.Account,
                                 UserName = a.UserName,
                                 Email = a.Email,
                                 Password = a.Password,
                                 Role = a.Role
                             };
            return await user.SingleOrDefaultAsync();
        }

        public async Task EditUser(string id, UsersEditDto user, string updaterName)
        {
            var update = _context.Users.Find(id);

            if (update != null)
            {
                update.Account = user.Account;
                update.UserName = user.UserName;
                update.Email = user.Email;
                update.Password = user.Password;
                update.Role = user.Role;
                update.LastUpdatedBy = updaterName;
                update.LastUpdatedTime = DateTime.Now;

                await _context.SaveChangesAsync();
            }
        }


        public async Task<List<UsersDto>> GetUsers(string account, string email, DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(account))
            {
                query = query.Where(u => u.Account.Contains(account));
            }

            if (!string.IsNullOrEmpty(email))
            {
                query = query.Where(u => u.Email.Contains(email));
            }

            if (startDate.HasValue)
            {
                query = query.Where(u => u.LastUpdatedTime >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                var nextDay = endDate.Value.AddDays(1);
                query = query.Where(u => u.LastUpdatedTime < nextDay);
            }

            return await query
                .Select(u => new UsersDto
                {
                    Account = u.Account,
                    UserName = u.UserName,
                    Email = u.Email,
                    LastUpdatedTime = u.LastUpdatedTime,
                    LastUpdatedBy = u.LastUpdatedBy
                })
                .ToListAsync();
        }
    }
}
