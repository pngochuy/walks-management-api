using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NZWalks.Core.Domain.Entities;
using NZWalks.Core.Domain.RepositoryInterface;
using NZWalks.Core.ServicesInterface;

namespace NZWalks.Core.Services
{
    public class WalkService : IWalkService
    {
        private readonly IWalkRepository walkRepository;

        //private readonly NZWalksDbContext dbContext;

        public WalkService(IWalkRepository walkRepository)
        {
            this.walkRepository = walkRepository;
        }

        public async Task<Walk?> CreateAsync(Walk walk)
        {
            await walkRepository.Add(walk);
            return await GetByIdAsync(walk.Id);
        }

        public async Task<Walk?> DeleteAsync(Guid id)
        {
            var existingWalk = await walkRepository.GetById(id);

            if (existingWalk == null) return null;

            await walkRepository.Delete(existingWalk);

            return existingWalk;
        }

        public async Task<List<Walk>?> GetAllAsync(
            string? filterOn = null,
            string? filterQuery = null,
            string? sortBy = null,
            string orderBy = "asc",
            int pageNumber = 1,
            int pageSize = 5
            )
        {
            return await walkRepository.GetAll(filterOn, filterQuery, sortBy, orderBy, pageNumber, pageSize);
        }

        public async Task<Walk?> GetByIdAsync(Guid id)
        {
            return await walkRepository.GetById(id);
        }

        public async Task<Walk?> UpdateAsync(Guid id, Walk walk)
        {
            var walkUpdated = await walkRepository.Update(id, walk);

            return walkUpdated;
        }
    }
}
