using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POS_APP.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace POS_APP.ViewModels.InventoryViewModels
{
    public partial class InvStockInViewModel : ObservableObject
    {
        private readonly InventoryViewModel _parent;

        public ObservableCollection<InventoryItem> AllInventoryItems => _parent?.AllInventoryItems ?? new();
        public ObservableCollection<string> ItemCategories => _parent?.ItemCategories ?? new();
        public ObservableCollection<string> UnitMeasurementTypes => _parent?.UnitMeasurementTypes ?? new();
        public ObservableCollection<string> Sources => _parent?.Sources ?? new();
        public ObservableCollection<string> SourceNames => _parent?.SourceNames ?? new();

        public InvStockInViewModel(InventoryViewModel parent)
        {
            _parent = parent;

            SelectedCategory = ItemCategories.FirstOrDefault() ?? string.Empty;
            SelectedUnitType = UnitMeasurementTypes.FirstOrDefault() ?? string.Empty;
            NewItemName = string.Empty;
        }

        [ObservableProperty] private string selectedCategory;
        [ObservableProperty] private string selectedUnitType;
        [ObservableProperty] private string newItemName;
        [ObservableProperty] private double newPackQuantity;
        [ObservableProperty] private double newQuantity;
        [ObservableProperty] private double newUnitCost;
        [ObservableProperty] private DateTime? newExpirationDate;

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
            // Validate
            if (string.IsNullOrWhiteSpace(NewItemName) ||
        string.IsNullOrWhiteSpace(SelectedCategory) ||
        NewPackQuantity <= 0 ||
        NewQuantity <= 0)
                return;

            var item = AllInventoryItems.FirstOrDefault(i =>
                i.Name.Equals(NewItemName.Trim(), StringComparison.OrdinalIgnoreCase) &&
                i.Category == SelectedCategory);

            if (item == null)
            {
                item = new InventoryItem
                {
                    Name = NewItemName.Trim(),
                    Category = SelectedCategory,
                    Unit = SelectedUnitType,
                    Threshold = 0,
                    StockStatus = ""
                };
                AllInventoryItems.Add(item);
            }

            // Add packs; DO NOT assign to item.Quantity
            for (int i = 0; i < (int)NewPackQuantity; i++)
            {
                item.Packs.Add(new ItemPack
                {
                    PackNumber = item.Packs.Count + 1,
                    ExpirationDate = NewExpirationDate,
                    PackQuantity = NewQuantity
                });
            }

            // Update item-level status
            item.StockStatus = item.Quantity >= item.Threshold ? "Good Stock" : "Low Stock";

            System.Windows.MessageBox.Show(
                $"{NewItemName} stocked in successfully!\n" +
                $"Item's New Total Packs : {item.PacksCount}\n" +
                $"Item's New Total Quantity : {item.QuantityDisplay}\n" +
                $"Status: {item.StockStatus}",
                "Success",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Information);

            // Reset form
            ClearForm();
        }

        private void ClearForm()
        {
            NewItemName = string.Empty;
            NewPackQuantity = 0;
            NewQuantity = 0;
            NewUnitCost = 0;
            NewExpirationDate = null;
        }

        [RelayCommand]
        private void Back()
        {
            _parent?.ShowDashboardCommand.Execute(null);
        }
    }
}
