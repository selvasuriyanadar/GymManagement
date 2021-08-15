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
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class Dashboard : UserControl
  {
    DashboardState? dashboard_state;

    public Dashboard()
    {
      InitializeComponent();
    }

    public Dashboard LoadState(DashboardState dashboard_state)
    {
      UnloadState();
      this.dashboard_state = dashboard_state;
      return this;
    }

    public void UnloadState()
    {
      if (dashboard_state != null)
      {
        dashboard_state = null;
      }
    }

    ///Stateful Command

    private void OnRegisterButtonClick(object sender, RoutedEventArgs e)
    {
      dashboard_state.openRegistrationPage();
    }
  }
}
