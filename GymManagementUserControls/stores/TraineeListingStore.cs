using System;
using System.Collections.Generic;
using GymManagementDataModel;
using GymManagementLogic;
using GymManagementHILogic;

namespace GymManagementUserControls
{
  public class TraineeListingStore : AppStore
  {
    public DataLineTravel<TraineeDetails> trainee_details_short_list;

    public TraineeListingStore(AppStoreManager appStoreManager, DataLineList<TraineeDetails> dll, ListingViewData vd) : base(appStoreManager)
    {
      var sd = new SearchData(
          @"
            ORDER BY joining_date DESC
          ",
          new Dictionary<string, long>(),
          new Dictionary<string, int>(),
          new Dictionary<string, string>()
        );
      trainee_details_short_list = new DataLineTravel<TraineeDetails>(
          sd,
          appStoreManager.mainDbTd.SearchTraineeDetails,
          appStoreManager.mainDbTd.GetTraineeDetailsSearchFullCount,
          dll,
          vd
        );
    }
  }
}
