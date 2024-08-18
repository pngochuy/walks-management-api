using System.ComponentModel.DataAnnotations;

namespace NZWalks.Core.DTOs
{
    public class UpdateRegionRequestDto
    {
        // nếu muốn cần cái thuộc tính nào đ thì copy bên Entity kia qua

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
