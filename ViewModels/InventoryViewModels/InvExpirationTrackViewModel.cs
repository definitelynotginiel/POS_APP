using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POS_APP.Models;
using POS_APP.ViewModels;
using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace POS_APP.ViewModels.InventoryViewModels;
public partial class InvExpirationTrackViewModel : ObservableObject
{
    private readonly InventoryViewModel _parent;

    public ObservableCollection<string> ItemCategories => _parent?.ItemCategories ?? new ObservableCollection<string>();

    public InvExpirationTrackViewModel(InventoryViewModel parent)
    {
        _parent = parent;

        SelectedCategory = ItemCategories.FirstOrDefault() ?? string.Empty;

        LoadItemPacks();
    }

    [ObservableProperty]
    private ObservableCollection<ItemPacks> inventoryItems = new();

    [ObservableProperty]
    private ItemPacks? selectedInventoryItem;

    [ObservableProperty]
    private string selectedCategory;

    private void LoadItemPacks()
    {
        if (_parent.AllInventoryItems == null) return;

        var packs = new ObservableCollection<ItemPacks>();

        foreach (var item in _parent.AllInventoryItems)
        {
            int i = 1;
            foreach (var date in item.ExpirationDates)
            {
                packs.Add(new ItemPacks
                {
                    ItemName = item.Name,
                    Category = item.Category,
                    Unit = item.Unit,
                    PackNumber = i++,
                    ExpirationDate = date,
                    Status = GetStatus(date),
                    PackQuantity = item.Quantity / item.ExpirationDates.Count // divide evenly
                });
            }
        }

        InventoryItems = packs;
    }


    private string GetStatus(DateTime? date)
    {
        if (date == null) return "Unknown";
        var today = DateTime.Today;

        if (date <= today) return "Expired";
        if ((date - today).Value.TotalDays <= 7) return "Near Expiration";
        return "Good";
    }

    [RelayCommand]
    private void StockOut(ItemPacks? pack)
    {
        if (pack == null) return;

        InventoryItems.Remove(pack);

        var item = _parent.AllInventoryItems
            .FirstOrDefault(i => i.Name == pack.ItemName && i.Category == pack.Category);

        if (item != null && pack.ExpirationDate != null)
        {
            // Remove the expiration date
            item.ExpirationDates.Remove(pack.ExpirationDate);

            // Subtract the quantity of this pack from total
            item.Quantity = Math.Max(0, item.Quantity - pack.PackQuantity);

            // If quantity is 0 and no more expiration dates, remove the item entirely
            if (item.Quantity <= 0 && item.ExpirationDates.Count == 0)
            {
                _parent.AllInventoryItems.Remove(item);
            }
        }
    }




    [RelayCommand]
    private void Back() => _parent.ShowDashboardCommand.Execute(null);
}
