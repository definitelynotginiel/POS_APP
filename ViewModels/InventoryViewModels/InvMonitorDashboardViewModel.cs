using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POS_APP.Models;
using System.Collections.ObjectModel;

namespace POS_APP.ViewModels.InventoryViewModels;

public partial class InvMonitorDashboardViewModel : ObservableObject
{
    private readonly InventoryViewModel? _parent;

    public ObservableCollection<InventoryItem> AllInventoryItems => _parent?.AllInventoryItems ?? new ObservableCollection<InventoryItem>();

    public ObservableCollection<string> ItemCategories => _parent?.ItemCategories ?? new ObservableCollection<string>();

    [ObservableProperty]
    private ObservableCollection<InventoryItem> inventoryItems = new();


    public InvMonitorDashboardViewModel(InventoryViewModel? parent)
    {
        _parent = parent;

        SelectedCategory = ItemCategories.FirstOrDefault() ?? string.Empty;

        // Initialize non-nullable fields to default values
        SelectedInventoryItem = new InventoryItem
        {
            Name = string.Empty,
            Category = string.Empty,
            Unit = string.Empty,
            Quantity = 0,
            StockStatus = string.Empty,
            ExpirationDates = new ObservableCollection<DateTime?>()
        };

        SelectedUnitType = string.Empty;
    }

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

    partial void OnSelectedCategoryChanged(string value)
    {
        ApplyFilter(); 
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

    }

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
        _parent?.ShowStockOutCommand.Execute(null);
    }

    [RelayCommand]
    private void GoToExpireTrack()
    {
        _parent?.ShowExpireTrackCommand.Execute(null);
    }
}
