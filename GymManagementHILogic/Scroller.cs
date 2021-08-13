using System;
using GymManagementDataModel;
using GymManagementLogic;
using System.Collections.Generic;

namespace GymManagementHILogic
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
}
