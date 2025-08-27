using CommunityToolkit.Mvvm.ComponentModel;

public partial class ItemPacks : ObservableObject
{
    [ObservableProperty]
    private string itemName = string.Empty;

    [ObservableProperty]
    private string category = string.Empty;

    [ObservableProperty]
    private string unit = string.Empty;

    [ObservableProperty]
    private int packNumber;

    [ObservableProperty]
    private DateTime? expirationDate;

    [ObservableProperty]
    private string status = "Unknown";

    [ObservableProperty]
    private int daysRemaining;

    [ObservableProperty]
    private double packQuantity = 1; // quantity per pack

    public string QuantityDisplay => $"{PackQuantity} {Unit}";
}