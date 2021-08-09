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
  public class TraineeListingState : TraineeListingStoreState
  {
    TraineeListingUpdates? trainee_listing_updates;
    TraineeListingGrid listingGrid;
    KineticListingState kineticListingState;

    OpenPage open_page;

    public TraineeListingState(OpenPage open_page, AppStore app_store) : base(app_store)
    {
      this.open_page = open_page;
      listingGrid = new TraineeListingGrid(OpenProfilePage);

      var sample = new TraineeCard();
      sample.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
      var vd = new ListingViewData(3, 3, 12, sample.DesiredSize.Height);

      base.Create(listingGrid, vd);
      kineticListingState = new KineticListingState(
          "Trainees",
          vd,
          trainee_listing_store.trainee_details_short_list.GetStateData,
          trainee_listing_store.trainee_details_short_list.RefreshStill,
          trainee_listing_store.trainee_details_short_list.Refresh,
          trainee_listing_store.trainee_details_short_list.IncLeft,
          trainee_listing_store.trainee_details_short_list.IncRight
        );
    }

    public void OpenProfilePage(long trainee_id)
    {
      open_page("ProfilePage", new ProfilePageState(app_store, "Trainees", trainee_id));
    }

    public void LoadTraineeListingUpdates(TraineeListingUpdates trainee_listing_updates)
    {
      this.trainee_listing_updates = trainee_listing_updates;
      SyncState();
    }

    public void UnloadTraineeListingUpdates()
    {
      this.trainee_listing_updates = null;
    }

    private void SyncState()
    {
      if (trainee_listing_updates != null)
      {
        trainee_listing_updates.UpdateListingControl(listingGrid, kineticListingState);
      }
    }
  }
}
