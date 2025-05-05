using IdentityService.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers
{
    public class VisitController : BaseController
    {
        private readonly IUserService _userService;

        public VisitController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Set referral url visit
        /// </summary>
        /// <param name="referralCode"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ReferalVisit([FromBody] string referralCode)
        {
            await _userService.ReferralVisit(referralCode);
            return Ok();
        }
    }
}
