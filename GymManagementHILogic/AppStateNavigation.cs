using System;
using GymManagementDataModel;
using GymManagementApi;
using GymManagementLogic;
using GymManagementDataStore;
using System.Collections.Generic;

namespace GymManagementHILogic
{
  public class AppStateNavigation
  {
    public Navigation navigation = new Navigation();
    public AppStateManager app_state_manager =  new AppStateManager();
    public delegate AppState? GetState(string name);
    public GetState GetMenuState;

    public AppStateNavigation(GetState GetMenuState)
    {
      this.GetMenuState = GetMenuState;
    }

    private void MakeSureMenuStateExists(string menu)
    {
      if (!app_state_manager.DoesCacheExist(navigation))
      {
        app_state_manager.SetState(navigation, GetMenuState(menu));
      }
    }

    public void Navigate(string menu)
    {
      navigation.NavigateToMenu(menu);
      if (navigation.GetCurrentNodeType() == Navigation.NodeType.Menu)
        MakeSureMenuStateExists(menu);
    }

    public void OpenPageWithinCurrentMenu(string page, AppState app_state)
    {
      navigation.OpenPage(page);
      app_state_manager.SetState(navigation, app_state);
    }

    public void ReturnToRoot()
    {
      navigation.ClearOpenPage();
    }
  }
}
