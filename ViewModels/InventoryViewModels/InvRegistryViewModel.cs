using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POS_APP.Models;
using System.Collections.ObjectModel;

namespace POS_APP.ViewModels.InventoryViewModels;

public partial class InvRegistryViewModel : ObservableObject
{
    private readonly InventoryViewModel _parent;

    public ObservableCollection<InventoryItem> AllInventoryItems => _parent?.AllInventoryItems ?? new();
    public ObservableCollection<string> ItemCategories => _parent?.ItemCategories ?? new ObservableCollection<string>();

    [ObservableProperty]
    private ObservableCollection<InventoryItem> inventoryItems = new();
    public InvRegistryViewModel(InventoryViewModel parent)
    {
        _parent = parent;
        SelectedCategory = ItemCategories.FirstOrDefault() ?? string.Empty;
    }
    [ObservableProperty]
    private string selectedCategory;

    [ObservableProperty]
    private bool isEditPanelVisible;   // controls panel visibility

    [ObservableProperty]
    private InventoryItem? selectedForEdit;  // the item being edited

    [ObservableProperty]
    private double goodThreshold; // e.g. 100 (user input)

    partial void OnSelectedCategoryChanged(string value)
    {
        ApplyFilter();
    }
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
    [RelayCommand]
    private void EditStatus(InventoryItem? item)
    {
        if (item == null) return;

        SelectedForEdit = item;
        GoodThreshold = item.Threshold; // preload
        IsEditPanelVisible = true;
    }

    [RelayCommand]
    private void SaveStatus()
    {
        if (SelectedForEdit != null)
        {
            // Save the entered threshold into the item
            SelectedForEdit.Threshold = GoodThreshold;

            // Recompute the stock status
            SelectedForEdit.StockStatus =
                SelectedForEdit.Quantity >= SelectedForEdit.Threshold ? "Good Stock" : "Low Stock";
        }

        IsEditPanelVisible = false;
        SelectedForEdit = null;
    }



    [RelayCommand]
    private void CancelEdit()
    {
        IsEditPanelVisible = false;
        SelectedForEdit = null;
    }

    [RelayCommand]
    private void Back()
    {
        _parent?.ShowDashboardCommand.Execute(null);
    }
}



    

