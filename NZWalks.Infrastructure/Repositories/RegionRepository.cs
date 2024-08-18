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
    public class RegionRepository : IRegionRepository // internal
    {
        private readonly NZWalksDbContext dbContext;

        public RegionRepository(NZWalksDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<Region>?> GetAll(
            string? filterOn = null, 
            string? filterQuery = null, 
            string? sortBy = null, 
            string orderBy = "asc", 
            int pageNumber = 1, int pageSize = 5
            )
        {
            var regions = dbContext.Regions.AsQueryable();

            // Filtering
            if (string.IsNullOrWhiteSpace(filterOn) == false &&
                string.IsNullOrEmpty(filterQuery) == false)
            {
                if (filterOn.Trim().Equals("Code", StringComparison.OrdinalIgnoreCase))
                {
                    // Contains sẽ tìm chuỗi con (substring) mà không quan tâm đến chữ viết hoa hay viết thường.
                    regions = regions.Where(x => x.Code.Equals(filterQuery.Trim()));
                } 
                else if (filterOn.Trim().Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    regions = regions.Where(x => x.Name.Contains(filterQuery.Trim()));
                }
            }

            // Sorting
            if (string.IsNullOrEmpty(sortBy) == false)
            {
                if (sortBy.Trim().Equals("Code", StringComparison.OrdinalIgnoreCase))
                {
                    regions = orderBy.Equals("asc") ? regions.OrderBy(x => x.Code) : regions.OrderByDescending(x => x.Code);
                }
                else if (sortBy.Trim().Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    regions = orderBy.Equals("asc") ? regions.OrderBy(x => x.Name) : regions.OrderByDescending(x => x.Name);
                }
            }

            // Pagination
            var skipResults = (pageNumber - 1) * pageSize;

            return await regions.Skip(skipResults).Take(pageSize).ToListAsync();
        }

        public async Task<Region?> GetById(Guid id)
        {
            return await dbContext.Regions.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task Add(Region region)
        {
            await dbContext.AddAsync(region);
            await dbContext.SaveChangesAsync();
        }
        public async Task<Region?> Update(Guid id, Region region)
        {
            // check if region exists
            var existingRegion = await dbContext.Regions.FirstOrDefaultAsync(x => x.Id == id);

            if (existingRegion == null) return null;

            existingRegion.Code = region.Code;
            existingRegion.Name = region.Name;
            existingRegion.RegionImageUrl = region.RegionImageUrl;

            // vì regionDomainModel được quản lí và theo dõi bởi DBContext sau khi `FirstOrDefault` nên chỉ cần
            // SaveChanges mà không dùng Update
            await dbContext.SaveChangesAsync();

            return existingRegion;
        }

        public async Task Delete(Region region)
        {
            dbContext.Regions.Remove(region); // Remove là Synchoronus
            await dbContext.SaveChangesAsync(); // nếu ko có sẽ ko xóa bên phía Database
        }
    }
}
