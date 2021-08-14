using System;
using System.Collections.Generic;
using GymManagementDataModel;
using GymManagementLogic;
using GymManagementHILogic;

namespace GymManagementUserControls
{
  public class PaymentListingStore : AppStore
  {
    public DataLineTravel<PaymentDetails> payment_details_short_list;

    public PaymentListingStore(AppStoreManager appStoreManager, DataLineList<PaymentDetails> dll, ListingViewData vd) : base(appStoreManager)
    {
      var sd = new SearchData(
          @"
            ORDER BY date_of_payment DESC
          ",
          new Dictionary<string, long>(),
          new Dictionary<string, int>(),
          new Dictionary<string, string>()
        );
      payment_details_short_list = new DataLineTravel<PaymentDetails>(
          sd,
          appStoreManager.mainDbPd.SearchPaymentDetails,
          appStoreManager.mainDbPd.GetPaymentDetailsSearchFullCount,
          dll,
          vd
        );
    }
  }
}
