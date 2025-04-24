namespace TransactionCore.Data.Entity
{
    public class Promo
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = null!;
        public decimal? DiscountPercent { get; set; }
        public int? BonusDays { get; set; } 
        public DateTime? ExpirationDate { get; set; }
        public bool IsActive { get; set; }
        public int? MaxUsageCount { get; set; }
        public int UsedCount { get; set; }

        public ICollection<PromoUsage> Usages { get; set; }

    }
}
