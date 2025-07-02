using System.ComponentModel.DataAnnotations;

namespace MarketData.BLL.Models.DtoModels;

public class AssetSyncMetadataDto
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime LastSyncedAt { get; set; }
}