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
        /// The address sending the tokens.
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// The address receiving the tokens.
        /// </summary>
        public string To { get; set; }

        /// <summary>
        /// The value transferred in smallest units.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The token symbol (e.g. USDT, USDC, TRONPAD).
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// The contract address of the TRC20 token.
        /// </summary>
        public string ContractAddress { get; set; }

        /// <summary>
        /// The number of decimals the token uses (e.g. 6 for USDT).
        /// </summary>
        public int Decimals { get; set; }

        /// <summary>
        /// The timestamp of the event (in milliseconds since epoch).
        /// </summary>
        public long Timestamp { get; set; }
    }
}
