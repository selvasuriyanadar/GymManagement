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
  public class PaymentListingGrid : DataLineGrid<PaymentDetails>
  {
    protected override UIElement GetUiItem(PaymentDetails d)
    {
      return new PaymentCard(d);
    }
  }

  public class PaymentListingUpdates
  {
    PaymentListing payment_listing;

    KineticListing kineticListing = new KineticListing();

    public PaymentListingUpdates(PaymentListing payment_listing)
    {
      this.payment_listing = payment_listing;
    }

    public void UpdateListingControl(PaymentListingGrid listingGrid, KineticListingState kineticListingState)
    {
      var grid = listingGrid.GetGrid();
      kineticListing.LoadState(grid, kineticListingState);
      this.payment_listing.body.Children.Clear();
      this.payment_listing.body.Children.Add(kineticListing);
    }
  }
}
