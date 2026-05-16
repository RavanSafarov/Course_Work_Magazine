namespace Course_Work_Magazine.DTO.Product_DTOs;

public class ProductQueryParams
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public string? Sort { get; set; }
    public string? SortDirection { get; set; }
    public string? SearchByName { get; set; }
}
