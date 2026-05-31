using Baseshop.Dtos;
using Baseshop.Models;

namespace Baseshop.Interface
{
    public interface IUsersService
    {
        Task<List<UsersDto>> GetUsers(string account, string email, DateTime? startDate, DateTime? endDate);

        Task CreateUser(UsersCreateDto user);

        Task<UsersEditDto> EditGetUser(string id);

        Task EditUser(string id, UsersEditDto user, string updaterName);
    }
}
