namespace TransactionCompliance.Application.DTO.Request;

public record TransactionHistoryFilter(
    DateTime ToDate,
    DateTime FromDate,
    int StatusId,
    int TransactionTypeId);