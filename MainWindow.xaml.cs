using POS_APP.ViewModels.LoginViewModels;

using System.Windows;

    
namespace POS_APP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            WindowState = WindowState.Maximized;

            DataContext = new MainWindowViewModel();
        }
    }
}