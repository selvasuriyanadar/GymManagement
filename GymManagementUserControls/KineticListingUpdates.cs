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
  public class KineticListingUpdates
  {
    KineticListing kineticListing;

    public KineticListingUpdates(KineticListing kineticListing)
    {
      this.kineticListing = kineticListing;
    }

    public void UpdateHorizontalScrollPosition(double pos)
    {
      kineticListing.scrollViewer.ScrollToHorizontalOffset(pos);
    }

    public void Initialise()
    {
      UpdateHorizontalScrollPosition(0);
    }

    public void LoadGrid(System.Windows.Controls.Primitives.UniformGrid grid, ListingViewData vd)
    {
      grid.Columns = (int) vd.itemsPerRow;
      kineticListing.scrollViewer.Content = grid;
    }

    public void UnloadGrid()
    {
      kineticListing.scrollViewer.Content = null;
    }

    public void SyncSliderStretch(bool expandSlider, DataLineData dld, long scrollItemPosition)
    {
      kineticListing.slider.ValueChanged -= kineticListing.Slider_ValueChanged;
      if (expandSlider)
      {
        //kineticListing.sliderCaption.Text = dld.GetRelScrollPositionOnFetchedItems(scrollItemPosition).ToString() + "/" + dld.FetchedItems.ToString();
        kineticListing.sliderCaption.Text = scrollItemPosition.ToString() + "/" + dld.FetchEndPosition.ToString();

        kineticListing.sliderExpandIcon.Visibility = Visibility.Collapsed;
        kineticListing.sliderCollapseIcon.Visibility = Visibility.Visible;
        
        kineticListing.slider.Minimum = dld.FetchPosition;
        kineticListing.slider.Maximum = dld.FetchEndPosition;
        kineticListing.slider.Value = scrollItemPosition;
        kineticListing.slider.SelectionStart = kineticListing.slider.Minimum;
        kineticListing.slider.SelectionEnd = kineticListing.slider.Maximum;
      }
      else
      {
        //kineticListing.sliderCaption.Text = dld.FetchPosition.ToString() + "/" + dld.EndPosition.ToString();
        kineticListing.sliderCaption.Text = scrollItemPosition.ToString() + "/" + dld.EndPosition.ToString();

        kineticListing.sliderExpandIcon.Visibility = Visibility.Visible;
        kineticListing.sliderCollapseIcon.Visibility = Visibility.Collapsed;

        kineticListing.slider.Minimum = dld.StartPosition;
        kineticListing.slider.Maximum = dld.EndPosition;
        kineticListing.slider.Value = scrollItemPosition;
        kineticListing.slider.SelectionStart = dld.FetchPosition;
        kineticListing.slider.SelectionEnd = dld.FetchEndPosition;
      }
      kineticListing.slider.ValueChanged += kineticListing.Slider_ValueChanged;
    }

    public void Sync(string? itemsName, bool loading, bool expandSlider, DataLineData dld, long scrollItemPosition, double verticalScrollPosition)
    {
      if (itemsName != null)
      {
        kineticListing.textInfo.Text = dld.TotalItems.ToString() + " " + itemsName + " found";
      }
      else
      {
        kineticListing.textInfo.Text = "";
      }

      if (loading)
      {
        kineticListing.fullScreenLoader.Visibility = Visibility.Visible;
      }
      else
      {
        kineticListing.fullScreenLoader.Visibility = Visibility.Collapsed;
      }

      SyncSliderStretch(expandSlider, dld, scrollItemPosition);

      if (verticalScrollPosition != null)
      {
        if (kineticListing.scrollViewer.ScrollableHeight < verticalScrollPosition)
        {
          verticalScrollPosition = kineticListing.scrollViewer.ScrollableHeight;
        }
        kineticListing.scrollViewer.ScrollToVerticalOffset((double) verticalScrollPosition);
      }
      else
      {
        kineticListing.scrollViewer.ScrollToVerticalOffset(0);
      }
    }
  }
}
