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
        public InventoryViewModel()
        { 

            SelectedCategory = ItemCategories.FirstOrDefault() ?? string.Empty;
            SelectedUnitType = UnitMeasurementTypes.FirstOrDefault() ?? string.Empty;
            NewItemName = string.Empty;

            // --- Sample inventory items ---
            AllInventoryItems = new ObservableCollection<InventoryItem>
    {
        new InventoryItem
        {
            Name = "Sugar",
            Category = "Food Ingredients",
            Quantity = 50,
            Unit = "kg",
            StockStatus = "Available",
            ExpirationDates = new ObservableCollection<DateTime?>
            {
                DateTime.Today.AddDays(30),
                DateTime.Today.AddDays(60)
            }
        },
        new InventoryItem
        {
            Name = "Flour",
            Category = "Food Ingredients",
            Quantity = 100,
            Unit = "kg",
            StockStatus = "Available",
            ExpirationDates = new ObservableCollection<DateTime?>
            {
                DateTime.Today.AddDays(15),
                DateTime.Today.AddDays(45)
            }
        },
        new InventoryItem
        {
            Name = "Coffee Powder",
            Category = "Beverage Ingredients",
            Quantity = 25,
            Unit = "kg",
            StockStatus = "Available",
            ExpirationDates = new ObservableCollection<DateTime?>
            {
                DateTime.Today.AddDays(10),
                DateTime.Today.AddDays(40)
            }
        },
        new InventoryItem
        {
            Name = "Plastic Cups",
            Category = "Disposable Utensils",
            Quantity = 200,
            Unit = "pcs",
            StockStatus = "Available",
            ExpirationDates = new ObservableCollection<DateTime?>()
        },
        new InventoryItem
        {
            Name = "Burger Wrapper",
            Category = "Food Packaging",
            Quantity = 500,
            Unit = "pcs",
            StockStatus = "Available",
            ExpirationDates = new ObservableCollection<DateTime?>()
        }
    };
            // Initialize child view
            CurrentInventoryView = new InvMonitorDashboardViewModel(this);
        }
        public ObservableCollection<InventoryItem> AllInventoryItems { get; set; } = new();

        public ObservableCollection<string> ItemCategories { get; set; } = ["Food Ingredients", "Beverage Ingredients",
            "Disposable Utensils", "Food Packaging", "Other" ];
        public ObservableCollection<string> UnitMeasurementTypes { get; set; } = ["pcs", "kg", "g", "L"];
        public ObservableCollection<string> Sources { get; set; } = ["Supplier A", "Supplier B"];
        public ObservableCollection<string> SourceNames { get; set; } = ["Source 1", "Source 2"];
        public ObservableCollection<string> StockOutReasons { get; set; } = ["Damaged", "Expired", "Sold", "Other"];

        [ObservableProperty]
        private string selectedCategory;

        [ObservableProperty]
        private string selectedUnitType;

        [ObservableProperty] 
        private string newItemName;

        [ObservableProperty] 
        private double newPackQuantity;

        [ObservableProperty] 
        private double newQuantity;

        [ObservableProperty] 
        private double newUnitCost;

        [ObservableProperty] 
        private DateTime? newExpirationDate;

        partial void OnSelectedCategoryChanged(string value) => OnPropertyChanged(nameof(StockInSummary));
        partial void OnNewItemNameChanged(string value) => OnPropertyChanged(nameof(StockInSummary));
        partial void OnNewPackQuantityChanged(double value) => OnPropertyChanged(nameof(StockInSummary));
        partial void OnNewQuantityChanged(double value) => OnPropertyChanged(nameof(StockInSummary));
        partial void OnSelectedUnitTypeChanged(string value) => OnPropertyChanged(nameof(StockInSummary));
        partial void OnNewUnitCostChanged(double value) => OnPropertyChanged(nameof(StockInSummary));
        partial void OnNewExpirationDateChanged(DateTime? value) => OnPropertyChanged(nameof(StockInSummary));

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

        [ObservableProperty]
        private object? currentInventoryView;

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

        [RelayCommand]
        private void ShowExpireTrack()
        {
            CurrentInventoryView = new InvExpirationTrackViewModel(this);
        }
    }
}
