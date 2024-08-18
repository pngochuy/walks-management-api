using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.CustomActionFilters;
using NZWalks.Core.Domain.Entities;
using NZWalks.Core.Domain.RepositoryInterface;
using NZWalks.Core.DTOs;
using NZWalks.Core.ServicesInterface;
using NZWalks.Infrastructure.DataContext;
using System.Collections.Generic;
using System.Text.Json;

namespace NZWalks.API.Controllers
{
    // https://localhost:port_number/api/controller_name
    // https://localhost:7061/api/regions
    [Route("api/[controller]")]
    [ApiController] 
    // [Authorize] // đặt Authorize ở đây là toàn bộ request đến Controller này sẽ
                // đc access khi user đã authenticated, nếu ko authenticated (login) sẽ ra lỗi 401 - Unauthorized
    public class RegionsController : ControllerBase
    {
        private readonly NZWalksDbContext dbContext;
        private readonly IRegionService regionService;
        private readonly IMapper mapper;
        private readonly ILogger<RegionsController> logger;

        public RegionsController(
            NZWalksDbContext dbContext,
            IRegionService regionService, 
            IMapper mapper,
            ILogger<RegionsController> logger 
            )
        {
            this.dbContext = dbContext;
            this.regionService = regionService;
            this.mapper = mapper;
            this.logger = logger;
        }

        // Get all regions
        // GET: https://localhost:7061/api/regions
        [HttpGet]
        // [Authorize(Roles = "admin,user")]

        public async Task<IActionResult> GetAll()
        {
            logger.LogInformation("GetAll method (/api/regions) was invoked!"); // LogError(), LogDebug(),... 

            var regionsDomain = await regionService.GetAllAsync();

            //logger.LogInformation($"Finished GetAll method with data: {JsonSerializer.Serialize(regionsDomain)}"); 

            // Map<Destination>(Resource)
            return Ok(mapper.Map<List<RegionDto>>(regionsDomain));
        }

        // Get Region by Id
        // GET: https://localhost:7061/api/regions/{id}
        [HttpGet]
        [Route("{id:Guid}")] // id phải đúng với tên id truyền vào ở dưới
        //[Authorize(Roles = "Reader,Writer")]
        [Authorize(Roles = "admin,user")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var regionDomain = await regionService.GetByIdAsync(id);

            if (regionDomain == null) return NotFound();

            // Map<Destination>(Resource)
            return Ok(mapper.Map<RegionDto>(regionDomain));
        }


        // Post to create a new region
        // POST: https://localhost:7061/api/regions
        [HttpPost]
        [ValidateModel] // Validate DTO trước khi vô Controller (do đó phải tạo 
                        // class custom ValidateModelAttribute kế thừa abstract class ActionFilterAttribute)
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([FromBody] AddRegionRequestDto addRegionRequestDto)
        {
            // Map DTO to Domain Model
            var regionDomainModel = mapper.Map<Region>(addRegionRequestDto);
            
            // Use Domain Model to create Region
            await regionService.CreateAsync(regionDomainModel);  

            // Map Domain Modal back to DTO
            var regionDto = mapper.Map<RegionDto>(regionDomainModel);

            return CreatedAtAction(nameof(GetById), new {id = regionDto.Id}, regionDto);
        }

        // Update region
        // PUT: https://localhost:7061/api/regions/{id}
        [HttpPut]
        [Route("{id:Guid}")]
        [ValidateModel] // Validate DTO trước khi vô Controller (do đó phải tạo 
                        // class custom ValidateModelAttribute kế thừa abstract class ActionFilterAttribute)
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRegionRequestDto updateRegionRequestDto)
        {
            // Map DTO to Domain Model
            var regionDomainModel = mapper.Map<Region>(updateRegionRequestDto);

            regionDomainModel = await regionService.UpdateAsync(id, regionDomainModel);

            if (regionDomainModel == null) return NotFound();
          
            return Ok(mapper.Map<RegionDto>(regionDomainModel));
        }


        // Delete region
        // DELETE: https://localhost:7061/api/regions/{id}
        [HttpDelete]
        [Route("{id:Guid}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var regionDomainModel = await regionService.DeleteAsync(id);

            if (regionDomainModel == null) return NotFound();

            return NoContent();
        }

    }
}
