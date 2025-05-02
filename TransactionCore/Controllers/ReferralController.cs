using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TransactionCore.Models.RequestModels;
using TransactionCore.Models.ResponseModels;
using TransactionCore.Services.Interface;

namespace TransactionCore.Controllers
{
    public class ReferralController : BaseController
    {
        private readonly IReferralService _referralService;
        private Guid UserId => Guid.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);

        public ReferralController(IReferralService referralService)
        {
            _referralService = referralService;
        }

        /// <summary>
        /// Returns overall referral statistics.
        /// </summary>
        [HttpGet("summary")]
        public async Task<ActionResult<ReferralSummaryResponseModel>> GetSummary()
        {
            var summary = await _referralService.GetReferralSummaryAsync(UserId);
            return Ok(summary);
        }

        /// <summary>
        /// Returns referral conversion stats.
        /// </summary>
        [HttpGet("conversion-stats")]
        public async Task<ActionResult<ReferralConversionStatsResponseModel>> GetConversionStats()
        {
            var stats = await _referralService.GetReferralConversionStatsAsync(UserId);
            return Ok(stats);
        }

        /// <summary>
        /// Returns registration counts by time range.
        /// </summary>
        [HttpGet("registrations")]
        public async Task<ActionResult<List<TimedStatResponseModel>>> GetRegistrations([FromQuery] int range = 1)
        {
            var result = await _referralService.GetRegistrationsAsync(UserId, range);
            return Ok(result);
        }

        /// <summary>
        /// Returns purchase counts by time range.
        /// </summary>
        [HttpGet("purchases")]
        public async Task<ActionResult<List<TimedStatResponseModel>>> GetPurchases([FromQuery] int range = 1)
        {
            var result = await _referralService.GetPurchasesAsync(UserId, range);
            return Ok(result);
        }

        /// <summary>
        /// Returns referral activity list.
        /// </summary>
        [HttpGet("activity")]
        public async Task<ActionResult<List<ReferralActivityResponseModel>>> GetActivity()
        {
            var result = await _referralService.GetReferralActivityAsync(UserId);
            return Ok(result);
        }

        /// <summary>
        /// Returns referral payment history.
        /// </summary>
        [HttpGet("payments")]
        public async Task<ActionResult<List<ReferralPaymentResponseModel>>> GetPayments()
        {
            var result = await _referralService.GetReferralPaymentsAsync(UserId);
            return Ok(result);
        }

        /// <summary>
        /// Returns referral withdrawal history.
        /// </summary>
        [HttpGet("withdrawals")]
        public async Task<ActionResult<List<ReferralWithdrawalResponseModel>>> GetWithdrawals()
        {
            var result = await _referralService.GetReferralWithdrawalsAsync(UserId);
            return Ok(result);
        }

        /// <summary>
        /// Creates a withdrawal request.
        /// </summary>
        [HttpPost("withdrawals")]
        public async Task<IActionResult> RequestWithdrawal([FromBody] ReferralWithdrawalRequestModel request)
        {
            await _referralService.CreateReferralWithdrawalAsync(UserId, request);
            return Ok();
        }
    }
}
