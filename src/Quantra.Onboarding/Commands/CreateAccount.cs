
using MediatR;

namespace Quantra.Onboarding.Commands;

public record CreateAccount(Guid OrganizationId, string Code, string Currency = "BRL") : IRequest<Guid>;
