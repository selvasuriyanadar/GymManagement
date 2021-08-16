using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GymManagementHILogic;

namespace GymManagementUserControls
{
  public class DashboardState : AppState
  {
    public DashboardState(AppStoreManager appStoreManager, OpenPage openPage) : base(appStoreManager, openPage)
    {
    }

    public void openRegistrationPage()
    {
      openPage("RegistrationPage", new TraineeRegistrationState(appStoreManager, openPage, "Dashboard"));
    }
  }
}
