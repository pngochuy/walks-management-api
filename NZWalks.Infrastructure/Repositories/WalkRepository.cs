using Microsoft.EntityFrameworkCore;
using NZWalks.Core.Domain.Entities;
using NZWalks.Core.Domain.RepositoryInterface;
using NZWalks.Infrastructure.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NZWalks.Infrastructure.Repositories
{
    public class WalkRepository : IWalkRepository
    {
        private readonly NZWalksDbContext dbContext;

        public WalkRepository(NZWalksDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task Add(Walk walk)
        {
            await dbContext.Walks.AddAsync(walk);
            await dbContext.SaveChangesAsync();
        }

        public async Task Delete(Walk walk)
        {
            dbContext.Walks.Remove(walk);
            await dbContext.SaveChangesAsync();
        }

        public async Task<List<Walk>?> GetAll(
            string? filterOn = null, 
            string? filterQuery = null, 
            string? sortBy = null, 
            string orderBy = "asc", 
            int pageNumber = 1, int pageSize = 5
            )
        {
            var walks = dbContext.Walks.Include("Difficulty").Include("Region").AsQueryable();

            // Filtering
            if (string.IsNullOrWhiteSpace(filterOn) == false &&
                string.IsNullOrEmpty(filterQuery) == false)
            {
                if (filterOn.Trim().Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    // Contains sẽ tìm chuỗi con (substring) mà không quan tâm đến chữ viết hoa hay viết thường.
                    walks = walks.Where(x => x.Name.Contains(filterQuery.Trim()));
                }
            }

            // Sorting
            if (string.IsNullOrEmpty(sortBy) == false)
            {
                if (sortBy.Trim().Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    walks = orderBy.Equals("asc") ? walks.OrderBy(x => x.Name) : walks.OrderByDescending(x => x.Name);
                }
                else if (sortBy.Trim().Equals("LengthInKm", StringComparison.OrdinalIgnoreCase))
                {
                    walks = orderBy.Equals("asc") ? walks.OrderBy(x => x.LengthInKm) : walks.OrderByDescending(x => x.LengthInKm);
                }
            }

            // Pagination
            var skipResults = (pageNumber - 1) * pageSize;

            return await walks.Skip(skipResults).Take(pageSize).ToListAsync();
        }

        public async Task<Walk?> GetById(Guid id)
        {
            return await dbContext.Walks.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Walk?> Update(Guid id, Walk walk)
        {
            var existingWalk = await dbContext.Walks.FirstOrDefaultAsync(x => x.Id == id);

            if (existingWalk == null) return null;

            existingWalk.Name = walk.Name;
            existingWalk.Description = walk.Description;
            existingWalk.LengthInKm = walk.LengthInKm;
            existingWalk.WalkImageUrl = walk.WalkImageUrl;
            existingWalk.DifficultyId = walk.DifficultyId;
            existingWalk.RegionId = walk.RegionId;

            await dbContext.SaveChangesAsync();

            return existingWalk;
        }
    }
}
