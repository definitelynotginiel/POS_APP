using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POS_APP.ViewModels.DashboardViewModels;
using POS_APP.Views.DashboardViews;
using System.Windows;

namespace POS_APP.ViewModels.LoginViewModels
{
    public partial class RoleLoginViewModel : ObservableObject  
    {

        private readonly MainWindowViewModel _mainVm;

        public RoleLoginViewModel(string role, MainWindowViewModel mainVm)
        {
            Role = role;
            _mainVm = mainVm;
            RoleTitle = $"{role.ToUpper()} LOGIN";
            RoleID = $"{role.ToUpper()} ID";
        }

        public string Role { get; }

        [ObservableProperty] private string _roleTitle;
        [ObservableProperty] private string _roleID;
        [ObservableProperty] private string? _username;
        [ObservableProperty] private string? _id;
        [ObservableProperty] private string? _password;

        [RelayCommand]
        private void Login()
        {
            if (CanLogin())
            {
                MessageBox.Show("Login successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                switch (Role.ToUpper())
                {
                    case "MANAGER":
                        _mainVm.CurrentView = new ManagerDashboardView
                        {
                            DataContext = new ManagerDashboardViewModel(_mainVm)
                        };
                        break;

                    case "SUPERVISOR":
                        _mainVm.CurrentView = new SupervisorDashboardView
                        {
                            DataContext = new SupervisorDashboardViewModel(_mainVm)
                        };
                        break;

                    /*case "CASHIER":
                        _mainVm.CurrentView = new CashierDashboardView
                        {
                            DataContext = new CashierDashboardViewModel(_mainVm)
                        };
                        break;

                    case "STAFF":
            _mainVm.CurrentView = new StaffDashboardView
            {
                DataContext = new StaffDashboardViewModel(_mainVm)
            };
            break;*/

                    default:
                        MessageBox.Show("Invalid role selected.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                }
            }
            else
            {
                MessageBox.Show("Please enter both username and password.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private bool CanLogin() => !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(RoleID) && !string.IsNullOrWhiteSpace(Password);

        

[RelayCommand]  
        public void Back()
        {
            _mainVm.CurrentView = new BranchDashboardView   
            {
                DataContext = new BranchDashboardViewModel(_mainVm)
            };
        }
    }
}
