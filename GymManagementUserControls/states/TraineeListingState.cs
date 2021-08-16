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
  public class TraineeListingState : AppState
  {
    TraineeListingUpdates? trainee_listing_updates;
    TraineeListingGrid listingGrid;
    KineticListingState kineticListingState;
    TraineeListingStore traineeListingStore;

    public TraineeListingState(AppStoreManager appStoreManager, OpenPage openPage) : base(appStoreManager, openPage)
    {
      listingGrid = new TraineeListingGrid(OpenProfilePage);

      var sample = new TraineeCard();
      sample.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
      var vd = new ListingViewData(3, 3, 12, sample.DesiredSize.Height);

      traineeListingStore = new TraineeListingStore(appStoreManager, listingGrid, vd);
      kineticListingState = new KineticListingState(
          "Trainees",
          vd,
          traineeListingStore.trainee_details_short_list.GetStateData,
          traineeListingStore.trainee_details_short_list.RefreshStill,
          traineeListingStore.trainee_details_short_list.Refresh,
          traineeListingStore.trainee_details_short_list.IncLeft,
          traineeListingStore.trainee_details_short_list.IncRight
        );
    }

    public override void Destroy()
    {
      traineeListingStore.Destroy();
    }

    public void OpenProfilePage(long trainee_id)
    {
      openPage("ProfilePage", new ProfilePageState(appStoreManager, openPage, "Trainees", trainee_id));
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
