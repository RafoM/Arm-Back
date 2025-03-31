namespace TransactionCore.Data.Entity
{
    public class UserFinance
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public decimal Balance { get; set; }
        public Guid? ExpectedPaymentId { get; set; }
        public Guid? AmountWalletId { get; set; }
    }
}
