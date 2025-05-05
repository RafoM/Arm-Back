namespace TransactionCore.Data.Entity
{
    public class RemainderInfo
    {
        public Guid Id { get; set; }
        public Guid UserInfoId { get; set; }
        public decimal Amount { get; set; }
        public Guid WalletId { get; set; }
        public Wallet Wallet { get; set; }
    }
}
