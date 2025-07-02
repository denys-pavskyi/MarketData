using MarketData.DAL.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MarketData.BLL.Models.DtoModels;

public class AssetMappingDto
{
    public Guid Id { get; set; }
    public string Provider { get; set; } = string.Empty;

    public string ProviderSymbol { get; set; } = string.Empty;

    public string Exchange { get; set; } = string.Empty;
    public Guid AssetId { get; set; }

}