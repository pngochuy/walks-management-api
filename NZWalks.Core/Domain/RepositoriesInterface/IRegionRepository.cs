using NZWalks.Core.Domain.Entities;

namespace NZWalks.Core.Domain.RepositoryInterface
{
    public interface IRegionRepository
    {
        Task Add(Region region);
        Task<Region?> GetById(Guid id);
        Task Delete(Region region);
        Task<List<Region>?> GetAll(
            string? filterOn = null,
            string? filterQuery = null,
            string? sortBy = null,
            string orderBy = "asc",
            int pageNumber = 1,
            int pageSize = 5);
        Task<Region?> Update(Guid id, Region region);

    }
}
