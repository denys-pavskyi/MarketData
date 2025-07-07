using MarketData.BLL.Models.Responses;
using System.Text.Json;

namespace MarketData.BLL.Helpers;

public static class PriceParserHelper
{
    public static PriceResponseDto? ParsePriceFromWebSocket(JsonElement root)
    {
        if (!root.TryGetProperty("provider", out var providerElement))
            return null;

        var provider = providerElement.GetString();
        return provider switch
        {
            "simulation" => ParseSimulation(root),
            "oanda" => ParseOanda(root),
            "dxfeed" => ParseDxFeed(root),
            _ => null
        };
    }
    private static PriceResponseDto? ParseSimulation(JsonElement root)
    {
        if (!root.TryGetProperty("instrumentId", out var id) ||
            !root.TryGetProperty("bid", out var bid))
            return null;

        return new PriceResponseDto
        {
            InstrumentId = id.GetString(),
            Provider = "simulation",
            Price = bid.GetProperty("price").GetDecimal(),
            UpdateTime = bid.GetProperty("timestamp").GetDateTime()
        };
    }

    private static PriceResponseDto? ParseOanda(JsonElement root)
    {
        if (!root.TryGetProperty("instrumentId", out var id) ||
            !root.TryGetProperty("quote", out var quote) ||
            !quote.TryGetProperty("last", out var last))
            return null;

        return new PriceResponseDto
        {
            InstrumentId = id.GetString(),
            Provider = "oanda",
            Price = last.GetProperty("price").GetDecimal(),
            UpdateTime = last.GetProperty("timestamp").GetDateTime()
        };
    }

    private static PriceResponseDto? ParseDxFeed(JsonElement root)
    {
        if (!root.TryGetProperty("instrumentId", out var id) ||
            !root.TryGetProperty("quote", out var quote) ||
            !quote.TryGetProperty("last", out var last))
            return null;

        return new PriceResponseDto
        {
            InstrumentId = id.GetString(),
            Provider = "dxfeed",
            Price = last.GetProperty("price").GetDecimal(),
            UpdateTime = last.GetProperty("timestamp").GetDateTime()
        };
    }
}