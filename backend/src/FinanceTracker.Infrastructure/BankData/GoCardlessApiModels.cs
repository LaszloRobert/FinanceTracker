using System.Text.Json.Serialization;

namespace FinanceTracker.Infrastructure.BankData;

internal sealed record GoCardlessInstitution(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("logo")] string? Logo,
    [property: JsonPropertyName("transaction_total_days")] string TransactionTotalDays,
    [property: JsonPropertyName("max_access_valid_for_days")] string MaxAccessValidForDays,
    [property: JsonPropertyName("countries")] List<string> Countries);

internal sealed record GoCardlessRequisitionRequest(
    [property: JsonPropertyName("redirect")] string Redirect,
    [property: JsonPropertyName("institution_id")] string InstitutionId);

internal sealed record GoCardlessRequisitionResponse(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("link")] string Link,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("institution_id")] string InstitutionId,
    [property: JsonPropertyName("accounts")] List<string> Accounts);

internal sealed record GoCardlessAccountDetailsWrapper(
    [property: JsonPropertyName("account")] GoCardlessAccountDetails Account);

internal sealed record GoCardlessAccountDetails(
    [property: JsonPropertyName("iban")] string? Iban,
    [property: JsonPropertyName("currency")] string Currency,
    [property: JsonPropertyName("ownerName")] string? OwnerName,
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("product")] string? Product);

internal sealed record GoCardlessBalancesWrapper(
    [property: JsonPropertyName("balances")] List<GoCardlessBalance> Balances);

internal sealed record GoCardlessBalance(
    [property: JsonPropertyName("balanceAmount")] GoCardlessAmount BalanceAmount,
    [property: JsonPropertyName("balanceType")] string BalanceType,
    [property: JsonPropertyName("referenceDate")] string? ReferenceDate);

internal sealed record GoCardlessAmount(
    [property: JsonPropertyName("amount")] string Amount,
    [property: JsonPropertyName("currency")] string Currency);

internal sealed record GoCardlessTransactionsWrapper(
    [property: JsonPropertyName("transactions")] GoCardlessTransactions Transactions);

internal sealed record GoCardlessTransactions(
    [property: JsonPropertyName("booked")] List<GoCardlessTransaction>? Booked,
    [property: JsonPropertyName("pending")] List<GoCardlessTransaction>? Pending);

internal sealed record GoCardlessTransaction(
    [property: JsonPropertyName("transactionId")] string? TransactionId,
    [property: JsonPropertyName("internalTransactionId")] string? InternalTransactionId,
    [property: JsonPropertyName("transactionAmount")] GoCardlessAmount TransactionAmount,
    [property: JsonPropertyName("bookingDate")] string? BookingDate,
    [property: JsonPropertyName("valueDate")] string? ValueDate,
    [property: JsonPropertyName("creditorName")] string? CreditorName,
    [property: JsonPropertyName("creditorAccount")] GoCardlessAccountRef? CreditorAccount,
    [property: JsonPropertyName("debtorName")] string? DebtorName,
    [property: JsonPropertyName("debtorAccount")] GoCardlessAccountRef? DebtorAccount,
    [property: JsonPropertyName("remittanceInformationUnstructured")] string? RemittanceInformationUnstructured,
    [property: JsonPropertyName("remittanceInformationUnstructuredArray")] List<string>? RemittanceInformationUnstructuredArray,
    [property: JsonPropertyName("additionalInformation")] string? AdditionalInformation,
    [property: JsonPropertyName("merchantCategoryCode")] string? MerchantCategoryCode,
    [property: JsonPropertyName("bankTransactionCode")] string? BankTransactionCode);

internal sealed record GoCardlessAccountRef(
    [property: JsonPropertyName("iban")] string? Iban);
