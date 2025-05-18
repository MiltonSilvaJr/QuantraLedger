
using MediatR;
using Quantra.Domain.Models;
using Quantra.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Quantra.Onboarding.Commands;

public class CreateAccountHandler : IRequestHandler<CreateAccount, Guid>
{
    private readonly LedgerDbContext _db;
    public CreateAccountHandler(LedgerDbContext db) => _db = db;

    public async Task<Guid> Handle(CreateAccount request, CancellationToken cancellationToken)
    {
        // simple validation org exists
        var org = await _db.Organizations.FindAsync(new object[]{request.OrganizationId}, cancellationToken);
        if (org == null) throw new Exception("Organization not found");

        var acc = new Account { OrganizationId = request.OrganizationId, Code = request.Code, Currency = request.Currency };
        _db.Accounts.Add(acc);
        await _db.SaveChangesAsync(cancellationToken);
        return acc.Id;
    }
}
