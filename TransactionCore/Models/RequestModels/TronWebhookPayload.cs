namespace TransactionCore.Models.RequestModels
{
    public class TronWebhookPayload
    {
        /// <summary>
        /// The type of event (expected to be "TRC20.Transfer").
        /// </summary>
        public string Event { get; set; }

        /// <summary>
        /// The blockchain transaction identifier.
        /// </summary>
        public string TransactionId { get; set; }

        /// <summary>
        /// The block number where the transaction is included.
        /// </summary>
        public long BlockNumber { get; set; }

        /// <summary>
        /// The address sending the USDT.
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// The address receiving the USDT.
        /// </summary>
        public string To { get; set; }

        /// <summary>
        /// The value transferred in smallest units (USDT uses 6 decimals).
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The token symbol (should be "USDT").
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// The timestamp of the event.
        /// </summary>
        public long Timestamp { get; set; }
    }
}
