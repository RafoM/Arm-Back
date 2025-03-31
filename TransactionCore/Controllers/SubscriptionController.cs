using Microsoft.AspNetCore.Mvc;
using TransactionCore.Models.RequestModels;
using TransactionCore.Models.ResponseModels;
using TransactionCore.Services.Interface;

namespace TransactionCore.Controllers
{
    public class SubscriptionController : BaseController
    {
        private readonly ISubscriptionService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionPackageController"/> class.
        /// </summary>
        /// <param name="service">The subscription package service.</param>
        public SubscriptionController(ISubscriptionService service)
        {
            _service = service;
        }

        /// <summary>
        /// Retrieves all subscription packages.
        /// </summary>
        /// <returns>A list of subscription packages.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubscriptionResponseModel>>> GetAll(int? languageId)
        {
            var packages = await _service.GetAllAsync(languageId);
            return Ok(packages);
        }

        /// <summary>
        /// Retrieves a subscription package by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the subscription package.</param>
        /// <returns>The subscription package matching the identifier.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<SubscriptionResponseModel>> GetById(int id)
        {
            var package = await _service.GetByIdAsync(id);
            if (package == null)
                return NotFound();

            return Ok(package);
        }

        /// <summary>
        /// Creates a new subscription package.
        /// </summary>
        /// <param name="request">The subscription package details.</param>
        /// <returns>The newly created subscription package.</returns>
        [HttpPost]
        public async Task<ActionResult<SubscriptionResponseModel>> Create([FromBody] SubscriptionRequestModel request)
        {
            var created = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Updates an existing subscription package.
        /// </summary>
        /// <param name="id">The identifier of the subscription package to update.</param>
        /// <param name="request">The updated subscription package details.</param>
        /// <returns>No content.</returns>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] SubscriptionUpdateModel request)
        {
            await _service.UpdateAsync(request);
            return NoContent();
        }

        /// <summary>
        /// Deletes a subscription package.
        /// </summary>
        /// <param name="id">The identifier of the subscription package to delete.</param>
        /// <returns>No content.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
