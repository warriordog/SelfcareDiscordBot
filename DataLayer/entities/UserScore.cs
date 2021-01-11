using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SelfcareBot.DataLayer.entities
{
    public class UserScore
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [AllowNull]
        public virtual KnownUser KnownUser { get; set; }

        [Required]
        [Column(TypeName = "text")]
        public string Category { get; set; }

        [Required]
        public int Score { get; set; }
    }
}