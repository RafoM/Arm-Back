using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransactionCore.Models.RequestModels;
using TransactionCore.Models.ResponseModels;
using TransactionCore.Services.Interface;

namespace TransactionCore.Controllers
{
    [Authorize]
    public class PaymentMethodController : BaseController
    {
        private readonly IPaymentMethodService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentMethodController"/> class.
        /// </summary>
        /// <param name="service">The payment method service.</param>
        public PaymentMethodController(IPaymentMethodService service)
        {
            _service = service;
        }

        /// <summary>
        /// Retrieves all payment methods.
        /// </summary>
        /// <returns>A list of payment methods.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentMethodResponseModel>>> GetAll()
        {
            var methods = await _service.GetAllAsync();
            return Ok(methods);
        }

        /// <summary>
        /// Retrieves a payment method by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the payment method.</param>
        /// <returns>The payment method matching the identifier.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentMethodResponseModel>> GetById(Guid id)
        {
            var method = await _service.GetByIdAsync(id);
            if (method == null)
                return NotFound();
                        
            return Ok(method);
        }

        /// <summary>
        /// Creates a new payment method.
        /// </summary>
        /// <param name="request">The payment method details.</param>
        /// <returns>The newly created payment method.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<PaymentMethodResponseModel>> Create([FromBody] PaymentMethodRequestModel request)
        {
            var created = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Updates an existing payment method.
        /// </summary>
        /// <param name="id">The identifier of the payment method to update.</param>
        /// <param name="request">The updated payment method details.</param>
        /// <returns>No content.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] PaymentMethodUpdateModel request)
        {
            await _service.UpdateAsync(request);
            return Ok();
        }

        /// <summary>
        /// Deletes a payment method.
        /// </summary>
        /// <param name="id">The identifier of the payment method to delete.</param>
        /// <returns>No content.</returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return Ok();
        }
    }
}
