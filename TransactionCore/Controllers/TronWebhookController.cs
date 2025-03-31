using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using TransactionCore.Models.RequestModels;
using TransactionCore.Services.Interface;

namespace TransactionCore.Controllers
{
    public class TronWebhookController : BaseController
    {
        private readonly ITronWebhookService _tronWebhookService;
        private readonly string _webhookSecret;

        public TronWebhookController(ITronWebhookService tronWebhookService, IConfiguration configuration)
        {
            _tronWebhookService = tronWebhookService;
            // Assume the secret is stored in configuration (e.g., appsettings.json or environment variables)
            _webhookSecret = configuration["WebhookSecret"] ?? throw new Exception("Webhook secret not configured.");
        }

        /// <summary>
        /// Receives TRC20 USDT transfer events from QuickNode via webhook.
        /// Validates the HMAC signature before processing.
        /// </summary>
        /// <param name="payload">The webhook payload.</param>
        /// <returns>An IActionResult indicating the processing result.</returns>
        [HttpPost]
        public async Task<IActionResult> ReceiveWebhook([FromBody] TronWebhookPayload payload)
        {
           
            if (!Request.Headers.TryGetValue("X-Quicknode-Signature", out var signatureHeader))
            {
                return Unauthorized("Missing signature header.");
            }

            Request.EnableBuffering();
            string requestBody;
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8, leaveOpen: true))
            {
                requestBody = await reader.ReadToEndAsync();
                Request.Body.Position = 0; 
            }

            if (!ValidateSignature(requestBody, signatureHeader))
            {
                return Unauthorized("Invalid signature.");
            }

            try
            {
                await _tronWebhookService.ProcessWebhookAsync(payload);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Validates the HMAC signature of the request body using the configured secret.
        /// </summary>
        /// <param name="requestBody">The raw request body.</param>
        /// <param name="signatureHeader">The signature value from the header.</param>
        /// <returns>True if the signature is valid; otherwise, false.</returns>
        private bool ValidateSignature(string requestBody, string signatureHeader)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_webhookSecret)))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(requestBody));
                var computedSignature = BitConverter.ToString(computedHash).Replace("-", "").ToLowerInvariant();
                return computedSignature == signatureHeader.ToLowerInvariant();
            }
        }
    }
}
