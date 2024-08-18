namespace NZWalks.Core.DTOs
{
    public class RegionDto
    {
        // nếu muốn cần cái thuộc tính nào thì copy bên Entity kia qua
        public Guid Id { get; set; } // Guid giống như uuid
        public string Code { get; set; }
        public string Name { get; set; }
        public string? RegionImageUrl { get; set; } // accept nullable (keyword: ?)
    }
}
