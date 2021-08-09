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
using GymManagementDataStore;
using GymManagementHILogic;

namespace GymManagementUserControls
{
  public class ListingItemsSyncPinner
  {
    public ((int, int), (int, int))? GetUpdateRange(List<long> old, List<long> fresh)
    {
      (int, int)? update_start = GetUpdateStartIndex((0, 0), old, fresh);
      if (update_start != null)
      {
        return GetUpdateRange(((int, int)) update_start, old, fresh);
      }
      return null;
    }

    private (int, int)? GetUpdateStartIndex((int, int) start_on, List<long> old, List<long> fresh)
    {
      for (int j = start_on.Item2; j < fresh.Count; j++)
      {
        var i = old.IndexOf(fresh[j], start_on.Item1);
        if (i >= 0)
        {
          return (i, j);
        }
      }
      return null;
    }

    private ((int, int), (int, int)) GetUpdateRange((int, int) update_start, List<long> old, List<long> fresh)
    {
      int i, j;
      for (i = update_start.Item1, j = update_start.Item2; (i < old.Count && j < fresh.Count); i++, j++)
      {
        if (old[i] != fresh[j])
        {
          break;
        }
      }
      return ((update_start.Item1, i), (update_start.Item2, j));
    }
  }

  public class ListingItemsSynced<T>
  {
    public delegate UIElement GetUiItem(T d);
    public delegate long GetDataKey(T d);
    ListingItemsSyncPinner listing_items_sync_pinner = new ListingItemsSyncPinner();

    List<long> keys = new List<long>();
    List<UIElement> ui_collection = new List<UIElement>();
    GetUiItem getUiItem;
    GetDataKey getDataKey;

    public ListingItemsSynced(GetUiItem getUiItem, GetDataKey getDataKey)
    {
      this.getDataKey = getDataKey;
      this.getUiItem = getUiItem;
    }

    private List<long> GetKeys(List<T> data)
    {
      var result = new List<long>();
      foreach (var d in data)
      {
        result.Add(getDataKey(d));
      }
      return result;
    }

    public List<UIElement> GetUiItems(List<T> data)
    {
      var result = new List<UIElement>();
      foreach (var d in data)
      {
        result.Add(getUiItem(d));
      }
      return result;
    }

    public void Sync(List<T> data)
    {
      var new_keys = GetKeys(data);
      var update_rng = listing_items_sync_pinner.GetUpdateRange(keys, new_keys);

      if (update_rng != null)
      {
        Remove((((((int, int), (int, int)))update_rng).Item1.Item2, keys.Count));
        Remove((0, ((((int, int), (int, int)))update_rng).Item1.Item1));

        AddToStart((0, ((((int, int), (int, int)))update_rng).Item2.Item1), data);
        AddToEnd((((((int, int), (int, int)))update_rng).Item2.Item2, new_keys.Count), data);
      }
      else
      {
        Reset(data);
      }
    }

    private void AddDataToUI(List<T> data)
    {
      foreach (var e in GetUiItems(data))
      {
        ui_collection.Add(e);
      }
      keys.AddRange(GetKeys(data));
    }

    private void ReverseAddDataToUI(List<T> data)
    {
      var new_ui_items = GetUiItems(data);
      for (int i = 0; i < new_ui_items.Count; i++)
      {
        ui_collection.Insert(i, new_ui_items[i]);
      }
      keys.InsertRange(0, GetKeys(data));
    }

    private void Reset(List<T> data)
    {
      ui_collection.Clear();
      keys.Clear();
      AddDataToUI(data);
    }

    private void Remove((int, int) rng)
    {
      ui_collection.RemoveRange(rng.Item1, rng.Item2 - rng.Item1);
      keys.RemoveRange(rng.Item1, rng.Item2 - rng.Item1);
    }

    private void AddToEnd((int, int) rng, List<T> data)
    {
      var new_data = data.GetRange(rng.Item1, rng.Item2 - rng.Item1);
      AddDataToUI(new_data);
    }

    private void AddToStart((int, int) rng, List<T> data)
    {
      var new_data = data.GetRange(rng.Item1, rng.Item2 - rng.Item1);
      ReverseAddDataToUI(new_data);
    }

    public long GetKeyAtIndex(int i)
    {
      return keys[i];
    }

    public List<UIElement> GetUiItems()
    {
      return ui_collection;
    }
  }
}
