using AutoMapper;
using MarketData.BLL.Models.DtoModels;
using MarketData.DAL.Entities;

namespace MarketData.BLL.Models.Other;

public class MapperProfile: Profile
{
    public MapperProfile()
    {

        CreateMap<AssetDto, Asset>()
            .ReverseMap();

        CreateMap<AssetMappingDto, AssetMapping>()
            .ReverseMap();



    }
}