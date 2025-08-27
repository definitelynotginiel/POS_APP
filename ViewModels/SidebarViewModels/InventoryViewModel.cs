using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POS_APP.Models;
using POS_APP.ViewModels.InventoryViewModels;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;

namespace POS_APP.ViewModels
{
    public partial class InventoryViewModel : ObservableObject
    {
        public InventoryViewModel()
        {
            // --- Sample inventory items ---
            AllInventoryItems = new ObservableCollection<InventoryItem>
            {
                new InventoryItem
    {
        Name = "Sugar",
        Category = "Food Ingredients",
        Unit = "kg",
        Threshold = 0,
        Packs = new ObservableCollection<ItemPack>
        {
            new ItemPack { PackNumber = 1, ExpirationDate = DateTime.Today.AddDays(1),  PackQuantity = 20 },
            new ItemPack { PackNumber = 2, ExpirationDate = DateTime.Today.AddDays(60), PackQuantity = 20 },
            new ItemPack { PackNumber = 3, ExpirationDate = DateTime.Today.AddDays(60), PackQuantity = 10 }
        }
    },
                new InventoryItem
    {
        Name = "Salt",
        Category = "Food Ingredients",
        Unit = "kg",
        Threshold = 0,
        Packs = new ObservableCollection<ItemPack>
        {
            new ItemPack { PackNumber = 1, ExpirationDate = DateTime.Today.AddDays(-1),  PackQuantity = 2 },
            new ItemPack { PackNumber = 2, ExpirationDate = DateTime.Today.AddDays(-1), PackQuantity = 2 },
            new ItemPack { PackNumber = 3, ExpirationDate = DateTime.Today.AddDays(60), PackQuantity = 2 }
        }
    },
            };

            // Set initial stock status based on thresholds
            UpdateStockStatus();

            // Initialize default view
            CurrentInventoryView = new InvMonitorDashboardViewModel(this);
        }

        // All inventory items
        public ObservableCollection<InventoryItem> AllInventoryItems { get; set; } = new();

        // Categories
        public ObservableCollection<string> ItemCategories { get; set; } = new()
        {
            "Food Ingredients", "Beverage Ingredients", "Disposable Utensils", "Food Packaging", "Other"
        };

        public ObservableCollection<string> UnitMeasurementTypes { get; set; } = new() { "pcs", "kg", "g", "L" };
        public ObservableCollection<string> Sources { get; set; } = new() { "Supplier A", "Supplier B" };
        public ObservableCollection<string> SourceNames { get; set; } = new() { "Source 1", "Source 2" };
        public ObservableCollection<string> StockOutReasons { get; set; } = new() { "Damaged", "Expired", "Sold", "Other" };

        [ObservableProperty]
        private object? currentInventoryView;

        public void UpdateStockStatus()
        {
            foreach (var item in AllInventoryItems)
            {
                double totalQuantity = item.Packs?.Sum(p => p.PackQuantity) ?? 0;
                double threshold = item.Threshold > 0 ? item.Threshold : 1;

                item.StockStatus = totalQuantity >= threshold ? "Good Stock" : "Low Stock";

            }
        }


        // Commands to switch child views
        [RelayCommand]
        private void ShowStockIn() => CurrentInventoryView = new InvStockInViewModel(this);

        [RelayCommand]
        private void ShowStockOut() => CurrentInventoryView = new InvStockOutViewModel(this);

        [RelayCommand]
        private void ShowDashboard() => CurrentInventoryView = new InvMonitorDashboardViewModel(this);

        [RelayCommand]
        private void ShowExpireTrack() => CurrentInventoryView = new InvExpirationTrackViewModel(this);

        [RelayCommand]
        private void ShowRegistry() => CurrentInventoryView = new InvRegistryViewModel(this);
    }
}
