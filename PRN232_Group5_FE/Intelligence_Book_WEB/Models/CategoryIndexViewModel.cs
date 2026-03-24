using Intelligence_Book_WEB.Models;

public class CategoryIndexViewModel
{
    public List<CategoryViewModel> Categories { get; set; } = new();

    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }

    public string SearchKeyword { get; set; } = string.Empty;
}