using System.Text.Json.Serialization;

namespace MarketData.BLL.Models.Responses;

public class PagingDto
{
    [JsonPropertyName("page")]
    public int Page { get; set; }

    [JsonPropertyName("pages")]
    public int Pages { get; set; }

    [JsonPropertyName("items")]
    public int Items { get; set; }
}