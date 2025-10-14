using Microsoft.EntityFrameworkCore;
using SMarket.DataAccess.Context;
using SMarket.DataAccess.Models;
using SMarket.DataAccess.Repositories.Interfaces;

namespace SMarket.DataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            var roles = await _context.Roles.ToListAsync();
            return await _context.Set<User>().Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            var user = await _context.Set<User>()
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id);

            // Debug: Check if user exists and log RoleId
            if (user != null)
            {
                Console.WriteLine($"User found: {user.Name}, RoleId: {user.RoleId}, Role: {(user.Role?.Name ?? "NULL")}");
            }

            return user;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Set<User>().ToListAsync();
        }

        public async Task<User> AddAsync(User entity)
        {
            await _context.Set<User>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(User entity)
        {
            _context.Set<User>().Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var user = await GetByIdAsync(id);
            if (user != null)
            {
                _context.Set<User>().Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(User entity)
        {
            _context.Set<User>().Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        // Debug method to check role relationship
        public async Task<bool> ValidateUserRoleRelationshipAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                Console.WriteLine($"User with ID {userId} not found");
                return false;
            }

            var role = await _context.Roles.FindAsync(user.RoleId);
            if (role == null)
            {
                Console.WriteLine($"Role with ID {user.RoleId} not found for user {user.Name}");
                return false;
            }

            Console.WriteLine($"User: {user.Name} has RoleId: {user.RoleId} which corresponds to Role: {role.Name}");
            return true;
        }
    }
}