namespace IdentityService.Models.RequestModels
{
    public class GoogleRegistrRequestModel
    {
        public string IdToken { get; set; }
        public string? ReferralCode { get; set; }
        public string? PromoCode { get; set; }
    }
}
