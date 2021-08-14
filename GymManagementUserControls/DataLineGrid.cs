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
using GymManagementLogic;
using MaterialDesignThemes.Wpf;
using System.Globalization;

namespace GymManagementUserControls
{
  public abstract class DataLineGrid<T> : DataLineList<T>
  {
    System.Windows.Controls.Primitives.UniformGrid grid = new System.Windows.Controls.Primitives.UniformGrid();

    public DataLineGrid()
    {
      grid.VerticalAlignment = VerticalAlignment.Center;
      grid.HorizontalAlignment = HorizontalAlignment.Center;
    }

    protected abstract UIElement GetUiItem(T d);

    protected override void RemoveRange(int pos, int cnt)
    {
      base.RemoveRange(pos, cnt);
      grid.Children.RemoveRange(pos, cnt);
    }

    protected override void InsertRange(int pos, List<T> dl)
    {
      base.InsertRange(pos, dl);
      int i = pos;
      foreach (var d in dl)
      {
        grid.Children.Insert(i, GetUiItem(d));
        i++;
      }
    }

    protected UserControl GetParentUserControl(DependencyObject sub_elem)
    {
      while (!(sub_elem is UserControl))
      {
        sub_elem = LogicalTreeHelper.GetParent(sub_elem);
      }
      return (UserControl) sub_elem;
    }

    public System.Windows.Controls.Primitives.UniformGrid GetGrid()
    {
      return grid;
    }
  }
}
