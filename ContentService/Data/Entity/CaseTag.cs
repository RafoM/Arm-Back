using System.ComponentModel.DataAnnotations;

namespace ContentService.Data.Entity
{
    public class CaseTag
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Tag { get; set; } = null!;
        public ICollection<Case> Case { get; set; } = new List<Case>();

    }
}
