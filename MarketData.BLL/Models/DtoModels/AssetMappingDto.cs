using MarketData.DAL.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MarketData.BLL.Models.DtoModels;

public class AssetMappingDto
{
    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = string.Empty;

    [JsonPropertyName("exchange")]
    public string? Exchange { get; set; }

    [JsonPropertyName("defaultOrderSize")]
    public int? DefaultOrderSize { get; set; }

    [JsonPropertyName("maxOrderSize")]
    public int? MaxOrderSize { get; set; }

}