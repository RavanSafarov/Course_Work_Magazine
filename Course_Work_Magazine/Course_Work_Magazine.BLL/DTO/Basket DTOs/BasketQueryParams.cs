namespace Course_Work_Magazine.DTO.Basket_DTOs;

public class BasketQueryParams
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public string? Sort { get; set; }
    public string? SortDirection { get; set; }
}
