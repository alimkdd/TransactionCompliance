using FluentValidation;
using TransactionCompliance.Application.DTO.Request;

namespace TransactionCompliance.Application.Validations;

public class TransactionHistoryRequestModelValidator
    : AbstractValidator<TransactionHistoryFilter>
{
    public TransactionHistoryRequestModelValidator()
    {
        RuleFor(x => x.FromDate)
            .LessThanOrEqualTo(x => x.ToDate)
            .WithMessage("'FromDate' must be earlier than or equal to 'ToDate'.");

        RuleFor(x => x.ToDate)
            .GreaterThanOrEqualTo(x => x.FromDate)
            .WithMessage("'ToDate' must be later than or equal to 'FromDate'.");

        RuleFor(x => x.StatusId)
            .GreaterThanOrEqualTo(0)
            .WithMessage("'StatusId' must be a non-negative number.");

        RuleFor(x => x.TransactionTypeId)
            .GreaterThanOrEqualTo(0)
            .WithMessage("'TransactionTypeId' must be a non-negative number.");
    }
}