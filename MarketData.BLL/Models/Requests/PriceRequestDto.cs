namespace MarketData.BLL.Models.Requests;

public class PriceRequestDto
{
    public string InstrumentId { get; set; }
    public string Provider { get; set; }
    public int Interval { get; set; }      
    public string Periodicity { get; set; }
    public int BarsCount { get; set; }
}