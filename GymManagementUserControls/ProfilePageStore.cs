using System;
using System.Collections.Generic;
using GymManagementDataModel;
using GymManagementLogic;
using GymManagementHILogic;

namespace GymManagementUserControls
{
  public class ProfilePageStore : AppStore
  {
    public TraineeDetails? trainee_details;
    public DataLineTravel<PaymentDetails> payment_details_short_list;

    public ProfilePageStore(AppStoreManager appStoreManager, DataLineList<PaymentDetails> dll, ListingViewData vd, SearchData sd) : base(appStoreManager)
    {
      payment_details_short_list = new DataLineTravel<PaymentDetails>(
          sd,
          appStoreManager.mainDbPd.SearchPaymentDetails,
          appStoreManager.mainDbPd.GetPaymentDetailsSearchFullCount,
          dll,
          vd
        );
    }

    public void FetchTrainee(long trainee_id)
    {
      trainee_details = appStoreManager.mainDbTd.FetchTraineeDetails(trainee_id);
    }
  }
}
