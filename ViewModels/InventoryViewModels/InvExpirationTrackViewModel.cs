using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POS_APP.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace POS_APP.ViewModels.InventoryViewModels;

public partial class InvExpirationTrackViewModel : ObservableObject
{
    private readonly InventoryViewModel _parent;

    public ObservableCollection<string> ItemCategories => _parent?.ItemCategories ?? new ObservableCollection<string>();

    public InvExpirationTrackViewModel(InventoryViewModel parent)
    {
        _parent = parent;

        SelectedCategory = ItemCategories.FirstOrDefault() ?? string.Empty;
        SelectedStatusFilter = "All";

        ApplyFilter();
        CalculateSummaryCounts();
    }

    [ObservableProperty]
    private ObservableCollection<ItemPacks> inventoryItems = new();

    [ObservableProperty]
    private ItemPacks? selectedInventoryItem;

    [ObservableProperty]
    private string selectedCategory;

    [ObservableProperty]
    private string selectedStatusFilter;

    [ObservableProperty]
    private int totalItemsCount;

    [ObservableProperty]
    private int nearExpiryCount;

    [ObservableProperty]
    private int expiredCount;

    [ObservableProperty]
    private int goodStockCount;

    [ObservableProperty]
    private DateTime currentDate = DateTime.Now;

    public ObservableCollection<string> StatusFilters { get; } = new ObservableCollection<string>
    {
        "All", "Expired", "Near Expiry", "Good"
    };

    private string GetStatus(DateTime? date)
    {
        if (date == null) return "Unknown";
        var today = DateTime.Today;

        if (date <= today) return "Expired";
        if ((date - today).Value.TotalDays <= 7) return "Near Expiry";
        return "Good";
    }

    private int GetDaysRemaining(DateTime? expirationDate)
    {
        if (expirationDate == null) return -1;
        var today = DateTime.Today;
        return (int)(expirationDate.Value - today).TotalDays;
    }

    [RelayCommand]
    private void StockOut(ItemPacks? pack)
    {
        if (pack == null) return;

        // Ask user for confirmation
        var result = System.Windows.MessageBox.Show(
            $"Are you sure you want to remove a pack of {pack.ItemName} ({pack.QuantityDisplay})?",
            "Confirm Stock Out",
            System.Windows.MessageBoxButton.YesNo,
            System.Windows.MessageBoxImage.Warning);

        if (result != System.Windows.MessageBoxResult.Yes)
            return; // User clicked No, cancel removal

        // remove from grid first
        InventoryItems.Remove(pack);

        var item = _parent.AllInventoryItems
            .FirstOrDefault(i => i.Name == pack.ItemName && i.Category == pack.Category);

        if (item == null) return;

        var toRemove = item.Packs.FirstOrDefault(p =>
            p.ExpirationDate == pack.ExpirationDate &&
            Math.Abs(p.PackQuantity - pack.PackQuantity) < 0.0001);

        if (toRemove != null)
            item.Packs.Remove(toRemove);

        // update status
        item.StockStatus = item.Quantity >= item.Threshold ? "Good Stock" : "Low Stock";

        if (item.PacksCount == 0 && item.Quantity <= 0)
            _parent.AllInventoryItems.Remove(item);

        // Update the total items count
        TotalItemsCount = InventoryItems.Count;

        // Recalculate summary counts
        CalculateSummaryCounts();
    }

    partial void OnSelectedCategoryChanged(string value)
    {
        ApplyFilter();
        CalculateSummaryCounts();
    }

    partial void OnSelectedStatusFilterChanged(string value)
    {
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        if (_parent.AllInventoryItems == null) return;

        var rows = new ObservableCollection<ItemPacks>();
        int n = 1;

        foreach (var item in _parent.AllInventoryItems.Where(i =>
            string.IsNullOrEmpty(SelectedCategory) || i.Category == SelectedCategory))
        {
            foreach (var pack in item.Packs)
            {
                var status = GetStatus(pack.ExpirationDate);
                var daysRemaining = GetDaysRemaining(pack.ExpirationDate);

                // Apply status filter
                if (SelectedStatusFilter != "All" && SelectedStatusFilter != status)
                    continue;

                rows.Add(new ItemPacks
                {
                    ItemName = item.Name,
                    Category = item.Category,
                    Unit = item.Unit,
                    PackNumber = n++,
                    ExpirationDate = pack.ExpirationDate,
                    Status = status,
                    DaysRemaining = daysRemaining,
                    PackQuantity = pack.PackQuantity,
                    
                });
            }
        }

        InventoryItems = rows;
        TotalItemsCount = rows.Count;
    }

    private void CalculateSummaryCounts()
    {
        if (_parent.AllInventoryItems == null) return;

        var today = DateTime.Today;
        NearExpiryCount = 0;
        ExpiredCount = 0;
        GoodStockCount = 0;

        foreach (var item in _parent.AllInventoryItems.Where(i =>
            string.IsNullOrEmpty(SelectedCategory) || i.Category == SelectedCategory))
        {
            foreach (var pack in item.Packs)
            {
                var status = GetStatus(pack.ExpirationDate);

                switch (status)
                {
                    case "Near Expiry":
                        NearExpiryCount++;
                        break;
                    case "Expired":
                        ExpiredCount++;
                        break;
                    case "Good":
                        GoodStockCount++;
                        break;
                }
            }
        }
    }

    [RelayCommand]
    private void Back() => _parent.ShowDashboardCommand.Execute(null);
}