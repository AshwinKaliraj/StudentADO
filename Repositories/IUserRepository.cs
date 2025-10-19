using StudentADO.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentADO.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(int userId);
        Task<User> GetByEmailAsync(string email);
        Task<List<User>> GetAllAsync();
        Task<int> CreateAsync(User user);
        Task<bool> UpdateAsync(User user);
        Task<bool> DeleteAsync(int userId);
        Task<bool> EmailExistsAsync(string email);
    }
}
