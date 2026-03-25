using FinanceTracker.Application.Abstractions.BankData;

namespace FinanceTracker.Application.Abstractions.BankData.Models;

public sealed record RequisitionDetails(
    string RequisitionId,
    string InstitutionId,
    RequisitionStatus Status,
    List<string> AccountIds);
