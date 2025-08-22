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
            SelectedMenu = MenuItems.First(m => m.Name == "Point of Sales");

            CurrentView = new POSView();     // Set default view
            Clock(); // Start the clock to update time and date
        }

        public List<SidebarItem> MenuItems { get; } =
        [
            new SidebarItem { Name = "Point of Sales", Icon = "ShoppingCart" },
            new SidebarItem { Name = "Reports", Icon = "BarChart" },
            new SidebarItem { Name = "Inventory", Icon = "Archive" },
            new SidebarItem { Name = "Manage Accounts", Icon = "Users" },
            new SidebarItem { Name = "Data Management", Icon = "Folder" },
            new SidebarItem { Name = "System Configuration", Icon = "Cog" },
            new SidebarItem { Name = "Menu Configuration", Icon = "ListAlt" },
            new SidebarItem { Name = "Audit of Logs", Icon = "Clipboard" },
            new SidebarItem { Name = "Promos", Icon = "Tags" },
            new SidebarItem { Name = "Services", Icon = "HoldingHeartHand" },
        ];

        public class SidebarItem
        {
            public string Name { get; set; } = string.Empty;
            public string Icon { get; set; } = string.Empty; // FontAwesome key
        }

        [ObservableProperty]
        private object? _currentView;

        [ObservableProperty]
        private SidebarItem? _selectedMenu;

        partial void OnSelectedMenuChanged(SidebarItem? value)
        {
            if (value == null) return;

            CurrentView = value.Name switch
            {
                "Point of Sales" => new POSView(),
                "Inventory" => new InventoryView(),
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