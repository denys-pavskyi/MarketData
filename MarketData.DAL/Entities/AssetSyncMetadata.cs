using System.ComponentModel.DataAnnotations;

namespace MarketData.DAL.Entities;

public class AssetSyncMetadata
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public DateTime LastSyncedAt { get; set; }
}