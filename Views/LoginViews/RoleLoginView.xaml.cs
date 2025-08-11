using POS_APP.ViewModels.LoginViewModels;
using System.Windows;
using System.Windows.Controls;


namespace POS_APP.Views.LoginViews
{
    /// <summary>
    /// Interaction logic for RoleLoginView.xaml
    /// </summary>
    public partial class RoleLoginView : UserControl
    {
        public RoleLoginView()
        {
            InitializeComponent();
        }
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is RoleLoginViewModel viewModel)
            {
                viewModel.Password = PasswordBox.Password;
            }
        }
    }
}
