using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TransactionCore.Models.ResponseModels;
using TransactionCore.Services.Interface;

namespace TransactionCore.Controllers
{
    [Authorize]
    public class PaymentsController : BaseController
    {
        private readonly IPaymentService _paymentService;
        private Guid UserId => Guid.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        /// <summary>
        /// Get paginated payment history for the current user.
        /// </summary>
        /// <param name="pageNumber">Page number (default is 1)</param>
        /// <param name="pageSize">Page size (default is 10)</param>
        /// <param name="languageId">Language ID from headers</param>
        /// <returns>Paginated list of user payments</returns>
        [HttpGet("all")]
        public async Task<ActionResult<PageResultModel<UserPaymentResponseModel>>> GetUserPayments(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromHeader(Name = "LanguageId")] int languageId = 1)
        {
            var result = await _paymentService.GetUserPaymentsAsync(UserId, languageId, pageNumber, pageSize);
            return Ok(result);
        }
    }
}
