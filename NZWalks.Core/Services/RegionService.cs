using Microsoft.EntityFrameworkCore;
using NZWalks.Core.Domain.RepositoryInterface;
using NZWalks.Core.Domain.Entities;
using NZWalks.Core.ServicesInterface;

namespace NZWalks.Core.Services
{
    public class RegionService : IRegionService
    {
        private readonly IRegionRepository regionRepository;

        public RegionService(IRegionRepository regionRepository)
        {
            this.regionRepository = regionRepository;
        }

        public async Task<Region> CreateAsync(Region region)
        {
            await regionRepository.Add(region);
            return region;
        }

        public async Task<List<Region>?> GetAllAsync(
            string? filterOn = null,
            string? filterQuery = null,
            string? sortBy = null,
            string orderBy = "asc",
            int pageNumber = 1,
            int pageSize = 5
            )
        {
            return await regionRepository.GetAll(filterOn, filterQuery, sortBy, orderBy, pageNumber, pageSize);
        }

        public async Task<Region?> GetByIdAsync(Guid id)
        {
            return await regionRepository.GetById(id);
        }

        public async Task<Region?> UpdateAsync(Guid id, Region region)
        {
            var regionUpdated = await regionRepository.Update(id, region);

            return regionUpdated;
        }

        public async Task<Region?> DeleteAsync(Guid id)
        {
            var existingRegion = await regionRepository.GetById(id);

            if (existingRegion == null) return null;

            await regionRepository.Delete(existingRegion);

            return existingRegion;
        }
    }
}
