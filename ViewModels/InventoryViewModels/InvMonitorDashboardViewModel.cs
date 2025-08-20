using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POS_APP.Models;
using POS_APP.ViewModels.InventoryViewModels;
using System.Collections.ObjectModel;
namespace POS_APP.ViewModels.InventoryViewModels;

public partial class InvMonitorDashboardViewModel : ObservableObject
{
    private readonly InventoryViewModel _parent;

    public ObservableCollection<InventoryItem> AllInventoryItems { get; set; } = new();

    [ObservableProperty]
    private ObservableCollection<InventoryItem> inventoryItems = new();

    [ObservableProperty]
    private InventoryItem selectedInventoryItem;

    [ObservableProperty]
    private int nearExpiryCount;

    [ObservableProperty]
    private int expiredCount;

    [ObservableProperty]
    private string selectedCategory;

    [ObservableProperty]
    private string selectedUnitType;

    public ObservableCollection<string> ItemCategories { get; set; } = [];
    public InvMonitorDashboardViewModel(InventoryViewModel parent)
    {
        _parent = parent;
        ItemCategories =
        [
            "Food Ingredients",
                "Beverage Ingredients",
                "Disposable Utensils",
                "Food Packaging",
                "Other"

                
        ];

        // Default to first category, ensuring null safety
        SelectedCategory = ItemCategories.FirstOrDefault() ?? string.Empty;
        var Fries = new InventoryItem
        {
            Name = "Fries",
            Category = "Food Ingredients",
            Unit = "kg",
            Quantity = 3,
            StockStatus = "In Stock", // Fix: Set the required 'StockStatus' property
            ExpirationDates = new ObservableCollection<DateTime?>
            {
                DateTime.Now.AddDays(-2), // expired
                DateTime.Now.AddDays(3),  // near expiry
                DateTime.Now.AddDays(-2), // expired
                DateTime.Now.AddDays(3),  // near expiry
                DateTime.Now.AddDays(30)  // good
            }
        };

        AllInventoryItems.Add(Fries);
        ApplyFilter();
    }

    partial void OnSelectedCategoryChanged(string value)
    {
        ApplyFilter(); // your existing filtering logic
    }


    partial void OnSelectedInventoryItemChanged(InventoryItem value) => UpdateExpirySummary(value); // always update totals

    private void ApplyFilter()
    {
        if (string.IsNullOrEmpty(SelectedCategory))
        {
            InventoryItems.Clear();
        }
        else
        {
            var filtered = AllInventoryItems
                .Where(i => i.Category == SelectedCategory)
                .ToList();
            InventoryItems = new ObservableCollection<InventoryItem>(filtered);
        }

        // update near-expiry/expired counts for the whole filtered list

    }

    // New method: sums expiry counts for all items in current category (InventoryItems)
    private void UpdateExpirySummary(InventoryItem item)
    {
        if (item == null || item.ExpirationDates == null)
        {
            NearExpiryCount = 0;
            ExpiredCount = 0;
            return;
        }

        var now = DateTime.Now;
        NearExpiryCount = item.ExpirationDates.Count(d => d.HasValue &&
                                                         d.Value > now &&
                                                         d.Value <= now.AddDays(7));

        ExpiredCount = item.ExpirationDates.Count(d => d.HasValue &&
                                                       d.Value < now);
    }

    [RelayCommand]
    private void GoToStockOut()
    {
        _parent.ShowStockOutCommand.Execute(null);
    }
}
