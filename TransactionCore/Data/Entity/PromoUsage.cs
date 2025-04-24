namespace TransactionCore.Data.Entity
{
    public class PromoUsage
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid PromoId { get; set; }
        public DateTime UsedAt { get; set; }

        public Promo Promo { get; set; }
    }
}
