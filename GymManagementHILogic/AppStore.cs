using System;
using GymManagementDataModel;
using GymManagementDataBase;
using GymManagementLogic;
using System.Collections.Generic;

namespace GymManagementHILogic
{
  public class AppStoreManager
  {
    List<AppStore> appStores = new List<AppStore>();
    public MainDatabaseTraineeDetails mainDbTd = new MainDatabaseTraineeDetails();
    public MainDatabasePaymentDetails mainDbPd = new MainDatabasePaymentDetails();

    public void AddStore(AppStore appStore)
    {
      appStores.Add(appStore);
    }

    public void StoreRegisteredTrainee(TraineeDetails td)
    {
      mainDbTd.InsertTraineeDetails(td);
    }

    public void StoreCreatedPayment(PaymentDetails pd)
    {
      mainDbPd.InsertPaymentDetails(pd);
    }

    public void Destroy(AppStore appStore)
    {
      appStores.Remove(appStore);
    }
  }

  public class AppStore
  {
    public AppStoreManager appStoreManager;

    public AppStore(AppStoreManager appStoreManager)
    {
      this.appStoreManager = appStoreManager;
      appStoreManager.AddStore(this);
    }

    public void Destroy()
    {
      appStoreManager.Destroy(this);
    }
  }
}
