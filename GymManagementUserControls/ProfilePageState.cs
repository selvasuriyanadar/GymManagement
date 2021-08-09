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
using MaterialDesignThemes.Wpf;
using System.Globalization;

namespace GymManagementUserControls
{
  public class ProfilePageState : ProfileViewStoreState
  {
    string menu;
    long trainee_id;

    ProfilePageUpdates? prof_page_updates;
    ProfilePagePaymentsGrid paymentsGrid = new ProfilePagePaymentsGrid();
    KineticListingState kineticListingState;

    public ProfilePageState(AppStore app_store, string menu, long trainee_id) : base(app_store)
    {
      this.menu = menu;
      this.trainee_id = trainee_id;

      var sample = new PaymentCard();
      sample.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
      var vd = new ListingViewData(2, 4, 12, sample.DesiredSize.Height);

      var sd = new SearchData(
          @"
            WHERE o.trainee_id = $searchparam1
            ORDER BY date_of_payment DESC
          ",
          new Dictionary<string, long>(){ { "$searchparam1", trainee_id } },
          new Dictionary<string, int>(),
          new Dictionary<string, string>()
        );

      base.Create(paymentsGrid, vd, sd);
      kineticListingState = new KineticListingState(
          "Payments",
          vd,
          profile_view_store.payment_details_short_list.GetStateData,
          profile_view_store.payment_details_short_list.RefreshStill,
          profile_view_store.payment_details_short_list.Refresh,
          profile_view_store.payment_details_short_list.IncLeft,
          profile_view_store.payment_details_short_list.IncRight
        );

      try
      {
        profile_view_store.FetchTrainee(trainee_id);
      }
      catch {}
    }

    public void LoadProfilePageUpdates(ProfilePageUpdates prof_page_updates)
    {
      this.prof_page_updates = prof_page_updates;
      SyncState();
      this.prof_page_updates.ResetComponent();
    }

    public void UnloadProfilePageUpdates()
    {
      prof_page_updates = null;
    }

    public override bool Match(AppState s)
    {
      try
      {
        return trainee_id == ((ProfilePageState)s).trainee_id;
      }
      catch
      {
        return false;
      }
    }

    private void SyncState()
    {
      if (prof_page_updates != null)
      {
        prof_page_updates.UpdateTraineeView(profile_view_store.trainee_details);

        prof_page_updates.UpdateListingControl(paymentsGrid, kineticListingState);
      }
    }
  }
}
