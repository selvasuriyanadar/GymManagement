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
  public class MenuPageNavigationState : AppState
  {
    AppStateNavigation app_state_nav;

    MenuPageNavigationUpdates? menuPageNavigationUpdates;

    public MenuPageNavigationState(MenuPageNavigationUpdates menuPageNavigationUpdates) : base(new AppStoreManager())
    {
      app_state_nav = new AppStateNavigation(GetMenuState);
      app_state_nav.Navigate("Dashboard");
      
      LoadMenuPageNavigationUpdates(menuPageNavigationUpdates);
    }

    private void LoadMenuPageNavigationUpdates(MenuPageNavigationUpdates menuPageNavigationUpdates)
    {
      this.menuPageNavigationUpdates = menuPageNavigationUpdates;
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
      if (menuPageNavigationUpdates != null)
      {
        menuPageNavigationUpdates.UpdateListViewMenu(app_state_nav.navigation.GetCurrentMenuIndex());
        menuPageNavigationUpdates.UpdateMainBody(app_state_nav.navigation.GetActiveComponentName(), (AppState) app_state_nav.app_state_manager.GetState(app_state_nav.navigation));
      }
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
