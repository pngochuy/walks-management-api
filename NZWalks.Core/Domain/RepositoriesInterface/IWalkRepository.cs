using Microsoft.AspNetCore.Mvc;
using NZWalks.Core.Domain.Entities;

namespace NZWalks.Core.Domain.RepositoryInterface
{
    public interface IWalkRepository
    {
        Task Add(Walk walk);
        Task<Walk?> GetById(Guid id);
        Task Delete(Walk walk);
        Task<List<Walk>?> GetAll(
            string? filterOn = null,
            string? filterQuery = null,
            string? sortBy = null,
            string orderBy = "asc",
            int pageNumber = 1,
            int pageSize = 5);
        Task<Walk?> Update(Guid id, Walk walk);

    }
}
