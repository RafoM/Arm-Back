using System.ComponentModel.DataAnnotations;

namespace ContentService.Data.Entity
{
    public class CaseTag
    {
        [Key]
        public Guid Id { get; set; }
        public ICollection<Case> Case { get; set; } = new List<Case>();

    }
}
