using Microsoft.AspNetCore.Mvc;
using TransactionCompliance.Application.DTO.Common;
using TransactionCompliance.Application.DTO.Request;
using TransactionCompliance.Application.DTO.Response;

namespace TransactionCompliance.Application.Interface;

public interface ITransactionRepository
{
    Task<ActionResult<DataTableResponse<TransactionHistoryResponseModel>>> GetTransactionHistory(string request);

    Task<string> Login(LoginRequestModel reqeust);
}