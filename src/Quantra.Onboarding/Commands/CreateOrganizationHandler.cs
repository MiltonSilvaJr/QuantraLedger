
using MediatR;
using Quantra.Domain.Models;
using Quantra.Persistence;

namespace Quantra.Onboarding.Commands;

public class CreateOrganizationHandler : IRequestHandler<CreateOrganization, Guid>
{
    private readonly LedgerDbContext _db;
    public CreateOrganizationHandler(LedgerDbContext db) => _db = db;

    public async Task<Guid> Handle(CreateOrganization request, CancellationToken cancellationToken)
    {
        var org = new Organization { Name = request.Name };
        _db.Organizations.Add(org);
        await _db.SaveChangesAsync(cancellationToken);
        return org.Id;
    }
}
