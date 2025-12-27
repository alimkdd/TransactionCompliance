namespace TransactionCompliance.Application.DTO.Common;

public class DataTableRequest<TFilter>
{
    public int PageIndex { get; set; } = 0;
    public int PageSize { get; set; } = 10;
    public string? SortCol { get; set; }
    public string? SortDirection { get; set; }
    public TFilter? TableFilter { get; set; }
}