using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POS_APP.ViewModels.LoginViewModels;
using POS_APP.Views.DashboardViews;
using POS_APP.Views.SidebarViews;
using System.Windows.Threading;


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
            Clock(); // Start the clock to update time and date
        }

        public List<string> MenuItems { get; } = new()
                {
                    "Point of Sales",
                    "Reports",
                    "Inventory",
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
                "Inventory" => new InventoryView(),
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
        [ObservableProperty] private string? _currentDate;
        [ObservableProperty] private string? _currentTime;
        [ObservableProperty] private string? _currentDay;
        private void Clock()
        {
            
            var dispatcherTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            dispatcherTimer.Tick += (_, _) =>
            {
                CurrentDay = DateTime.Now.ToString("dddd");
                CurrentDate = DateTime.Now.ToString("MMMM dd, yyyy");
                CurrentTime = DateTime.Now.ToString("hh:mm:ss tt");

            };
            dispatcherTimer.Start();
        }
    }
}
