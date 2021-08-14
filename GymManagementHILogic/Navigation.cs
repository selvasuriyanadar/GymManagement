using System;
using GymManagementDataModel;
using GymManagementLogic;
using System.Collections.Generic;

namespace GymManagementHILogic
{
  public class Navigation
  {
    public enum NodeType {
      Menu,
      Page
    }

    string menu;
    Dictionary<string, string?> pages = new Dictionary<string, string?>() {
      { "Dashboard", null },
      { "Trainees", null },
      { "Payments", null },
      { "Notifications", null }
    };

    List<string> menu_items = new List<string>(new string[] { "Dashboard", "Trainees", "Payments", "Notifications" });

    public void NavigateToMenu(string menu)
    {
      this.menu = menu;
    }

    public void OpenPage(string page)
    {
      pages[menu] = page;
    }

    public void ClearOpenPage()
    {
      pages[menu] = null;
    }

    public string GetCurrentMenu()
    {
      return menu;
    }

    public int GetCurrentMenuIndex()
    {
      return menu_items.IndexOf(menu);
    }

    public NodeType GetCurrentNodeType()
    {
      if (pages[menu] != null)
        return NodeType.Page;
      else
        return NodeType.Menu;
    }

    public (string, string?) GetActiveComponent()
    {
      if (GetCurrentNodeType() == NodeType.Page)
      {
        return (menu, pages[menu]);
      }
      else
      {
        return (menu, null);
      }
    }

    public string GetActiveComponentName()
    {
      if (GetCurrentNodeType() == NodeType.Page)
      {
        return pages[menu];
      }
      else
      {
        return menu;
      }
    }

    public (string, string?) BackupState()
    {
      return GetActiveComponent();
    }

    public void RestoreState((string, string?) comp)
    {
      NavigateToMenu(comp.Item1);
      if (comp.Item2 != null)
        OpenPage(comp.Item2);
      else
        ClearOpenPage();
    }
  }
}
