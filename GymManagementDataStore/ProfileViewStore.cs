using System;
using System.Collections.Generic;
using GymManagementDataModel;
using GymManagementApi;

namespace GymManagementDataStore
{
  public class DataDoesNotExist : Exception
  {
    public DataDoesNotExist(string msg) : base(msg) {}
  }

  public class ProfileViewStore
  {
    public TraineeDetails? trainee_details;
    public DataLineTravel<PaymentDetails> payment_details_short_list;

    MainDatabaseTraineeDetails main_db_td;
    MainDatabasePaymentDetails main_db_pd;

    public ProfileViewStore(MainDatabaseTraineeDetails main_db_td, MainDatabasePaymentDetails main_db_pd, DataLineList<PaymentDetails> dll, ListingViewData vd, SearchData sd)
    {
      this.main_db_td = main_db_td;
      this.main_db_pd = main_db_pd;
      payment_details_short_list = new DataLineTravel<PaymentDetails>(
          sd,
          main_db_pd.SearchPaymentDetails,
          main_db_pd.GetPaymentDetailsSearchFullCount,
          dll,
          vd
        );
    }

    public void FetchTrainee(long trainee_id)
    {
      trainee_details = main_db_td.FetchTraineeDetails(trainee_id);
      if (trainee_details == null)
        throw new DataDoesNotExist("the trainee_id is not valid or the trainee data is corrupt.");
    }
  }
}
