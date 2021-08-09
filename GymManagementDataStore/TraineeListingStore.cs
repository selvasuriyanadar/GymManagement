using System;
using System.Collections.Generic;
using GymManagementDataModel;
using GymManagementApi;

namespace GymManagementDataStore
{
  public class TraineeListingStore
  {
    public DataLineTravel<TraineeDetails> trainee_details_short_list;

    MainDatabaseTraineeDetails main_db_td;

    public TraineeListingStore(MainDatabaseTraineeDetails main_db_td, DataLineList<TraineeDetails> dll, ListingViewData vd)
    {
      this.main_db_td = main_db_td;
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
          main_db_td.SearchTraineeDetails,
          main_db_td.GetTraineeDetailsSearchFullCount,
          dll,
          vd
        );
    }
  }
}
