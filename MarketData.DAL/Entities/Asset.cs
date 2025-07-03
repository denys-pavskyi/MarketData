using System.ComponentModel.DataAnnotations;

namespace MarketData.DAL.Entities;

public class Asset
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Symbol { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Kind { get; set; } = string.Empty;

    [MaxLength(10)]
    public string Currency { get; set; } = string.Empty;

    public decimal TickSize { get; set; }

    public ICollection<AssetMapping> Mappings { get; set; } = new List<AssetMapping>();
}