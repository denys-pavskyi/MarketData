using System.Text.Json.Serialization;

namespace MarketData.BLL.Models.Responses;

public class PagedResponseDto<T>
{
    [JsonPropertyName("paging")]
    public PagingDto Paging { get; set; } = null!;

    [JsonPropertyName("data")]
    public List<T> Data { get; set; } = new();
}