using System.Linq.Expressions;
using SMarket.DataAccess.Models;

namespace SMarket.DataAccess.Repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
    }
}
