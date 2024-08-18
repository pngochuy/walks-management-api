using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.CustomActionFilters;
using NZWalks.Core.Domain.Entities;
using NZWalks.Core.DTOs;
using NZWalks.Core.ServicesInterface;
using System.Linq.Expressions;

namespace NZWalks.API.Controllers
{
    // https://localhost:port_number/api/controller_name
    // https://localhost:7061/api/walks
    [Route("api/[controller]")]
    [ApiController]
    public class WalksController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IWalkService walkService;

        public WalksController(IMapper mapper, IWalkService walkService)
        {
            this.mapper = mapper;
            this.walkService = walkService;
        }

        // Get all Walk
        // POST: /api/walks?filterOn=Name&filterQuery=Track
        [HttpGet]
        [Authorize(Roles = "admin,user")]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? filterOn,
            [FromQuery] string? filterQuery,
            [FromQuery] string? sortBy,
            [FromQuery] string? orderBy,
            [FromQuery] int? pageNumber,
            [FromQuery] int? pageSize
            //[FromQuery] int pageNumber = 1, // gắn default value sẽ tự động hiện trên URL
            //[FromQuery] int pageSize = 5 
        )
        {
            List<Walk> walkListDomain = await walkService.GetAllAsync(
                filterOn,
                filterQuery,
                sortBy,
                orderBy ?? "asc", // if sortBy != null, it'll sorted by "asc" by default with sortBy (if we don't select sort option).
                                  // Otherwise, it not sorted!
                pageNumber ?? 1,
                pageSize ?? 5
            //pageNumber, pageSize
            );

            // Map<Destination>(Resource)
            return Ok(mapper.Map<List<WalkDto>>(walkListDomain));

            // expression1 ?? expression2
            // Nếu expression1 không phải là null, kết quả sẽ là expression1.
            // Nếu expression1 là null, kết quả sẽ là expression2
        }

        // Get Walk by Id
        // GET: https://localhost:7061/api/walks/{id}
        [HttpGet]
        [Route("{id:Guid}")]
        [Authorize(Roles = "admin,user")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var walkDomain = await walkService.GetByIdAsync(id);

            if (walkDomain == null) return NotFound();

            return Ok(mapper.Map<WalkDto>(walkDomain));
        }

        // Create Walk
        // POST: https://localhost:7061/api/walks
        [HttpPost]
        [ValidateModel]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([FromBody] AddWalkRequestDto addWalkRequestDto)
        {
            var walkDomain = mapper.Map<Walk>(addWalkRequestDto);

            walkDomain = await walkService.CreateAsync(walkDomain);

            var walkDto = mapper.Map<WalkDto>(walkDomain);

            return CreatedAtAction(nameof(GetById), new { id = walkDto.Id }, walkDto);
        }

        // Update Walk
        // PUT: https://localhost:7061/api/walks/{id}
        [HttpPut]
        [Route("{id:Guid}")]
        [ValidateModel]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateWalkRequestDto updateWalkRequestDto)
        {
            var walkDomain = mapper.Map<Walk>(updateWalkRequestDto);

            walkDomain = await walkService.UpdateAsync(id, walkDomain);

            if (walkDomain == null) return NotFound();

            return Ok(mapper.Map<WalkDto>(walkDomain));
        }

        // Delete Walk
        // DELETE: https://localhost:7061/api/walks/{id}
        [HttpDelete]
        [Route("{id:Guid}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var walkDomain = await walkService.DeleteAsync(id);

            if (walkDomain == null) return NotFound();

            return NoContent();
        }
    }
}
