namespace NZWalks.Core.Domain.Entities
{
    public class Region
    {
        public Guid Id { get; set; } // Guid giống như uuid, tự tạo
        public string Code { get; set; }
        public string Name { get; set; }
        public string? RegionImageUrl { get; set; } // accept nullable (keyword: ?)
    }
}
