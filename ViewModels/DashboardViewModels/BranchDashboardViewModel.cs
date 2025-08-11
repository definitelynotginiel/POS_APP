using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POS_APP.ViewModels.LoginViewModels;
using POS_APP.Views.DashboardViews;
using POS_APP.Views.LoginViews;
using System.Windows.Threading;

namespace POS_APP.ViewModels.DashboardViewModels
{
    public partial class BranchDashboardViewModel : ObservableObject
    {
        private readonly MainWindowViewModel _mainVm;

        public BranchDashboardViewModel(MainWindowViewModel mainVm)
        {
            _mainVm = mainVm;
            Clock();
        }
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
        [ObservableProperty] private string? _currentDate;
        [ObservableProperty] private string? _currentTime;
        [ObservableProperty] private string? _currentDay;

        [RelayCommand]
        public void ShowRoleLogin(string role)
        {
            _mainVm.CurrentView = new RoleLoginView
            {
                DataContext = new RoleLoginViewModel(role, _mainVm)
            };
        }

        [RelayCommand]

        public void AdminDashboard()
        {
            _mainVm.CurrentView = new AdminDashboardView
            {
                DataContext = new AdminDashboardViewModel(_mainVm)
            };
        }
    }
}
