
namespace Quantra.Domain.Models;

public class Organization
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public ICollection<Account> Accounts { get; set; } = new List<Account>();
}
