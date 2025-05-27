using System.ComponentModel.DataAnnotations;

namespace ContentService.Data.Entity
{
    public class Case
    {
        [Key]
        public Guid Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public ICollection<CaseTag> CaseTags { get; set; } = new List<CaseTag>();
    }
}
