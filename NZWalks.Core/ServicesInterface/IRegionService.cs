using NZWalks.Core.Domain.Entities;

namespace NZWalks.Core.ServicesInterface
{
    public interface IRegionService
    {
        Task<List<Region>?> GetAllAsync(
            string? filterOn = null,
            string? filterQuery = null,
            string? sortBy = null,
            string orderBy = "asc",
            int pageNumber = 1,
            int pageSize = 5
            );
        Task<Region?> GetByIdAsync(Guid id); // ?: có thể chứa null
        Task<Region> CreateAsync(Region region);
        Task<Region?> UpdateAsync(Guid id, Region region);
        Task<Region?> DeleteAsync(Guid id);

    }
}
