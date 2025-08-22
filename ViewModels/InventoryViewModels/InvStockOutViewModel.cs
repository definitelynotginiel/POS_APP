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

        // Find the item in inventory
        var item = AllInventoryItems.FirstOrDefault(i =>
            i.Name.Equals(ItemName.Trim(), StringComparison.OrdinalIgnoreCase) &&
            i.Category == SelectedCategory);

        if (item == null)
        {
            System.Windows.MessageBox.Show("Item not found in inventory.", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            return;
        }

        // Check if the expiration date exists in the item
        int availablePacksWithDate = item.ExpirationDates.Count(d => d.HasValue && d.Value.Date == ExpirationDate.Value.Date);
        if (availablePacksWithDate == 0)
        {
            System.Windows.MessageBox.Show("No packs available with the selected expiration date.", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            return;
        }

        // Ensure we do not remove more packs than exist for this expiration date
        int packsToRemove = Math.Min((int)PackQuantity, availablePacksWithDate);
        double totalQuantityToRemove = Quantity * packsToRemove;

        if (item.Quantity < totalQuantityToRemove)
        {
            System.Windows.MessageBox.Show("Not enough stock to remove.", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            return;
        }

        item.Quantity -= totalQuantityToRemove;

        // Remove the exact expiration dates (oldest first)
        for (int i = 0; i < packsToRemove; i++)
        {
            var dateToRemove = item.ExpirationDates.First(d => d.HasValue && d.Value.Date == ExpirationDate.Value.Date);
            item.ExpirationDates.Remove(dateToRemove);
        }

        // Remove item entirely if no quantity and no expiration dates
        if (item.Quantity <= 0 && item.ExpirationDates.Count == 0)
        {
            _parent?.AllInventoryItems.Remove(item);
        }

        // Update filter so UI reflects changes
        ApplyFilter();

        // Success message
        System.Windows.MessageBox.Show("Stock-out successful!", "Success", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);

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
