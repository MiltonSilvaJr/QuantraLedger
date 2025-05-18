
using MediatR;

namespace Quantra.Onboarding.Commands;

public record CreateOrganization(string Name) : IRequest<Guid>;
