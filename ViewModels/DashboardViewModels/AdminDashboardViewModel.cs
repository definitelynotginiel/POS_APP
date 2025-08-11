using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POS_APP.ViewModels.LoginViewModels;
using POS_APP.Views.DashboardViews;
using POS_APP.Views.SidebarViews;


namespace POS_APP.ViewModels.DashboardViewModels
{
    public partial class AdminDashboardViewModel : ObservableObject // Ensure the class inherits from ObservableObject
    {
        private readonly MainWindowViewModel _mainVm;

        public AdminDashboardViewModel(MainWindowViewModel mainVm)
        {
            _mainVm = mainVm;
            SelectedMenu = "Point of Sales";
            CurrentView = new POSView();     // Set default view
        }

        public List<string> MenuItems { get; } = new()
                {
                    "Point of Sales",
                    "Reports",
                    "Inventory Management",
                    "Manage Accounts",
                    "Data Management",
                    "System Configuration",
                    "Menu Configuration",
                    "Audit of Logs",
                    "Promos",
                    "Services",
                };

        [ObservableProperty]
        private object? _currentView;

        [ObservableProperty]
        private string? _selectedMenu;

        partial void OnSelectedMenuChanged(string? value)
        {
            CurrentView = value switch
            {
                "Point of Sales" => new POSView(),
                "Inventory Management" => new InventoryView(),
                // TODO: Add other cases here...
                _ => new POSView(),
            };
        }

        [RelayCommand]
        private void Logout()
        {
            _mainVm.CurrentView = new BranchDashboardView
                {
                DataContext = new BranchDashboardViewModel(_mainVm)
            };

        }
    }
}
