
namespace Quantra.Domain.Models;
public class AuditLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EntityId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string Payload { get; set; } = string.Empty;
}
