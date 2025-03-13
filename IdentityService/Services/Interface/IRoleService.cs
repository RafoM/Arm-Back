using IdentityService.Data.Entity;

namespace IdentityService.Services.Interface
{
    public interface IRoleService
    {
        Task<IEnumerable<Role>> GetAllRolesAsync();
        Task<Role> GetRoleByIdAsync(int id);
        Task<Role> CreateRoleAsync(string roleName);
        Task UpdateRoleAsync(int id, string newRoleName);
        Task DeleteRoleAsync(int id);
    }
}
