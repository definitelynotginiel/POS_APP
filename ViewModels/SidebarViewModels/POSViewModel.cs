using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

public partial class POSViewModel : ObservableObject
{
    [ObservableProperty]
    private MenuCategory _selectedCategory;

    public ObservableCollection<MenuCategory> Categories { get; } = new();
    public ObservableCollection<KitchenOrder> KitchenOrders { get; set; } = new ObservableCollection<KitchenOrder>();

    public POSViewModel()
    {
        // Sample Kitchen Orders
        KitchenOrders.Add(new KitchenOrder { OrderId = "D1" });
        KitchenOrders.Add(new KitchenOrder { OrderId = "T1" });
        KitchenOrders.Add(new KitchenOrder { OrderId = "T2" });

        // Example Categories

        Categories.Add(new MenuCategory
        {
            Name = "Fries and Spaghetti",
            Products = new List<Product>
            {
                new Product { Name = "Fries", Price = 25, ImagePath = "", HasSize = true, Sizes = new List<string>{ "Small","Medium","Large" } },
                new Product { Name = "Fries Overload", Price = 69, ImagePath = "", HasSize = true, Sizes = new List<string>{ "Medium","Large" } },
                new Product { Name = "Spaghetti w/ Tuna Sandwich", Price = 59, ImagePath = "" },
                new Product { Name = "Czaeyra’s Spaghetti", Price = 79, ImagePath = "" }
            }
        });

        Categories.Add(new MenuCategory
        {
            Name = "Milktea",
            Products = new List<Product>
            {
                new Product { Name = "Classic Milk Tea", Price = 75, ImagePath = "", HasFlavor = true, Flavors = new List<string>{ "Taro","Honey","Chocolate" } },
                new Product { Name = "Brown Sugar Milk Tea", Price = 85, ImagePath = "", HasFlavor = true, Flavors = new List<string>{ "Taro","Caramel","Matcha" } }
            }
        });

        // Category with Subcategories example
        Categories.Add(new MenuCategory
        {
            Name = "Others",
            HasSubCategories = true,
            SubCategories = new List<SubCategory>
            {
                new SubCategory
                {
                    Name = "Soft Drinks",
                    Products = new List<Product>
                    {
                        new Product { Name = "Coke", Price = 36, ImagePath = "" },
                        new Product { Name = "Sprite", Price = 36, ImagePath = "" },
                        new Product { Name = "Royal", Price = 36, ImagePath = "" }
                    }
                },
                new SubCategory
                {
                    Name = "Water",
                    Products = new List<Product>
                    {
                        new Product { Name = "Mineral Water", Price = 12, ImagePath = "" }
                    }
                }
            }
        });

        SelectedCategory = Categories[0];
    }

    public class KitchenOrder
    {
        public string OrderId { get; set; }
    }

    [RelayCommand]
    private void SelectCategory(MenuCategory category)
    {
        SelectedCategory = category;
    }
}