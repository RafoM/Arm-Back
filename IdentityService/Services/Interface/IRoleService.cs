using IdentityService.Data.Entity;
using IdentityService.Models.RequestModels;

namespace IdentityService.Services.Interface
{
    public interface IRoleService
    {
        Task<IEnumerable<Role>> GetAllRolesAsync();
        Task<Role> GetRoleByIdAsync(int id);
        Task<Role> CreateRoleAsync(string roleName);
        Task UpdateRoleAsync(RoleUpdateModel model);
        Task DeleteRoleAsync(int id);
    }
}
