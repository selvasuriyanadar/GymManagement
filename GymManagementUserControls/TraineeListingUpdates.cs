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
  public class TraineeListingGrid : DataLineGrid<TraineeDetails>
  {
    public delegate void OpenProfilePage(long trainee_id);

    OpenProfilePage openProfilePage;

    public TraineeListingGrid(OpenProfilePage openProfilePage)
    {
      this.openProfilePage = openProfilePage;
    }

    public void CardButton_Click(object sender, RoutedEventArgs e)
    {
      var uc = GetParentUserControl((DependencyObject)sender);
      openProfilePage((long) (GetList()[GetGrid().Children.IndexOf(uc)])._id);
    }

    protected override UIElement GetUiItem(TraineeDetails d)
    {
      return new TraineeCard(d, CardButton_Click);
    }
  }

  public class TraineeListingUpdates
  {
    TraineeListing trainee_listing;

    KineticListing kineticListing = new KineticListing();

    public TraineeListingUpdates(TraineeListing trainee_listing)
    {
      this.trainee_listing = trainee_listing;
    }

    public void UpdateListingControl(TraineeListingGrid listingGrid, KineticListingState kineticListingState)
    {
      var grid = listingGrid.GetGrid();
      kineticListing.LoadState(grid, kineticListingState);
      this.trainee_listing.body.Children.Clear();
      this.trainee_listing.body.Children.Add(kineticListing);
    }
  }
}
