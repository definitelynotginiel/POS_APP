using System.Collections.Generic;

public class Product
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string ImagePath { get; set; }
    public bool HasSize { get; set; }
    public bool HasFlavor { get; set; }
    public List<string> Sizes { get; set; } = new List<string>();
    public List<string> Flavors { get; set; } = new List<string>();
}

public class MenuCategory
{
    public string Name { get; set; }
    public bool HasSubCategories { get; set; }
    public List<SubCategory> SubCategories { get; set; } = new List<SubCategory>();
    public List<Product> Products { get; set; } = new List<Product>();
}

public class SubCategory
{
    public string Name { get; set; }
    public List<Product> Products { get; set; } = new List<Product>();
}