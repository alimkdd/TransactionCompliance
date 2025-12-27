namespace TransactionCompliance.Application.DTO.Response;

public class TransactionHistoryResponseModel
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string TransactionType { get; set; }
    public string Status { get; set; }
    public decimal Amount { get; set; }
    public string AccountNumberMasked { get; set; }
    public string CardNumberMasked { get; set; }
}