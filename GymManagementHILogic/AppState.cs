using System;
using GymManagementDataModel;
using GymManagementLogic;
using System.Collections.Generic;

namespace GymManagementHILogic
{
  public class AppState
  {
    protected AppStoreManager appStoreManager;
    protected OpenPage openPage;

    public delegate void OpenPage(string page, AppState appState);

    public AppState(AppStoreManager appStoreManager, OpenPage openPage)
    {
      this.appStoreManager = appStoreManager;
      this.openPage = openPage;
    }

    public virtual bool Match(AppState app_state)
    {
      return true;
    }

    public virtual void Destroy() {}
  }

  public class AppStateManager
  {
    Dictionary<string, AppState?> menu_states = new Dictionary<string, AppState?>() {
      { "Dashboard", null },
      { "Trainees", null },
      { "Payments", null },
      { "Notifications", null }
    };

    Dictionary<string, Dictionary<string, AppState?>> page_states = new Dictionary<string, Dictionary<string, AppState?>>() {
      { "Dashboard",
        new Dictionary<string, AppState?>(){
          { "RegistrationPage", null },
          { "ProfilePage", null },
          { "PaymentPage", null },
          { "PrintBillPage", null }
        }
      },
      { "Trainees",
        new Dictionary<string, AppState?>(){
          { "RegistrationPage", null },
          { "ProfilePage", null },
          { "PaymentPage", null },
          { "PrintBillPage", null }
        }
      },
      { "Payments",
        new Dictionary<string, AppState?>(){
          { "RegistrationPage", null },
          { "ProfilePage", null },
          { "PaymentPage", null },
          { "PrintBillPage", null }
        }
      },
      { "Notifications",
        new Dictionary<string, AppState?>(){
          { "RegistrationPage", null },
          { "ProfilePage", null },
          { "PaymentPage", null },
          { "PrintBillPage", null }
        }
      }
    };

    public void SetState(Navigation nav, AppState app_state)
    {
      AppState app_state_cache = GetState(nav);
      if (app_state_cache != null)
      {
        if (app_state_cache.Match(app_state))
        {
          return;
        }
        else
        {
          app_state_cache.Destroy();
        }
      }

      if (nav.GetCurrentNodeType() == Navigation.NodeType.Menu)
        menu_states[nav.GetActiveComponentName()] = app_state;
      else
        page_states[nav.GetCurrentMenu()][nav.GetActiveComponentName()] = app_state;
    }

    public AppState? GetState(Navigation nav)
    {
      if (nav.GetCurrentNodeType() == Navigation.NodeType.Menu)
        return menu_states[nav.GetActiveComponentName()];
      else
        return page_states[nav.GetCurrentMenu()][nav.GetActiveComponentName()];
    }

    public bool DoesCacheExist(Navigation nav)
    {
      return GetState(nav) != null;
    }
  }
}
