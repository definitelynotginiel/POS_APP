using POS_APP.ViewModels.LoginViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS_APP.ViewModels.DashboardViewModels
{
    internal class ManagerDashboardViewModel
    {
        private MainWindowViewModel mainVm;

        public ManagerDashboardViewModel(MainWindowViewModel mainVm)
        {
            this.mainVm = mainVm;
        }
    }
}
