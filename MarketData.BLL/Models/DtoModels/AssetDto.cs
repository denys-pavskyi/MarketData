using MarketData.DAL.Entities;
using System.ComponentModel.DataAnnotations;

namespace MarketData.BLL.Models.DtoModels;

public class AssetDto
{
    public Guid Id { get; set; }

    public string Symbol { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Kind { get; set; } = string.Empty;

    public string Currency { get; set; } = string.Empty;

    public string BaseCurrency { get; set; } = string.Empty;

    public decimal TickSize { get; set; }

    public List<Guid> MappingIds { get; set; } = new();
}