using System.Collections.ObjectModel;
using System.Linq;

namespace POS_APP.Models
{
    public class InventoryItem
    {
        public required string Name { get; set; }
        public required string Category { get; set; }
        public required string Unit { get; set; }

        public string StockStatus { get; set; } = "Good";
        public double Threshold { get; set; } = 0;

        // Packs hold per-pack quantity + expiration
        public ObservableCollection<ItemPack> Packs { get; set; } = new();

        public int PacksCount => Packs?.Count ?? 0;

        // ⛔ Quantity is computed (read-only). Don’t assign to it anywhere.
        public double Quantity => Packs?.Sum(p => p.PackQuantity) ?? 0;

        public string QuantityDisplay => $"{Quantity} {Unit}";
    }

    public class ItemPack
    {
        public int PackNumber { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public double PackQuantity { get; set; }
    }
}
