using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FinanceTracker.Application.Abstractions.BankData;
using FinanceTracker.Application.Abstractions.BankData.Models;

namespace FinanceTracker.Infrastructure.BankData;

internal sealed class GoCardlessBankDataService(
    HttpClient httpClient,
    GoCardlessTokenService tokenService)
    : IBankDataService
{
    public async Task<List<BankInstitution>> GetInstitutionsAsync(
        string countryCode,
        CancellationToken cancellationToken)
    {
        List<GoCardlessInstitution>? institutions = await GetAsync<List<GoCardlessInstitution>>(
            $"/api/v2/institutions/?country={countryCode}",
            cancellationToken);

        return institutions?.Select(MapInstitution).ToList() ?? [];
    }

    public async Task<BankInstitution?> GetInstitutionByIdAsync(
        string institutionId,
        CancellationToken cancellationToken)
    {
        GoCardlessInstitution? institution = await GetAsync<GoCardlessInstitution>(
            $"/api/v2/institutions/{institutionId}/",
            cancellationToken);

        return institution is not null ? MapInstitution(institution) : null;
    }

    public async Task<RequisitionResult> CreateRequisitionAsync(
        string institutionId,
        Uri redirectUrl,
        CancellationToken cancellationToken)
    {
        var body = new GoCardlessRequisitionRequest(redirectUrl.ToString(), institutionId);

        GoCardlessRequisitionResponse? requisition = await PostAsync<GoCardlessRequisitionRequest, GoCardlessRequisitionResponse>(
            "/api/v2/requisitions/",
            body,
            cancellationToken);

        return new RequisitionResult(requisition!.Id, requisition.Link);
    }

    public async Task<RequisitionDetails> GetRequisitionDetailsAsync(
        string requisitionId,
        CancellationToken cancellationToken)
    {
        GoCardlessRequisitionResponse? requisition = await GetAsync<GoCardlessRequisitionResponse>(
            $"/api/v2/requisitions/{requisitionId}/",
            cancellationToken);

        return new RequisitionDetails(
            requisition!.Id,
            requisition.InstitutionId,
            MapRequisitionStatus(requisition.Status),
            requisition.Accounts ?? []);
    }

    public async Task<BankAccountDetails> GetAccountDetailsAsync(
        string accountId,
        CancellationToken cancellationToken)
    {
        GoCardlessAccountDetailsWrapper? wrapper = await GetAsync<GoCardlessAccountDetailsWrapper>(
            $"/api/v2/accounts/{accountId}/details/",
            cancellationToken);

        GoCardlessAccountDetails details = wrapper!.Account;

        return new BankAccountDetails(
            details.Iban,
            details.Currency,
            details.OwnerName,
            details.Name,
            details.Product);
    }

    public async Task<List<BankAccountBalance>> GetAccountBalancesAsync(
        string accountId,
        CancellationToken cancellationToken)
    {
        GoCardlessBalancesWrapper? wrapper = await GetAsync<GoCardlessBalancesWrapper>(
            $"/api/v2/accounts/{accountId}/balances/",
            cancellationToken);

        return wrapper?.Balances.Select(b => new BankAccountBalance(
            decimal.Parse(b.BalanceAmount.Amount, CultureInfo.InvariantCulture),
            b.BalanceAmount.Currency,
            b.BalanceType,
            b.ReferenceDate is not null ? DateOnly.Parse(b.ReferenceDate, CultureInfo.InvariantCulture) : null))
            .ToList() ?? [];
    }

    public async Task<BankTransactionResult> GetTransactionsAsync(
        string accountId,
        DateOnly? dateFrom,
        DateOnly? dateTo,
        CancellationToken cancellationToken)
    {
        string url = $"/api/v2/accounts/{accountId}/transactions/";

        var queryParams = new List<string>();

        if (dateFrom.HasValue)
        {
            queryParams.Add($"date_from={dateFrom.Value:yyyy-MM-dd}");
        }

        if (dateTo.HasValue)
        {
            queryParams.Add($"date_to={dateTo.Value:yyyy-MM-dd}");
        }

        if (queryParams.Count > 0)
        {
            url += "?" + string.Join("&", queryParams);
        }

        GoCardlessTransactionsWrapper? wrapper = await GetAsync<GoCardlessTransactionsWrapper>(
            url, cancellationToken);

        List<BankTransaction> booked = MapTransactions(wrapper?.Transactions.Booked, isBooked: true);
        List<BankTransaction> pending = MapTransactions(wrapper?.Transactions.Pending, isBooked: false);

        return new BankTransactionResult(booked, pending);
    }

    private async Task<T?> GetAsync<T>(string url, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        await SetAuthHeaderAsync(request, cancellationToken);

        HttpResponseMessage response = await httpClient.SendAsync(request, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<T>(cancellationToken);
    }

    private async Task<TResponse?> PostAsync<TBody, TResponse>(
        string url, TBody body, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = JsonContent.Create(body)
        };
        await SetAuthHeaderAsync(request, cancellationToken);

        HttpResponseMessage response = await httpClient.SendAsync(request, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken);
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (!response.IsSuccessStatusCode)
        {
            string body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException(
                $"GoCardless API error ({(int)response.StatusCode}): {body}",
                null,
                response.StatusCode);
        }
    }

    private async Task SetAuthHeaderAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        string token = await tokenService.GetAccessTokenAsync(cancellationToken);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private static BankInstitution MapInstitution(GoCardlessInstitution i) =>
        new(
            i.Id,
            i.Name,
            i.Logo,
            int.Parse(i.TransactionTotalDays, CultureInfo.InvariantCulture),
            int.Parse(i.MaxAccessValidForDays, CultureInfo.InvariantCulture),
            i.Countries);

    private static List<BankTransaction> MapTransactions(
        List<GoCardlessTransaction>? transactions,
        bool isBooked)
    {
        if (transactions is null)
        {
            return [];
        }

        return transactions.Select(t =>
        {
            string? remittanceInfo = t.RemittanceInformationUnstructured
                ?? (t.RemittanceInformationUnstructuredArray is not null
                    ? string.Join(" | ", t.RemittanceInformationUnstructuredArray)
                    : null);

            return new BankTransaction(
                t.TransactionId,
                t.InternalTransactionId,
                decimal.Parse(t.TransactionAmount.Amount, CultureInfo.InvariantCulture),
                t.TransactionAmount.Currency,
                t.BookingDate is not null ? DateOnly.Parse(t.BookingDate, CultureInfo.InvariantCulture) : null,
                t.ValueDate is not null ? DateOnly.Parse(t.ValueDate, CultureInfo.InvariantCulture) : null,
                new BankTransactionParty(t.CreditorName, t.CreditorAccount?.Iban),
                new BankTransactionParty(t.DebtorName, t.DebtorAccount?.Iban),
                remittanceInfo,
                t.AdditionalInformation,
                t.MerchantCategoryCode,
                t.BankTransactionCode,
                isBooked);
        }).ToList();
    }

    private static RequisitionStatus MapRequisitionStatus(string status) =>
        status.ToUpperInvariant() switch
        {
            "CR" => RequisitionStatus.Created,
            "GC" => RequisitionStatus.GivingConsent,
            "LN" => RequisitionStatus.Linked,
            "EX" => RequisitionStatus.Expired,
            "RJ" => RequisitionStatus.Rejected,
            "SA" => RequisitionStatus.Suspended,
            _ => RequisitionStatus.Error
        };
}
