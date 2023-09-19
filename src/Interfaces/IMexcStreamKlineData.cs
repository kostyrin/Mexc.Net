using Mexc.Net.Enums;

namespace Mexc.Net.Interfaces;

public interface IMexcStreamKlineData
{
    IMexcStreamKline Data { get; set; }
}

public interface IMexcStreamKline
{
    public DateTime OpenTime { get; set; }

    public decimal Volume { get; set; }

    public DateTime CloseTime { get; set; }

    public KlineInterval Interval { get; set; }

    public decimal OpenPrice { get; set; }

    public decimal ClosePrice { get; set; }

    public decimal HighPrice { get; set; }

    public decimal LowPrice { get; set; }
}