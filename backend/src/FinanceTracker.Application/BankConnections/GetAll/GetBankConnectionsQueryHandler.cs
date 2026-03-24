using FinanceTracker.Application.Abstractions.Authentication;
using FinanceTracker.Application.Abstractions.Data;
using FinanceTracker.Application.Abstractions.Messaging;
using FinanceTracker.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Application.BankConnections.GetAll;

internal sealed class GetBankConnectionsQueryHandler(
    IApplicationDbContext dbContext,
    IUserContext userContext)
    : IQueryHandler<GetBankConnectionsQuery, List<BankConnectionResponse>>
{
    public async Task<Result<List<BankConnectionResponse>>> Handle(
        GetBankConnectionsQuery query,
        CancellationToken cancellationToken)
    {
        Guid userId = userContext.UserId;

        List<BankConnectionResponse> connections = await dbContext.BankConnections
            .Where(bc => bc.UserId == userId)
            .Select(bc => new BankConnectionResponse(
                bc.Id,
                bc.InstitutionId,
                bc.InstitutionName,
                bc.InstitutionLogo,
                bc.Status.ToString(),
                bc.LinkedAt,
                bc.ExpiresAt,
                dbContext.Accounts.Count(a => a.BankConnectionId == bc.Id && !a.IsDeleted)))
            .ToListAsync(cancellationToken);

        return connections;
    }
}
