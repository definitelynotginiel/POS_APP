using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POS_APP.ViewModels.LoginViewModels;
using POS_APP.Views.DashboardViews;
using POS_APP.Views.SidebarViews;
using System.Windows.Threading;

namespace POS_APP.ViewModels.DashboardViewModels
{
    public partial class AdminDashboardViewModel : ObservableObject
    {
        private readonly MainWindowViewModel _mainVm;

        public AdminDashboardViewModel(MainWindowViewModel mainVm)
        {
            _mainVm = mainVm;
            SelectedMenu = MenuItems.First(m => m.Name == "Point of Sales");
            CurrentView = new POSView();
            Clock();
        }

        public List<SidebarItem> MenuItems { get; } =
        [
            new SidebarItem { Name = "Point of Sales", Icon = "\uf54e" },      // shopping-cart
            new SidebarItem { Name = "Reports", Icon = "\ue0e3" },             // bar-chart
            new SidebarItem { Name = "Inventory", Icon = "\uf494" },           // archive
            new SidebarItem { Name = "Manage Accounts", Icon = "\uf0c0" },     // users
            new SidebarItem { Name = "Data Management", Icon = "\ue185" },     // folder
            new SidebarItem { Name = "System Configuration", Icon = "\uf013" }, // cog
            new SidebarItem { Name = "Menu Configuration", Icon = "\uf03a" },  // list-alt
            new SidebarItem { Name = "Audit of Logs", Icon = "\uf02d" },       // clipboard-list
            new SidebarItem { Name = "Promos", Icon = "\uf06b" },              // tags
            new SidebarItem { Name = "Services", Icon = "\uf4c4" },            // hands-holding (or use "\uf0f1" for cog if not available)
        ];

        public class SidebarItem
        {
            public string Name { get; set; } = string.Empty;
            public string Icon { get; set; } = string.Empty; // Unicode character
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