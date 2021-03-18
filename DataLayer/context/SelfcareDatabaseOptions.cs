using System.ComponentModel.DataAnnotations;

namespace SelfcareBot.DataLayer.context
{
    public class SelfcareDatabaseOptions
    {
        [Required] public string ConnectionString { get; init; }
    }
}