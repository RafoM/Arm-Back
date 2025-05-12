using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransactionCore.Models.RequestModels;
using TransactionCore.Models.ResponseModels;
using TransactionCore.Services.Interface;

namespace TransactionCore.Controllers
{
    [Authorize]
    public class NetworkController : BaseController
    {
        private readonly INetworkService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkController"/> class.
        /// </summary>
        /// <param name="service">The network service.</param>
        public NetworkController(INetworkService service)
        {
            _service = service;
        }

        /// <summary>
        /// Retrieves all blockchain networks.
        /// </summary>
        /// <returns>A list of networks.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NetworkResponseModel>>> GetAll()
        {
            var networks = await _service.GetAllAsync();
            return Ok(networks);
        }

        /// <summary>
        /// Retrieves a blockchain network by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the network.</param>
        /// <returns>The network matching the identifier.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<NetworkResponseModel>> GetById(Guid id)
        {
            var network = await _service.GetByIdAsync(id);
            if (network == null)
                return NotFound();

            return Ok(network);
        }

        /// <summary>
        /// Creates a new blockchain network.
        /// </summary>
        /// <param name="request">The network details.</param>
        /// <returns>The newly created network.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<NetworkResponseModel>> Create([FromBody] NetworkRequestModel request)
        {
            var created = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Updates an existing blockchain network.
        /// </summary>
        /// <param name="id">The identifier of the network to update.</param>
        /// <param name="request">The updated network details.</param>
        /// <returns>No content.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] NetworkUpdateModel request)
        {
            await _service.UpdateAsync(request);
            return Ok();
        }

        /// <summary>
        /// Deletes a blockchain network.
        /// </summary>
        /// <param name="id">The identifier of the network to delete.</param>
        /// <returns>No content.</returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return Ok();
        }

        /// <summary>
        /// Uploads a icon for a specific network.
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
