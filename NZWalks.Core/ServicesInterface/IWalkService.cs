using Microsoft.AspNetCore.Mvc;
using NZWalks.Core.Domain.Entities;

namespace NZWalks.Core.ServicesInterface
{
    public interface IWalkService
    {
        Task<List<Walk>> GetAllAsync(
            string? filterOn = null,
            string? filterQuery = null,
            string? sortBy = null,
            string orderBy = "asc",
            int pageNumber = 1,
            int pageSize = 5
            );
        Task<Walk?> GetByIdAsync(Guid id); // ?: có thể chứa null
        Task<Walk?> CreateAsync(Walk walk);
        Task<Walk?> UpdateAsync(Guid id, Walk walk);
        Task<Walk?> DeleteAsync(Guid id);

    }
}
