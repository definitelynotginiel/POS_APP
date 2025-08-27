using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using POS_APP.Models;

namespace POS_APP.ViewModels.InventoryViewModels;

public partial class InvStockOutViewModel : ObservableObject
{
    private readonly InventoryViewModel? _parent;

    public ObservableCollection<InventoryItem> AllInventoryItems => _parent?.AllInventoryItems ?? new ObservableCollection<InventoryItem>();

    public ObservableCollection<string> ItemCategories => _parent?.ItemCategories ?? new ObservableCollection<string>();

    public ObservableCollection<string> UnitMeasurementTypes => _parent?.UnitMeasurementTypes ?? new ObservableCollection<string>();

    public ObservableCollection<string> StockOutReasons => _parent?.StockOutReasons ?? new ObservableCollection<string>();

    public InvStockOutViewModel(InventoryViewModel? parent)
    {
        _parent = parent;

        SelectedCategory = ItemCategories.FirstOrDefault() ?? string.Empty;
        SelectedUnitType = UnitMeasurementTypes.FirstOrDefault() ?? string.Empty; // Initialize 'selectedUnitType'

        FilteredItems = new ObservableCollection<string>();
        ApplyFilter();
    }

    // --- Collections ---

    partial void OnItemNameChanged(string value) => OnPropertyChanged(nameof(StockOutSummary));
    partial void OnUnitCostChanged(double value) => OnPropertyChanged(nameof(StockOutSummary));
    partial void OnExpirationDateChanged(DateTime? value) => OnPropertyChanged(nameof(StockOutSummary));
    partial void OnQuantityChanged(double value) => OnPropertyChanged(nameof(StockOutSummary));
    partial void OnPackQuantityChanged(double value) => OnPropertyChanged(nameof(StockOutSummary));
    partial void OnSelectedUnitTypeChanged(string value) => OnPropertyChanged(nameof(StockOutSummary));
     
    partial void OnCustomReasonChanged(string value) => OnPropertyChanged(nameof(StockOutSummary));

    [ObservableProperty]
    public ObservableCollection<string> filteredItems;

    // --- Selected values ---
    [ObservableProperty]
    public string selectedCategory;

    partial void OnSelectedCategoryChanged(string value) => ApplyFilter();

    [ObservableProperty]
    public string selectedItem;
    partial void OnSelectedItemChanged(string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            ItemName = value;

            // Optionally, prefill quantity and unit from inventory
            var item = AllInventoryItems.FirstOrDefault(i => i.Name == value && i.Category == SelectedCategory);
            if (item != null)
            {
                SelectedUnitType = item.Unit;
            }
        }
    }

    [ObservableProperty]
    public string selectedReason;

    partial void OnSelectedReasonChanged(string value)
    {
        IsOtherReasonVisible = value == "Other";
        OnPropertyChanged(nameof(StockOutSummary));

    }

    // --- Form fields ---
    [ObservableProperty]
    public string itemName;

    [ObservableProperty]
    public double unitCost;

    [ObservableProperty]
    public DateTime? expirationDate = DateTime.Now;

    [ObservableProperty]
    public string customReason;

    [ObservableProperty]
    public double packQuantity;

    [ObservableProperty]
    public double quantity;

    [ObservableProperty]
    public string selectedUnitType;

    [ObservableProperty]
    public string searchQuery;

    partial void OnSearchQueryChanged(string value) => ApplyFilter();

    // --- UI helpers ---
    [ObservableProperty]
    public bool isOtherReasonVisible;

    // --- Live summary ---
    public string StockOutSummary
    {
        get
        {
            if (string.IsNullOrWhiteSpace(ItemName))
                return string.Empty;

            double totalQuantity = Quantity * PackQuantity;

            return $@"
                Category: {SelectedCategory}
                Item: {ItemName} ({totalQuantity} {SelectedUnitType})
                Cost: {UnitCost}
                Expiration Date: {ExpirationDate?.ToShortDateString() ?? "-"}
                Reason: {(SelectedReason == "Other" ? CustomReason : SelectedReason)}";
        }
    }

    [RelayCommand]
    private void StockOut()
    {
        if (string.IsNullOrWhiteSpace(ItemName) ||
            string.IsNullOrWhiteSpace(SelectedCategory) ||
            PackQuantity <= 0 ||
            Quantity <= 0 ||
            ExpirationDate == null)
            return;

        var item = AllInventoryItems.FirstOrDefault(i =>
            i.Name.Equals(ItemName.Trim(), StringComparison.OrdinalIgnoreCase) &&
            i.Category == SelectedCategory);

        if (item == null)
        {
            System.Windows.MessageBox.Show("Item not found in inventory.", "Error",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            return;
        }

        // All packs with this expiration date
        var sameDatePacks = item.Packs
            .Where(p => p.ExpirationDate.HasValue &&
                        p.ExpirationDate.Value.Date == ExpirationDate.Value.Date)
            .ToList();

        if (sameDatePacks.Count == 0)
        {
            System.Windows.MessageBox.Show("No packs available with the selected expiration date.", "Error",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            return;
        }

        // We treat Quantity as the per-pack size the user intends to remove.
        // Enforce exact match to each pack’s size to avoid “25kg for a 20kg pack” issues.
        if (sameDatePacks.Any(p => Math.Abs(p.PackQuantity - Quantity) > 0.0001))
        {
            System.Windows.MessageBox.Show(
                "Per-pack quantity does not match the stored pack size.\n" +
                "Please match the pack’s quantity exactly.",
                "Error",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            return;
        }

        int requestedPacks = (int)PackQuantity; // number of packs to remove
        if (requestedPacks > sameDatePacks.Count)
        {
            System.Windows.MessageBox.Show("Not enough packs for that expiration date.", "Error",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            return;
        }

        // Remove the exact number of packs
        for (int i = 0; i < requestedPacks; i++)
            item.Packs.Remove(sameDatePacks[i]);

        // Update status
        item.StockStatus = item.Quantity >= item.Threshold ? "Good Stock" : "Low Stock";

        // Optional: remove item entirely if no packs left
        if (item.PacksCount == 0 && item.Quantity <= 0)
            _parent?.AllInventoryItems.Remove(item);

        ApplyFilter(); // refresh UI lists

        System.Windows.MessageBox.Show(
                "Stock-out successful!" +
                $"{ItemName}'s pack/s stocked out successfully!\n" +
                $"Item's New Total Packs : {item.PacksCount}\n" +
                $"Item's New Total Quantity : {item.QuantityDisplay}\n" +
                $"Status: {item.StockStatus}",
                "Success",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Information);
        // Clear form
        ItemName = string.Empty;
        Quantity = 0;
        PackQuantity = 0;
        UnitCost = 0;
        ExpirationDate = null;
        CustomReason = string.Empty;
    }




    [RelayCommand]
    private void Back()
    {
        _parent?.ShowDashboardCommand.Execute(null);
    }

    // --- Filtering ---
    private void ApplyFilter()
    {
        var items = AllInventoryItems
            .Where(i => SelectedCategory == null || i.Category == SelectedCategory)
            .Select(i => i.Name);

        if (!string.IsNullOrWhiteSpace(SearchQuery))
            items = items.Where(i => i.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase));

        FilteredItems = new ObservableCollection<string>(items);
    }
}
