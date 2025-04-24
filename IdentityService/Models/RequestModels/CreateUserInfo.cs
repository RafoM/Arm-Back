namespace IdentityService.Models.RequestModels
{
    public interface CreateUserInfo
    {
        Guid UserId { get; }
        Guid? ReferrerId { get; }    
        string? PromoCode { get; }   
    }
}
