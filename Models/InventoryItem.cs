    using System.Collections.ObjectModel;

    namespace POS_APP.Models
    {
        public class InventoryItem
        {
            public required string Name { get; set; }
            public required string Category { get; set; }

            public int Packs => ExpirationDates?.Count ?? 0; // total packs
            public required string StockStatus { get; set; }

            // Track expiration per pack internally
            public ObservableCollection<DateTime?> ExpirationDates { get; set; } = new();
            public double Quantity { get; set; }      // total quantity
            public required string Unit { get; set; }

            public string QuantityDisplay => $"{Quantity} {Unit}";
        }
    }
