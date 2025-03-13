using IdentityService.Data.Entity;
using IdentityService.Data;
using IdentityService.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Services.Implementation
{
    public class RoleService : IRoleService
    {
        private readonly IdentityDbContext _dbContext;

        public RoleService(IdentityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            return await _dbContext.Roles.ToListAsync();
        }

        public async Task<Role> GetRoleByIdAsync(int id)
        {
            var role = await _dbContext.Roles.FindAsync(id);
            return role; 
        }

        public async Task<Role> CreateRoleAsync(string roleName)
        {
            var exists = await _dbContext.Roles.AnyAsync(r => r.Name == roleName);
            if (exists)
                throw new Exception($"Role '{roleName}' already exists.");

            var role = new Role
            {
                Name = roleName
            };

            _dbContext.Roles.Add(role);
            await _dbContext.SaveChangesAsync();

            return role;
        }

        public async Task UpdateRoleAsync(int id, string newRoleName)
        {
            var role = await _dbContext.Roles.FindAsync(id);
            if (role == null) throw new Exception($"Role with ID={id} not found.");

            var nameTaken = await _dbContext.Roles.AnyAsync(r => r.Name == newRoleName && r.Id != id);
            if (nameTaken) throw new Exception($"Another role with the name '{newRoleName}' already exists.");

            role.Name = newRoleName;
            _dbContext.Roles.Update(role);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteRoleAsync(int id)
        {
            var role = await _dbContext.Roles.FindAsync(id);
            if (role == null) throw new Exception($"Role with ID={id} not found.");

            bool inUse = await _dbContext.Users.AnyAsync(u => u.RoleId == id);
            if (inUse) throw new Exception("Cannot delete a role assigned to existing users.");

            _dbContext.Roles.Remove(role);
            await _dbContext.SaveChangesAsync();
        }
    }
}
