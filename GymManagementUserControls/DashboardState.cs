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
    public OpenPage openPage;

    public DashboardState(OpenPage openPage, AppStoreManager appStoreManager) : base(appStoreManager)
    {
      this.openPage = openPage;
    }

    public void openRegistrationPage()
    {
      openPage("RegistrationPage", new TraineeRegistrationState(openPage, appStoreManager, "Dashboard"));
    }
  }
}
