using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POS_APP.Models;
using POS_APP.ViewModels.InventoryViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace POS_APP.ViewModels
{
    public partial class InventoryViewModel : ObservableObject
    {
        [RelayCommand]
        private void ShowStockOut()
        {
            CurrentInventoryView = new InvStockOutViewModel(this);
        }

        [RelayCommand]
        private void ShowDashboard()
        {
            CurrentInventoryView = new InvMonitorDashboardViewModel(this);
        }

        [ObservableProperty]
        private object? currentInventoryView;

            partial void OnSelectedCategoryChanged(string value) => OnPropertyChanged(nameof(StockInSummary));
            partial void OnNewItemNameChanged(string value) => OnPropertyChanged(nameof(StockInSummary));
            partial void OnNewPackQuantityChanged(double value) => OnPropertyChanged(nameof(StockInSummary));
            partial void OnNewQuantityChanged(double value) => OnPropertyChanged(nameof(StockInSummary));
            partial void OnSelectedUnitTypeChanged(string value) => OnPropertyChanged(nameof(StockInSummary));
            partial void OnNewUnitCostChanged(double value) => OnPropertyChanged(nameof(StockInSummary));
            partial void OnNewExpirationDateChanged(DateTime? value) => OnPropertyChanged(nameof(StockInSummary));

        public ObservableCollection<InventoryItem> AllInventoryItems { get; set; } = new();

        [ObservableProperty]
        private string selectedCategory;

        [ObservableProperty]
        private string selectedUnitType;

        // New fields for stock-in form
        [ObservableProperty] private string newItemName;
        [ObservableProperty] private double newPackQuantity;
        [ObservableProperty] private double newQuantity;
        [ObservableProperty] private double newUnitCost;
        [ObservableProperty] private DateTime? newExpirationDate;

        public ObservableCollection<string> ItemCategories { get; set; } = [];
        public ObservableCollection<string> UnitMeasurementTypes { get; set; } = ["pcs", "kg", "g", "L"];
        public ObservableCollection<string> Sources { get; set; } = ["Supplier A", "Supplier B"];
        public ObservableCollection<string> SourceNames { get; set; } = ["Source 1", "Source 2"];

        public InventoryViewModel()
        {
            CurrentInventoryView = new InvMonitorDashboardViewModel(this);

            ItemCategories =
            [
                "Food Ingredients",
                "Beverage Ingredients",
                "Disposable Utensils",
                "Food Packaging",
                "Other"
            ];
            SelectedCategory = ItemCategories.FirstOrDefault() ?? string.Empty;
        }

        



        public string StockInSummary
        {
            get
            {
                if (string.IsNullOrWhiteSpace(NewItemName))
                    return string.Empty;

                double totalQuantity = NewPackQuantity * NewQuantity;

                return $@"
                {SelectedCategory}
                {NewItemName} ({totalQuantity} {SelectedUnitType})
                Cost: {NewUnitCost}
                Expiration Date: {NewExpirationDate?.ToShortDateString() ?? "-"}";
            }
        }


        [RelayCommand]
        private void StockInItem()
        {
            // Validate your inputs
            if (string.IsNullOrWhiteSpace(NewItemName) ||
                string.IsNullOrWhiteSpace(SelectedCategory) ||
                NewPackQuantity <= 0 ||
                NewQuantity <= 0)
                return;

            // Find or create
            var item = AllInventoryItems.FirstOrDefault(i =>
                i.Name.Equals(NewItemName.Trim(), StringComparison.OrdinalIgnoreCase) &&
                i.Category == SelectedCategory);

            if (item == null)
            {
                item = new InventoryItem
                {
                    Name = NewItemName.Trim(),
                    Category = SelectedCategory,
                    Quantity = 0, // we'll add to this
                    Unit = SelectedUnitType,
                    StockStatus = "Available"
                };
                AllInventoryItems.Add(item);
            }

            // Total quantity = per-pack quantity * number of packs
            double totalQuantityToAdd = NewQuantity * NewPackQuantity;
            item.Quantity += totalQuantityToAdd;

            // Add expiration entries (one per pack, keeping old ones)
            var dateToAdd = NewExpirationDate;
            for (int i = 0; i < (int)NewPackQuantity; i++)
            {
                item.ExpirationDates.Add(dateToAdd);
            }

            // Show success message
            System.Windows.MessageBox.Show("Stock-in successful!", "Success", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);

            // Clear form
            NewItemName = string.Empty;
            NewPackQuantity = 0;
            NewQuantity = 0;
            NewUnitCost = 0;
            NewExpirationDate = null;
        }
    }
}
