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
  public class MenuPageNavigation : AppState
  {
    AppStateNavigation app_state_nav;

    MainWindowUpdates main_window_updates;

    public MenuPageNavigation(MainWindowUpdates main_window_updates) : base(new AppStoreManager())
    {
      app_state_nav = new AppStateNavigation(GetMenuState);
      app_state_nav.Navigate("Dashboard");
      
      LoadMainWindowUpdates(main_window_updates);
    }

    private void LoadMainWindowUpdates(MainWindowUpdates main_window_updates)
    {
      this.main_window_updates = main_window_updates;
      SyncState();
    }

    private void SyncState()
    {
      RenderComponent();
    }

    public AppState? GetMenuState(string name)
    {
      switch (name)
      {
        case "Dashboard":
          return new DashboardState(OpenPageWithinCurrentMenu, appStoreManager);
        case "Trainees":
          return new TraineeListingState(OpenPageWithinCurrentMenu, appStoreManager);
        case "Payments":
          return new PaymentListingState(appStoreManager);
      }
      return null;
    }

    private void RenderComponent()
    {
      main_window_updates.UpdateListViewMenu(app_state_nav.navigation.GetCurrentMenuIndex());
      main_window_updates.UpdateMainBody(app_state_nav.navigation.GetActiveComponentName(), (AppState) app_state_nav.app_state_manager.GetState(app_state_nav.navigation));
    }

    public void Navigate(string menu)
    {
      app_state_nav.Navigate(menu);

      RenderComponent();
    }

    public void ReturnToRoot()
    {
      app_state_nav.ReturnToRoot();

      RenderComponent();
    }

    public void OpenPageWithinCurrentMenu(string page, AppState app_state)
    {
      app_state_nav.OpenPageWithinCurrentMenu(page, app_state);

      RenderComponent();
    }
  }
}
