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
using GymManagementUserControls;
using GymManagementHILogic;

namespace GymManagement
{
  public class MainWindowUpdates
  {
    MainWindow main_window;
    Dashboard dashboard = new Dashboard();
    RegistrationPage reg_page = new RegistrationPage();
    ProfilePage prof_page = new ProfilePage();
    TraineeListing trainee_listing = new TraineeListing();
    PaymentListing payment_listing = new PaymentListing();
    string current_comp;

    public MainWindowUpdates(MainWindow main_window)
    {
      this.main_window = main_window;
    }

    private void UnloadCurrentComponent()
    {
      switch ((string)current_comp)
      {
        case "Dashboard":
          dashboard.UnloadState();
          break;
        case "RegistrationPage":
          reg_page.UnloadState();
          break;
        case "ProfilePage":
          prof_page.UnloadState();
          break;
        case "Trainees":
          trainee_listing.UnloadState();
          break;
        case "Payments":
          payment_listing.UnloadState();
          break;
      }
    }

    public void UpdateMainBody(string comp, AppState app_state)
    {
      if (current_comp != null)
        UnloadCurrentComponent();
      main_window.MainBody.Children.Clear();
      UserControl usc = new UserControl();
      current_comp = comp;

      switch (comp)
      {
        case "Dashboard":
          usc = dashboard.LoadState((DashboardState)app_state);
          break;
        case "RegistrationPage": 
          usc = reg_page.LoadState((TraineeRegistrationState)app_state);
          break;
        case "ProfilePage":
          usc = prof_page.LoadState((ProfilePageState)app_state);
          break;
        case "Trainees":
          usc = trainee_listing.LoadState((TraineeListingState)app_state);
          break;
        case "Payments":
          usc = payment_listing.LoadState((PaymentListingState)app_state);
          break;
      }

      main_window.MainBody.Children.Add(usc);
    }

    public void UpdateListViewMenu(int menu_index)
    {
      main_window.ListViewMenu.SelectionChanged -= main_window.ListViewMenu_SelectionChanged;
      main_window.ListViewMenu.SelectedIndex = menu_index;
      main_window.ListViewMenu.SelectionChanged += main_window.ListViewMenu_SelectionChanged;
    }
  }
}
