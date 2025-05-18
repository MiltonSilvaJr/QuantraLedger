
namespace Quantra.Domain.Models;

public class Account
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code { get; set; } = string.Empty;
    public string Currency { get; set; } = "BRL";
    public Guid OrganizationId { get; set; }
    public Organization? Organization { get; set; }
}
