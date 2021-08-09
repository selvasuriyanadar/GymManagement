using System;
using GymManagementDataModel;
using GymManagementApi;
using GymManagementLogic;
using GymManagementDataStore;
using System.Collections.Generic;

namespace GymManagementHILogic
{
  public class InvalidData : Exception
  {
    public InvalidData(string msg) : base(msg) {}
  }

  public class TraineeListingStoreState : AppState
  {
    public TraineeListingStore trainee_listing_store;

    public TraineeListingStoreState(AppStore app_store) : base(app_store)
    {
    }

    public void Create(DataLineList<TraineeDetails> dll, ListingViewData vd)
    {
      trainee_listing_store = app_store.CreateStore(this, dll, vd);
    }

    public override void Destroy()
    {
      app_store.RemoveStore(this);
    }
  }

  public class PaymentListingStoreState : AppState
  {
    public PaymentListingStore payment_listing_store;

    public PaymentListingStoreState(AppStore app_store) : base(app_store)
    {
    }

    public void Create(DataLineList<PaymentDetails> dll, ListingViewData vd)
    {
      payment_listing_store = app_store.CreateStore(this, dll, vd);
    }

    public override void Destroy()
    {
      app_store.RemoveStore(this);
    }
  }

  public class ProfileViewStoreState : AppState
  {
    public ProfileViewStore profile_view_store;

    public ProfileViewStoreState(AppStore app_store) : base(app_store)
    {
    }

    public void Create(DataLineList<PaymentDetails> dll, ListingViewData vd, SearchData sd)
    {
      profile_view_store = app_store.CreateStore(this, dll, vd, sd);
    }

    public override void Destroy()
    {
      app_store.RemoveStore(this);
    }
  }

  public class AppStore
  {
    Dictionary<TraineeListingStoreState, TraineeListingStore> trainee_listing_stores = new Dictionary<TraineeListingStoreState, TraineeListingStore>();
    Dictionary<PaymentListingStoreState, PaymentListingStore> payment_listing_stores = new Dictionary<PaymentListingStoreState, PaymentListingStore>();
    Dictionary<ProfileViewStoreState, ProfileViewStore> profile_view_stores = new Dictionary<ProfileViewStoreState, ProfileViewStore>();

    MainDatabaseTraineeDetails main_db_td;
    MainDatabasePaymentDetails main_db_pd;

    public PaymentListingStore CreateStore(PaymentListingStoreState s, DataLineList<PaymentDetails> dll, ListingViewData vd)
    {
      payment_listing_stores.Add(s, new PaymentListingStore(main_db_pd, dll, vd));
      return payment_listing_stores[s];
    }

    public void RemoveStore(PaymentListingStoreState s)
    {
      payment_listing_stores.Remove(s);
    }

    public TraineeListingStore CreateStore(TraineeListingStoreState s, DataLineList<TraineeDetails> dll, ListingViewData vd)
    {
      trainee_listing_stores.Add(s, new TraineeListingStore(main_db_td, dll, vd));
      return trainee_listing_stores[s];
    }

    public void RemoveStore(TraineeListingStoreState s)
    {
      trainee_listing_stores.Remove(s);
    }

    public ProfileViewStore CreateStore(ProfileViewStoreState s, DataLineList<PaymentDetails> dll, ListingViewData vd, SearchData sd)
    {
      profile_view_stores.Add(s, new ProfileViewStore(main_db_td, main_db_pd, dll, vd, sd));
      return profile_view_stores[s];
    }

    public void RemoveStore(ProfileViewStoreState s)
    {
      profile_view_stores.Remove(s);
    }

    public AppStore()
    {
      main_db_td = new MainDatabaseTraineeDetails();
      main_db_pd = new MainDatabasePaymentDetails();
    }

    public void StoreRegisteredTrainee(TraineeDetails td)
    {
      main_db_td.InsertTraineeDetails(td);
    }

    public void StoreCreatedPayment(PaymentDetails pd)
    {
      main_db_pd.InsertPaymentDetails(pd);
    }
  }
}
