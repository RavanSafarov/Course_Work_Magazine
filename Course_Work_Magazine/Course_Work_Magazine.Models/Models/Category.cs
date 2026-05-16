namespace Course_Work_Magazine.Models;

public class Category
{
    public int Id { get; set; }

    public string NameOfCategory { get; set; } = string.Empty;

    public List<Product> Products { get; set; } = new();
}
