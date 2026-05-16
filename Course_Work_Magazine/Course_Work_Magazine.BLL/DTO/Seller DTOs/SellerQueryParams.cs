namespace Course_Work_Magazine.DTO.Customer_DTOs;

public class SellerQueryParams
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public string? Sort { get; set; }
    public string? SortDirection { get; set; }
    public string? SearchByName { get; set; }
    public bool IncludeArchived { get; set; } = false;
}
