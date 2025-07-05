using System.Text.Json.Serialization;

namespace MarketData.BLL.Models.Other;

public class WebSocketPriceMessage
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("instrumentId")]
    public string InstrumentId { get; set; } = string.Empty;

    [JsonPropertyName("provider")]
    public string Provider { get; set; } = string.Empty;

    [JsonPropertyName("bid")]
    public decimal? Bid { get; set; }

    [JsonPropertyName("ask")]
    public decimal? Ask { get; set; }

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }
}