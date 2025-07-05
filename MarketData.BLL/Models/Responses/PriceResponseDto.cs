namespace MarketData.BLL.Models.Responses;

public class PriceResponseDto
{
    public string InstrumentId { get; set; }
    public string Provider { get; set; }
    public decimal? CurrentBid { get; set; }
    public decimal? CurrentAsk { get; set; }
    public DateTime UpdateTime { get; set; }
    // public List<BarDto> HistoricalBars { get; set; } // todo 
}