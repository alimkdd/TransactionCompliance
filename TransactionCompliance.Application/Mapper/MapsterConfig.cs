using Mapster;
using TransactionCompliance.Application.DTO.Response;
using TransactionCompliance.Domain.Models;

namespace TransactionCompliance.Application.Mapper;

public class MapsterConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Transaction, TransactionHistoryResponseModel>()
            // Mask account and card numbers during mapping
            .Map(dest => dest.AccountNumberMasked, src => MaskSensitiveNumber(src.AccountNumber))
            .Map(dest => dest.CardNumberMasked, src => MaskSensitiveNumber(src.CardNumber))
            // Map related names
            .Map(dest => dest.TransactionType, src => src.TransactionType.Name)
            .Map(dest => dest.Status, src => src.Status.Name);
    }

    private static string MaskSensitiveNumber(string number)
    {
        if (string.IsNullOrEmpty(number))
            return string.Empty;

        return number.Length > 4
            ? new string('*', number.Length - 4) + number[^4..]
            : number;
    }
}