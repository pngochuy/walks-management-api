using AutoMapper;
using NZWalks.Core.Domain.Entities;
using NZWalks.Core.DTOs;

namespace NZWalks.API.Mappings
{
    public class AutoMapperProfiles:Profile
    {
        public AutoMapperProfiles()
        {
            // map từ Source -> Destination
            CreateMap<Region, RegionDto>().ReverseMap();
            CreateMap<Region, AddRegionRequestDto>().ReverseMap();
            CreateMap<Region, UpdateRegionRequestDto>().ReverseMap();

            CreateMap<Walk, WalkDto>().ReverseMap();
            CreateMap<Walk, AddWalkRequestDto>().ReverseMap();
            CreateMap<Walk, UpdateWalkRequestDto>().ReverseMap();

            CreateMap<Difficulty, DifficultyDto>().ReverseMap();

            /*
             * CreateMap<RegionDto, Region>()
                .ForMember(x => x.Name, opt => opt.MapFrom(x => x.Code)) // map với attribut nào ko tồn tại 
                .ReverseMap(); // map ngược lại đc
             * -------------------
             * CreateMap<RegionDto, Region>(): Tạo ánh xạ giữa RegionDto và Region (có thể ánh xạ List).
                .ForMember(x => x.Name, opt => opt.MapFrom(x => x.Code)): Đây là cấu hình ánh xạ tùy 
                chỉnh. Nó ánh xạ thuộc tính Code của RegionDto sang thuộc tính Name của Region.
                .ReverseMap(): Tự động tạo ánh xạ ngược từ Region sang RegionDto. Điều này có nghĩa là bạn có 
                thể sử dụng ánh xạ này để chuyển đổi cả hai chiều giữa Region và RegionDto.
            */
        }
    }
}
