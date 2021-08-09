using System;
using System.Collections.Generic;
using GymManagementDataModel;
using GymManagementApi;

namespace GymManagementDataStore
{
  public class PaymentListingStore
  {
    public DataLineTravel<PaymentDetails> payment_details_short_list;

    MainDatabasePaymentDetails main_db_pd;

    public PaymentListingStore(MainDatabasePaymentDetails main_db_pd, DataLineList<PaymentDetails> dll, ListingViewData vd)
    {
      this.main_db_pd = main_db_pd;
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
          main_db_pd.SearchPaymentDetails,
          main_db_pd.GetPaymentDetailsSearchFullCount,
          dll,
          vd
        );
    }
  }
}
