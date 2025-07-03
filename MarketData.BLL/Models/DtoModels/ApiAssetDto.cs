namespace MarketData.BLL.Models.DtoModels;

public class ApiAssetDto
{
    public Guid Id { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public string Kind { get; set; } = string.Empty;
    public string? Exchange { get; set; }
    public string? Description { get; set; }
    public decimal TickSize { get; set; }
    public string? Currency { get; set; }
    public AssetMappingDto Mappings { get; set; } = new();
    public object? Profile { get; set; }
}