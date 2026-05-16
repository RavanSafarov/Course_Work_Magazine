namespace Course_Work_Magazine.DTO.Order_DTOs;

public class OrderQueryParams
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public string? Sort { get; set; }
    public string? SortDirection { get; set; }
    public string? SearchByComment { get; set; }
    public DateTimeOffset? StartDateFrom { get; set; }
    public DateTimeOffset? StartDateTo { get; set; }
    public string? Status { get; set; }
    public bool? IncludeArchived { get; set; }
    public string? UserId { get; set; }
}
