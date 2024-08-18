using NZWalks.Core.DTOs;

namespace NZWalks.Core.DTOs
{
    public class WalkDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double LengthInKm { get; set; }
        public string? WalkImageUrl { get; set; }

        // bởi vì đã có Include khi trả về List trong hàm GetAllSync của WalkRepository và nó trả về các thuộc tính
        // Entity trong Walk lun rồi nên ko cần attribute là Guid nữa mà là một ClassDTO
        //public Guid DifficultyId { get; set; }
        //public Guid RegionId { get; set; }

        public DifficultyDto Difficulty { get; set; }
        public RegionDto Region { get; set; }
    }
}
