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
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class PaymentListing : UserControl
  {
    PaymentListingState? payment_listing_state;
    PaymentListingUpdates payment_listing_updates;

    public PaymentListing()
    {
      InitializeComponent();
      payment_listing_updates = new PaymentListingUpdates(this);
    }

    public PaymentListing LoadState(PaymentListingState payment_listing_state)
    {
      UnloadState();
      this.payment_listing_state = payment_listing_state;
      this.payment_listing_state.LoadPaymentListingUpdates(payment_listing_updates);
      return this;
    }

    public void UnloadState()
    {
      if (payment_listing_state != null)
      {
        payment_listing_state.UnloadPaymentListingUpdates();
        payment_listing_state = null;
      }
    }
  }
}
