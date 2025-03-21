namespace IdentityService.Models.RequestModels
{
    public class UpdateUserRoleRequestModel
    {
        public Guid UserId { get; set; }
        public int RoleId { get; set; }
    }
}
