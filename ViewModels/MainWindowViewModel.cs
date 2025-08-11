
using CommunityToolkit.Mvvm.ComponentModel;
using POS_APP.ViewModels.LoginViewModels; 
using POS_APP.Views.LoginViews;
using System.Windows;
using System.Windows.Controls;

namespace POS_APP.ViewModels.LoginViewModels
{
    public partial class MainWindowViewModel: ObservableObject
    {
        

        [ObservableProperty]
        private object? _currentView;
    
        public MainWindowViewModel()
        {


            CurrentView = new BranchLoginViewModel(this);

        }
    }   
}
