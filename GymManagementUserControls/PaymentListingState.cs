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
  public class PaymentListingState : PaymentListingStoreState
  {
    PaymentListingUpdates? payment_listing_updates;
    PaymentListingGrid listingGrid;
    KineticListingState kineticListingState;

    public PaymentListingState(AppStore app_store) : base(app_store)
    {
      listingGrid = new PaymentListingGrid();

      var sample = new PaymentCard();
      sample.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
      var vd = new ListingViewData(2, 4, 12, sample.DesiredSize.Height);

      base.Create(listingGrid, vd);
      kineticListingState = new KineticListingState(
          "Payments",
          vd,
          payment_listing_store.payment_details_short_list.GetStateData,
          payment_listing_store.payment_details_short_list.RefreshStill,
          payment_listing_store.payment_details_short_list.Refresh,
          payment_listing_store.payment_details_short_list.IncLeft,
          payment_listing_store.payment_details_short_list.IncRight
        );
    }

    public void LoadPaymentListingUpdates(PaymentListingUpdates payment_listing_updates)
    {
      this.payment_listing_updates = payment_listing_updates;
      SyncState();
    }

    public void UnloadPaymentListingUpdates()
    {
      this.payment_listing_updates = null;
    }

    private void SyncState()
    {
      if (payment_listing_updates != null)
      {
        payment_listing_updates.UpdateListingControl(listingGrid, kineticListingState);
      }
    }
  }
}
