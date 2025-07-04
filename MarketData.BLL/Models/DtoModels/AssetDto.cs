using MarketData.DAL.Entities;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MarketData.BLL.Models.DtoModels;

public class AssetDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = string.Empty;

    [JsonPropertyName("kind")]
    public string Kind { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("exchange")]
    public string Exchange { get; set; } = string.Empty;


    [JsonPropertyName("tickSize")]
    public decimal TickSize { get; set; }

    [JsonPropertyName("currency")]
    public string Currency { get; set; } = string.Empty;

    [JsonPropertyName("mappings")]
    [JsonConverter(typeof(AssetMappingConverter))]
    public List<AssetMappingDto> Mappings { get; set; } = new();

}