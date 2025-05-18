
namespace Quantra.Domain.Models;

public class AssetRate
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FromCurrency { get; set; } = "BRL";
    public string ToCurrency { get; set; } = "BRL";
    public decimal Rate { get; set; } = 1m;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
