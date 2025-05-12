using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransactionCore.Models.RequestModels;
using TransactionCore.Models.ResponseModels;
using TransactionCore.Services.Interface;

namespace TransactionCore.Controllers
{
    [Authorize]
    public class CryptoController : BaseController
    {
        private readonly ICryptoService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="CryptoController"/> class.
        /// </summary>
        /// <param name="service">The crypto service.</param>
        public CryptoController(ICryptoService service)
        {
            _service = service;
        }

        /// <summary>
        /// Retrieves all cryptocurrencies.
        /// </summary>
        /// <returns>A list of cryptocurrencies.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CryptoResponseModel>>> GetAll()
        {
            var cryptos = await _service.GetAllAsync();
            return Ok(cryptos);
        }

        /// <summary>
        /// Retrieves a cryptocurrency by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the cryptocurrency.</param>
        /// <returns>The cryptocurrency matching the identifier.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<CryptoResponseModel>> GetById(Guid id)
        {
            var crypto = await _service.GetByIdAsync(id);
            if (crypto == null)
                return NotFound();

            return Ok(crypto);
        }

        /// <summary>
        /// Creates a new cryptocurrency.
        /// </summary>
        /// <param name="request">The cryptocurrency details.</param>
        /// <returns>The newly created cryptocurrency.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<CryptoResponseModel>> Create([FromBody] CryptoRequestModel request)
        {
            var created = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Updates an existing cryptocurrency.
        /// </summary>
        /// <param name="id">The identifier of the cryptocurrency to update.</param>
        /// <param name="request">The updated cryptocurrency details.</param>
        /// <returns>No content.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] CryptoUpdateModel request)
        {
            await _service.UpdateAsync(request);
            return Ok();
        }

        //UploadCryptoIcon

        /// <summary>
        /// Deletes a cryptocurrency.
        /// </summary>
        /// <param name="id">The identifier of the cryptocurrency to delete.</param>
        /// <returns>No content.</returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return Ok();
        }
        /// <summary>
        /// Uploads a icon for a specific crypto.
        /// </summary>
        /// <param name="request">The flag upload request.</param>
        /// <returns>The URL of the uploaded flag.</returns>
        [HttpPost("icon")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<string>> UploadIcon([FromForm] UploadIqonRequest request)
        {
            if (request.FlagFile == null || request.FlagFile.Length == 0)
            {
                return BadRequest("Invalid icon file.");
            }

            var iconUrl = await _service.UploadIconAsync(request.LanguageId, request.FlagFile);
            return Ok(iconUrl);
        }
    }
}
