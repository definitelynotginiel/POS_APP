using POS_APP.ViewModels.LoginViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS_APP.ViewModels.DashboardViewModels
{
    internal class SupervisorDashboardViewModel
    {
        private MainWindowViewModel mainVm;

        public SupervisorDashboardViewModel(MainWindowViewModel mainVm)
        {
            this.mainVm = mainVm;
        }
    }
}
