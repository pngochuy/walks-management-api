using System.ComponentModel.DataAnnotations;

namespace NZWalks.Core.DTOs
{
    public class AddRegionRequestDto
    {
        // nếu muốn cần cái thuộc tính nào để create thì copy bên Entity kia qua đây

        [Required]
        [MinLength(1, ErrorMessage = "Code has to be minium of 3 characters")]
        [MaxLength(5, ErrorMessage = "Code has to be maxium of 5 characters")]
        public string Code { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "Code has to be maxium of 100 characters")]
        public string Name { get; set; }

        public string? RegionImageUrl { get; set; }
    }
}
