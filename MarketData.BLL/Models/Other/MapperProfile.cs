using AutoMapper;
using MarketData.BLL.Models.DtoModels;
using MarketData.DAL.Entities;

namespace MarketData.BLL.Models.Other;

public class MapperProfile: Profile
{
    public MapperProfile()
    {

        CreateMap<AssetDto, Asset>()
            .ForMember(dest => dest.Mappings, opt => opt.MapFrom((src, dest, destMember, context) =>
            {
                var mappings = new List<AssetMapping>();
                foreach (var kvp in src.Mappings)
                {
                    var mappingDto = kvp.Value;
                    var mapping = context.Mapper.Map<AssetMapping>(mappingDto);
                    mapping.Provider = kvp.Key;
                    mappings.Add(mapping);
                }
                return mappings;
            }));

        CreateMap<AssetMappingDto, AssetMapping>()
            .ForMember(dest => dest.ProviderSymbol, opt => opt.MapFrom(src => src.Symbol))
            .ForMember(dest => dest.Provider, opt => opt.Ignore())
            .ReverseMap()
            .ForMember(dest => dest.Symbol, opt => opt.MapFrom(src => src.ProviderSymbol));


    }
}