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
  public class PaymentListingState : AppState
  {
    PaymentListingUpdates? payment_listing_updates;
    PaymentListingGrid listingGrid = new PaymentListingGrid();
    KineticListingState kineticListingState;
    PaymentListingStore paymentListingStore;

    public PaymentListingState(AppStoreManager appStoreManager) : base(appStoreManager)
    {
      var sample = new PaymentCard();
      sample.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
      var vd = new ListingViewData(2, 4, 12, sample.DesiredSize.Height);

      paymentListingStore = new PaymentListingStore(appStoreManager, listingGrid, vd);
      kineticListingState = new KineticListingState(
          "Payments",
          vd,
          paymentListingStore.payment_details_short_list.GetStateData,
          paymentListingStore.payment_details_short_list.RefreshStill,
          paymentListingStore.payment_details_short_list.Refresh,
          paymentListingStore.payment_details_short_list.IncLeft,
          paymentListingStore.payment_details_short_list.IncRight
        );
    }

    public override void Destroy()
    {
      paymentListingStore.Destroy();
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
