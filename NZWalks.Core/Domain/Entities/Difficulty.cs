using System.ComponentModel.DataAnnotations;

namespace NZWalks.Core.Domain.Entities
{
    public class Difficulty
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
