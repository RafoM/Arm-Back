using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TransactionCore.Models.RequestModels;
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

        /// <summary>
        /// Retrieves payment details for a user and applies promo logic if provided.
        /// </summary>
        /// <param name="userId">The user's GUID.</param>
        /// <param name="requestModel">The payment details request data.</param>
        /// <returns>Detailed payment info prepared for the transaction.</returns>
        [HttpPost]
        [Authorize] // Optional: Add based on your app's authentication strategy
        [ProducesResponseType(typeof(PaymentDetailsResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PaymentDetailsResponseModel>> GetPaymentDetails([FromBody] PaymentDetailsRequestModel requestModel)
        {
            try
            {
                var result = await _paymentService.GetPaymentDetails(UserId, requestModel);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
