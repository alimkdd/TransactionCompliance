namespace TransactionCompliance.Application.DTO.Common;

public class DataTableResponse<T>
{
    public IEnumerable<T> Data { get; set; } = new List<T>();
    public int TotalRecord { get; set; }
}