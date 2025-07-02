using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketData.DAL.Entities;

public class AssetMapping
{
    [Key]
    public Guid Id { get; set; }

    [Required] 
    [MaxLength(30)] 
    public string Provider { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string ProviderSymbol { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Exchange { get; set; } = string.Empty;


    public Guid AssetId { get; set; }

    [ForeignKey(nameof(AssetId))]
    public Asset? Asset { get; set; }
}