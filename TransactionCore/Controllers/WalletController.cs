using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransactionCore.Models.RequestModels;
using TransactionCore.Models.ResponseModels;
using TransactionCore.Services.Interface;

namespace TransactionCore.Controllers
{
    [Authorize]
    public class WalletController : BaseController
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        /// <summary>
        /// Retrieves all wallets.
        /// </summary>
        /// <returns>A list of wallet response models.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WalletResponseModel>>> GetAll()
        {
            var wallets = await _walletService.GetAllAsync();
            return Ok(wallets);
        }

        /// <summary>
        /// Retrieves a wallet by its identifier.
        /// </summary>
        /// <param name="id">The wallet identifier.</param>
        /// <returns>The wallet response model.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<WalletResponseModel>> GetById(Guid id)
        {
            var wallet = await _walletService.GetByIdAsync(id);
            if (wallet == null)
            {
                return NotFound();
            }
            return Ok(wallet);
        }

        /// <summary>
        /// Creates a new wallet.
        /// </summary>
        /// <param name="model">The wallet request model.</param>
        /// <returns>The newly created wallet response model.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<WalletResponseModel>> Create([FromBody] WalletRequestModel model)
        {
            var createdWallet = await _walletService.CreateAsync(model);
            return CreatedAtAction(nameof(GetById), new { id = createdWallet.Id }, createdWallet);
        }

        /// <summary>
        /// Updates an existing wallet.
        /// </summary>
        /// <param name="id">The wallet identifier.</param>
        /// <param name="model">The wallet request model.</param>
        /// <returns>No content.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] WalletUpdateModel model)
        {
            await _walletService.UpdateAsync(model);
            return Ok();
        }

        /// <summary>
        /// Deletes a wallet.
        /// </summary>
        /// <param name="id">The wallet identifier.</param>
        /// <returns>No content.</returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _walletService.DeleteAsync(id);
            return NoContent();
        }
    }
}
