using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POS_APP.ViewModels.DashboardViewModels;
using POS_APP.Views.DashboardViews;
using POS_APP.Views.LoginViews;
using System.Windows;

namespace POS_APP.ViewModels.LoginViewModels
{
    public partial class BranchLoginViewModel(MainWindowViewModel mainVm) : ObservableObject
    {
        private readonly MainWindowViewModel _mainVm = mainVm;

        [ObservableProperty]
        private string? _branchName;

        [ObservableProperty]
        private string? _branchId;

        [ObservableProperty]
        private string? _password;

        [RelayCommand]
        private void Login()
        {
            if (CanLogin())
            {
                MessageBox.Show("Login successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                _mainVm.CurrentView = new BranchDashboardView
                {
                    DataContext = new BranchDashboardViewModel(_mainVm)
                };
            }
            else
            {
                MessageBox.Show("Please enter both branch name and password.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private bool CanLogin() => !string.IsNullOrWhiteSpace(BranchName) && !string.IsNullOrWhiteSpace(BranchId) && !string.IsNullOrWhiteSpace(Password);
    }
}