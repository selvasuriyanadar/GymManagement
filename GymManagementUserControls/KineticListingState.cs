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
using System.Windows.Threading;
using GymManagementDataModel;
using GymManagementDataStore;
using GymManagementHILogic;

namespace GymManagementUserControls
{
  public class Scroller
  {
    private double verticalScrollPosition;
    private long scrollItemPosition;
    private double scrollOffset = 0;

    public Scroller(DataLineData dld, ListingViewData viewData)
    {
      Reset(dld, viewData);
    }

    public long GetSP()
    {
      return scrollItemPosition;
    }

    public double GetVP()
    {
      return verticalScrollPosition;
    }

    public void Reset(DataLineData dld, ListingViewData viewData)
    {
      scrollItemPosition = dld.FetchPosition;
      verticalScrollPosition = GetScrollPosition(dld.FetchPosition, scrollItemPosition, viewData);
      scrollOffset = 0;
    }

    public void Set(long sp, DataLineData dld, ListingViewData viewData)
    {
      scrollItemPosition = sp;
      verticalScrollPosition = GetScrollPosition(dld.FetchPosition, scrollItemPosition, viewData) + scrollOffset;
    }

    public void Scroll(double vp, DataLineData dld, ListingViewData viewData)
    {
      verticalScrollPosition = vp;
      scrollItemPosition = GetScrollItemPosition(dld.FetchPosition, verticalScrollPosition, viewData);
      scrollOffset = GetScrollOffset(dld, viewData);
    }

    public void Restore(DataLineData dld, ListingViewData viewData)
    {
      var vp = GetScrollPosition(dld.FetchPosition, scrollItemPosition, viewData);
      verticalScrollPosition = vp + scrollOffset;
    }

    private double GetScrollOffset(DataLineData dld, ListingViewData viewData)
    {
      var vp = GetScrollPosition(dld.FetchPosition, scrollItemPosition, viewData);
      return verticalScrollPosition - vp;
    }

    private long GetScrollItemPosition(long fetchPosition, double verticalScrollPosition, ListingViewData vd)
    {
      var rows = (int) (verticalScrollPosition / ((double) vd.itemHeight));
      var visualScrollItemPosition = (rows * ((int) vd.itemsPerRow)) + 1;
      return fetchPosition - 1 + visualScrollItemPosition;
    }

    private double GetScrollPosition(long fetchPosition, long scrollItemPosition, ListingViewData vd)
    {
      var visualScrollItemPosition = scrollItemPosition - fetchPosition + 1;
      var rows = visualScrollItemPosition / ((int) vd.itemsPerRow);
      if ((visualScrollItemPosition % ((int) vd.itemsPerRow)) != 0)
      {
        rows += 1;
      }
      return (rows - 1) * ((double) vd.itemHeight);
    }
  }

  public class KineticListingState
  {
    public delegate DataLineData GetDataLineData();
    public delegate void RefreshStill(ListingViewData vd);
    public delegate void Refresh(long fetchPosition, ListingViewData vd);
    public delegate void IncLeft();
    public delegate void IncRight();

    string? itemsName; // maintained always valid !!!
    ListingViewData viewData; // maintained always valid !!!
    bool loaderState = false;
    bool expandSlider = true;
    Scroller scroller;

    KineticListingUpdates? kineticListingUpdates;
    GetDataLineData getDataLineData;
    RefreshStill refreshStill;
    Refresh refresh;
    IncLeft incLeft;
    IncRight incRight;

    public KineticListingState(
        string? itemsName,
        ListingViewData viewData,
        GetDataLineData getDataLineData,
        RefreshStill refreshStill,
        Refresh refresh,
        IncLeft incLeft,
        IncRight incRight
      )
    {
      if (viewData.Validate().Count != 0)
      {
        throw new ArgumentException("invalid view data.", "view data");
      }

      this.itemsName = itemsName;
      this.viewData = viewData;
      this.getDataLineData = getDataLineData;
      this.refreshStill = refreshStill;
      this.refresh = refresh;
      this.incLeft = incLeft;
      this.incRight = incRight;
      scroller = new Scroller(getDataLineData(), viewData);
    }

    public void LoadKineticListingUpdates(KineticListingUpdates kineticListingUpdates, System.Windows.Controls.Primitives.UniformGrid grid)
    {
      this.kineticListingUpdates = kineticListingUpdates;
      kineticListingUpdates.LoadGrid(grid, viewData);
      Sync();
      kineticListingUpdates.Initialise();
    }

    public void UnloadKineticListingUpdates()
    {
      kineticListingUpdates.UnloadGrid();
      kineticListingUpdates = null;
    }

    private void Sync()
    {
      if (kineticListingUpdates != null)
      {
        kineticListingUpdates.Sync(itemsName, loaderState, expandSlider, getDataLineData(), scroller.GetSP(), scroller.GetVP());
      }
    }

    private void SyncSliderStretch()
    {
      if (kineticListingUpdates != null)
      {
        kineticListingUpdates.SyncSliderStretch(expandSlider, getDataLineData(), scroller.GetSP());
      }
    }

    public void UpdateExpandSlider(bool expandSlider)
    {
      this.expandSlider = expandSlider;
      SyncSliderStretch();
    }

    public void UpdateScrollItemPosition(long scrollItemPosition)
    {
      DataLineData dld = getDataLineData();
      if (scrollItemPosition <= dld.FetchEndPosition && scrollItemPosition >= dld.FetchPosition)
      {
        scroller.Set(scrollItemPosition, getDataLineData(), viewData);
        Sync();
      }
      else if (scrollItemPosition == dld.FetchEndPosition + 1)
      {
        UpdateRightInc();
      }
      else if (scrollItemPosition == dld.FetchPosition - 1)
      {
        UpdateLeftInc();
      }
      else if (scrollItemPosition > dld.FetchEndPosition || scrollItemPosition < dld.FetchPosition)
      {
        refresh(scrollItemPosition, viewData);
        scroller.Reset(getDataLineData(), viewData);
        Sync();
      }
    }

    public void RefreshAtCurrentItemPosition()
    {
      refreshStill(viewData);
      scroller.Reset(getDataLineData(), viewData);
      Sync();
    }

    public void UpdateScrollItemPositionByVerticalScrollPosition(double verticalScrollPosition)
    {
      scroller.Scroll(verticalScrollPosition, getDataLineData(), viewData);
      Sync();
    }

    public void UpdateLeftInc()
    {
      incLeft();
      scroller.Restore(getDataLineData(), viewData);
      Sync();
    }

    public void UpdateRightInc()
    {
      incRight();
      scroller.Restore(getDataLineData(), viewData);
      Sync();
    }
  }
}
