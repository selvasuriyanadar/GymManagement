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
using GymManagementHILogic;
using GymManagementLogic;

namespace GymManagementUserControls
{
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

    public void LoadKineticListingUpdates(KineticListingUpdates kineticListingUpdates)
    {
      this.kineticListingUpdates = kineticListingUpdates;
      Sync();
    }

    public void UnloadKineticListingUpdates()
    {
      kineticListingUpdates = null;
    }

    private void Sync()
    {
      SyncScrollControl();
      SyncViewData();
    }

    private void SyncScrollControl()
    {
      if (kineticListingUpdates != null)
      {
        kineticListingUpdates.Sync(itemsName, loaderState, expandSlider, getDataLineData(), scroller.GetSP(), scroller.GetVP());
      }
    }
    
    private void SyncViewData()
    {
      if (kineticListingUpdates != null)
      {
        kineticListingUpdates.SyncViewData(viewData);
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
        SyncScrollControl();
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
        SyncScrollControl();
      }
    }

    public void RefreshAtCurrentItemPosition()
    {
      refreshStill(viewData);
      scroller.Reset(getDataLineData(), viewData);
      SyncScrollControl();
    }

    public void UpdateScrollItemPositionByVerticalScrollPosition(double verticalScrollPosition)
    {
      scroller.Scroll(verticalScrollPosition, getDataLineData(), viewData);
      SyncScrollControl();
    }

    public void UpdateLeftInc()
    {
      incLeft();
      scroller.Restore(getDataLineData(), viewData);
      SyncScrollControl();
    }

    public void UpdateRightInc()
    {
      incRight();
      scroller.Restore(getDataLineData(), viewData);
      SyncScrollControl();
    }
  }
}
