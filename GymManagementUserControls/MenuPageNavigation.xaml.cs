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
using GymManagementDataModel;
using GymManagementHILogic;

namespace GymManagementUserControls
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MenuPageNavigation : UserControl
  {
    MenuPageNavigationState? menuPageNavigationState;
    MenuPageNavigationUpdates menuPageNavigationUpdates;


    public MenuPageNavigation()
    {
      InitializeComponent();

      menuPageNavigationUpdates = new MenuPageNavigationUpdates(this);
    }

    public MenuPageNavigation LoadState(MenuPageNavigationState menuPageNavigationState)
    {
      UnloadState();
      this.menuPageNavigationState = menuPageNavigationState;
      this.menuPageNavigationState.LoadMenuPageNavigationUpdates(menuPageNavigationUpdates);
      return this;
    }

    public void UnloadState()
    {
      if (menuPageNavigationState != null)
      {
        menuPageNavigationState.UnloadMenuPageNavigationUpdates();
        menuPageNavigationState = null;
      }
    }

    ///Controller + Events

    public void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      var result = ((ListViewItem)((ListView)sender).SelectedItem).Name;

      ((ListView)sender).SelectionChanged -= ListViewMenu_SelectionChanged;
      ((ListView)sender).SelectedItem = null;
      ((ListView)sender).SelectionChanged += ListViewMenu_SelectionChanged;

      menuPageNavigationState.Navigate(result);

      e.Handled = true;
    }

    // triggers when a selected menu item is clicked
    private void OnListViewMenuItemClicked(object sender, MouseButtonEventArgs e)
    {
      var item = sender as ListViewItem;
      if (item != null && item.IsSelected)
      {
        menuPageNavigationState.ReturnToRoot();
      }
    }

    ///Stateless Command

    private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
    {
      ButtonCloseMenu.Visibility = Visibility.Visible;
      ButtonOpenMenu.Visibility = Visibility.Collapsed;
    }

    private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
    {
      ButtonCloseMenu.Visibility = Visibility.Collapsed;
      ButtonOpenMenu.Visibility = Visibility.Visible;
    }
  }
}
