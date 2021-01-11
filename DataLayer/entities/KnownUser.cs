using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SelfcareBot.DataLayer.entities
{
    public class KnownUser
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        public ulong DiscordId { get; set; }

        [Required]
        [Column(TypeName = "text")]
        public string Username { get; set; }

        [Required]
        [Column(TypeName = "text")]
        public string Discriminator { get; set; }

        [AllowNull]
        public virtual ICollection<UserScore> UserScores { get; set; }
    }
}