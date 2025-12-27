using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Dynamic.Core;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using TransactionCompliance.Application.DTO.Common;
using TransactionCompliance.Application.DTO.Request;
using TransactionCompliance.Application.DTO.Response;
using TransactionCompliance.Application.Interface;
using TransactionCompliance.Infrastructure.Context;

namespace TransactionCompliance.Application.Services;

public class TransactionRepository : ITransactionRepository
{
    private readonly AppDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<TransactionRepository> _logger;
    private readonly IConfiguration _configuration;

    public TransactionRepository(AppDbContext dbContext,
        IHttpContextAccessor httpContextAccessor,
        ILogger<TransactionRepository> logger,
        IConfiguration configuration)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<string> Login(LoginRequestModel request)
    {
        try
        {
            // Validate user credentials
            var user = await _dbContext.UserAccounts
                .Where(u => u.UserName == request.Username && u.Password == request.Password)
                .FirstOrDefaultAsync();

            if (user == null) return null;

            // Read JWT configuration values
            var jwtKey = _configuration["Jwt:Key"];
            var jwtIssuer = _configuration["Jwt:Issuer"];
            var jwtAudience = _configuration["Jwt:Audience"];

            // Define claims that will be embedded in the JWT
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) };

            // Create a symmetric security key and signing credentials
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Generate the JWT token with a 12-hour expiration time
            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(12),
                signingCredentials: creds
            );

            // Return the serialized token string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ActionResult<DataTableResponse<TransactionHistoryResponseModel>>> GetTransactionHistory(string request)
    {
        try
        {
            // Deserialize dataTable request
            var dataTableRequest = string.IsNullOrEmpty(request)
                ? null
                : JsonSerializer.Deserialize<DataTableRequest<TransactionHistoryFilter>>(request);

            var filter = dataTableRequest?.TableFilter;

            // Apply default sorting and pagination values
            var sortColumnName = dataTableRequest?.SortCol ?? "CreatedAt";
            var sortDirection = dataTableRequest?.SortDirection ?? "desc";
            var pageSize = dataTableRequest?.PageSize ?? 50;
            pageSize = Math.Min(pageSize, 100);
            var skip = (dataTableRequest?.PageIndex ?? 0) * pageSize;

            // Retrieve the authenticated user's ID from JWT claims
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim))
            {
                _logger.LogWarning("Unauthorized access attempt to transaction history. Request: {@Request}", request);
                return new UnauthorizedObjectResult(new { Message = "User is not authorized." });
            }

            var userId = int.Parse(userIdClaim);

            var query = _dbContext.Transactions
                        .AsNoTracking()
                        .Include(t => t.UserAccount)
                        .Where(t => t.UserId == userId);

            // Apply optional filters (date range, transaction type, status)
            if (filter != null)
            {
                if (filter.FromDate != default)
                    query = query.Where(t => t.CreatedAt >= filter.FromDate);

                if (filter.ToDate != default)
                    query = query.Where(t => t.CreatedAt <= filter.ToDate);

                if (filter.TransactionTypeId != 0)
                    query = query.Where(t => t.TransactionTypeId == filter.TransactionTypeId);

                if (filter.StatusId != 0)
                    query = query.Where(t => t.StatusId == filter.StatusId);
            }

            // Get total record count before pagination
            var totalRecords = await query.CountAsync();

            // Apply sorting, pagination, and projection to response model using Mapster
            var data = await query
                .OrderBy($"{sortColumnName} {sortDirection}")
                .Skip(skip)
                .Take(pageSize)
                .ProjectToType<TransactionHistoryResponseModel>()
                .ToListAsync();

            _logger.LogInformation("Transaction history accessed by user {@UserAuditInfo}",
                new
                {
                    UserId = userId,
                    RequestTime = DateTime.UtcNow,
                    Filter = filter,
                    SortColumn = sortColumnName,
                    SortDirection = sortDirection,
                    dataTableRequest?.PageIndex,
                    PageSize = pageSize,
                    RecordCount = data.Count
                });

            var response = new DataTableResponse<TransactionHistoryResponseModel>
            {
                Data = data,
                TotalRecord = totalRecords
            };

            return response;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Invalid JSON request format in GetTransactionHistory. Request: {Request}", request);
            return new BadRequestObjectResult(new { Message = "Invalid request format." });
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database access error while retrieving transaction history.");
            return new ObjectResult(new { Message = "A database error occurred while retrieving transaction history." })
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while retrieving transaction history.");
            return new ObjectResult(new { Message = "An unexpected error occurred. Please try again later." })
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
    }
}