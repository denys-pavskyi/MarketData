using AutoMapper;
using MarketData.BLL.Models.DtoModels;
using MarketData.DAL.Entities;

namespace MarketData.BLL.Models.Other;

public class MapperProfile: Profile
{
    public MapperProfile()
    {

        CreateMap<Asset, AssetDto>()
            .ForMember(dest => dest.MappingIds, opt => opt.MapFrom(src => src.Mappings.Select(t => t.Id)))
            .ReverseMap();


        CreateMap<AssetMapping, AssetMappingDto>()
            .ReverseMap();


    }
}