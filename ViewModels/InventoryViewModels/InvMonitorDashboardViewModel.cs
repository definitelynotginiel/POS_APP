using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POS_APP.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace POS_APP.ViewModels.InventoryViewModels;

public partial class InvMonitorDashboardViewModel : ObservableObject
{
    private readonly InventoryViewModel? _parent;

    public ObservableCollection<InventoryItem> AllInventoryItems => _parent?.AllInventoryItems ?? new ObservableCollection<InventoryItem>();

    public ObservableCollection<string> ItemCategories => _parent?.ItemCategories ?? new ObservableCollection<string>();

    [ObservableProperty]
    private ObservableCollection<InventoryItem> inventoryItems = new();

    [ObservableProperty]
    private string searchText = string.Empty;

    partial void OnSearchTextChanged(string value)
    {
        ApplyFilter();
    }

    public InvMonitorDashboardViewModel(InventoryViewModel? parent)
    {
        _parent = parent;

        if (ItemCategories.Count > 0)
        {
            SelectedCategory = ItemCategories.First();
        }
        else
        {
            SelectedCategory = string.Empty;
        }

        // Initialize with all items if no category is selected
        if (string.IsNullOrEmpty(SelectedCategory))
        {
            InventoryItems = new ObservableCollection<InventoryItem>(AllInventoryItems);
        }

        // Calculate initial summary counts
        CalculateSummaryCounts();
    }

    [ObservableProperty]
    private InventoryItem selectedInventoryItem;

    [ObservableProperty]
    private int nearExpiryCount;

    [ObservableProperty]
    private int expiredCount;

    [ObservableProperty]
    private int goodStockCount;

    [ObservableProperty]
    private int totalItemsCount;

    [ObservableProperty]
    private int lowStockCount;

    [ObservableProperty]
    private string selectedCategory;

    partial void OnSelectedCategoryChanged(string value)
    {
        ApplyFilter();
        CalculateSummaryCounts();
    }

    partial void OnSelectedInventoryItemChanged(InventoryItem value)
    {
        UpdateExpirySummary(value);
    }

    private void ApplyFilter()
    {
        var filtered = AllInventoryItems.AsEnumerable();

        // Apply category filter
        if (!string.IsNullOrEmpty(SelectedCategory) && SelectedCategory != "All Categories")
        {
            filtered = filtered.Where(i => i.Category == SelectedCategory);
        }

        // Apply search filter
        if (!string.IsNullOrEmpty(SearchText))
        {
            filtered = filtered.Where(i =>
                i.Name.Contains(SearchText, System.StringComparison.OrdinalIgnoreCase) ||
                (i.Category?.Contains(SearchText, System.StringComparison.OrdinalIgnoreCase) ?? false));
        }

        InventoryItems = new ObservableCollection<InventoryItem>(filtered);
        TotalItemsCount = InventoryItems.Count;
    }

    private void CalculateSummaryCounts()
    {
        // Calculate counts for all items in the current view
        TotalItemsCount = InventoryItems.Count;

        LowStockCount = InventoryItems.Count(i =>
            i.StockStatus == "Low Stock");

        // Calculate expiry counts across all items
        var now = DateTime.Now;

        NearExpiryCount = InventoryItems.Sum(item =>
            item.Packs?.Count(p =>
                p.ExpirationDate.HasValue &&
                p.ExpirationDate.Value > now &&
                p.ExpirationDate.Value <= now.AddDays(7)) ?? 0);

        ExpiredCount = InventoryItems.Sum(item =>
            item.Packs?.Count(p =>
                p.ExpirationDate.HasValue &&
                p.ExpirationDate.Value < now) ?? 0);

        GoodStockCount = InventoryItems.Sum(item =>
            item.Packs?.Count(p =>
                p.ExpirationDate.HasValue &&
                p.ExpirationDate.Value > now.AddDays(7)) ?? 0);
    }

    private void UpdateExpirySummary(InventoryItem item)
    {
        if (item == null || item.Packs == null)
        {
            return;
        }

        var now = DateTime.Now;

        // Update counts for the selected item
        NearExpiryCount = item.Packs.Count(p =>
            p.ExpirationDate.HasValue &&
            p.ExpirationDate.Value > now &&
            p.ExpirationDate.Value <= now.AddDays(7));

        ExpiredCount = item.Packs.Count(p =>
            p.ExpirationDate.HasValue &&
            p.ExpirationDate.Value < now);

        GoodStockCount = item.Packs.Count(p =>
            p.ExpirationDate.HasValue &&
            p.ExpirationDate.Value > now.AddDays(7));
    }

    [RelayCommand]
    private void GoToStockIn()
    {
        _parent?.ShowStockInCommand.Execute(null);
    }

    [RelayCommand]
    private void GoToStockOut()
    {
        _parent?.ShowStockOutCommand.Execute(null);
    }

    [RelayCommand]
    private void GoToExpireTrack()
    {
        _parent?.ShowExpireTrackCommand.Execute(null);
    }

    [RelayCommand]
    private void GoToRegistry()
    {
        _parent?.ShowRegistryCommand.Execute(null);
    }
}