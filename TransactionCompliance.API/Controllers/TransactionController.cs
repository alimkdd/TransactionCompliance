using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TransactionCompliance.Application.DTO.Common;
using TransactionCompliance.Application.DTO.Request;
using TransactionCompliance.Application.DTO.Response;
using TransactionCompliance.Application.Interface;

namespace TransactionCompliance.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]

public class TransactionController : ControllerBase
{
    private readonly ILogger<TransactionController> _logger;
    private readonly ITransactionRepository _transactionRepository;
    public TransactionController(ILogger<TransactionController> logger, ITransactionRepository transactionRepository)
    {
        _logger = logger;
        _transactionRepository = transactionRepository;
    }

    #region GET

    [HttpGet("History")]
    [EnableRateLimiting("Default")]
    public async Task<ActionResult<DataTableResponse<TransactionHistoryResponseModel>>> GetTransactionHistory([FromQuery] string request)
        => await _transactionRepository.GetTransactionHistory(request);

    #endregion

    #region POST

    [AllowAnonymous]
    [HttpPost("Login")]
    public async Task<string> Login([FromBody] LoginRequestModel request)
         => await _transactionRepository.Login(request);

    #endregion
}