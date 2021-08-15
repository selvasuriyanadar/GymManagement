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
using GymManagementLogic;
using System.Globalization;
using MaterialDesignThemes.Wpf;

namespace GymManagementUserControls
{
  ///ToDo Single Data Validations, Invalid Validations and Warning Validations

  public class TraineeRegistrationState : AppState
  {
    string menu;
    TraineeDetails trainee_details = new TraineeDetails();
    RegistrationPaymentGroup pay_group = new RegistrationPaymentGroup();
    ImagePicker imagePicker = new ImagePicker();
    bool registration_complete = false;
    bool payment_complete = false;

    public OpenPage openPage;

    RegistrationPageUpdates? reg_page_updates;

    public TraineeRegistrationState(OpenPage openPage, AppStoreManager appStoreManager, string menu) : base(appStoreManager)
    {
      this.openPage = openPage;
      this.menu = menu;

      pay_group.InitiateWithDefaultSelections();
      EnterJoiningDate(DateTime.Today);
    }

    public void LoadRegistrationPageUpdates(
        RegistrationPageUpdates reg_page_updates
      )
    {
      this.reg_page_updates = reg_page_updates;

      SyncState();
      reg_page_updates.ResetComponent();
    }

    public void UnloadRegistrationPageUpdates()
    {
      this.reg_page_updates = null;
    }

    public override bool Match(AppState app_state)
    {
      try
      {
        return registration_complete == ((TraineeRegistrationState)app_state).registration_complete;
      }
      catch
      {
        return false;
      }
    }

    private void SyncState()
    {
      SyncPaymentPanel();
      SyncJoiningDate();
      SyncPhoto();
      SyncSupplementLentState();
      SyncRegistrationActionArea();

      SyncTraineeDetailsSex();
      SyncTraineeDetails("FirstName");
      SyncTraineeDetails("LastName");
      SyncTraineeDetails("Comments");
      SyncTraineeDetails("Phone");
      SyncTraineeDetails("Email");
      SyncTraineeDetails("Address");
      SyncTraineeDetails("City");
      SyncTraineeDetails("Age");

      SyncPaymentRelatedAmounts("SubscriptionAmount");
      SyncPaymentRelatedAmounts("SubscriptionDiscount");
      SyncPaymentRelatedAmounts("AdvanceAmount");
      SyncPaymentRelatedAmounts("AdvanceDiscount");
      SyncPaymentRelatedAmounts("SupplementAmount");
      SyncPaymentRelatedAmounts("SupplementDiscount");
      SyncPaymentRelatedAmounts("SupplementLendAmount");
      SyncPaymentRelatedAmounts("SupplementLendDiscount");
      SyncPaymentTotalAmount();
    }

    /// Reverse Commands

    public TraineeDetails? GetTraineeDetails()
    {
      if (trainee_details.Validate().Count == 0)
      {
        var trainee_details_copy = new TraineeDetails();
        trainee_details.Copy(trainee_details_copy);
        return trainee_details_copy;
      }
      else
      {
        return null;
      }
    }

    public List<PaymentDetails>? GetPaymentDetails(bool pay)
    {
      if (pay_group.Validate(pay).Count == 0)
      {
        return pay_group.ExtractAllPaymentDetails(pay);
      }
      else
      {
        return null;
      }
    }

    public void Submit(bool pay)
    {
      var td = GetTraineeDetails();
      if (td != null)
      {
        var pt = GetPaymentDetails(pay);
        if (pt != null)
        {
          try
          {
            appStoreManager.StoreRegisteredTrainee(td);
            foreach (var pd in pt)
            {
              pd._trainee_id = td._id;
              appStoreManager.StoreCreatedPayment(pd);
            }
            if (reg_page_updates != null)
            {
              reg_page_updates.SnackIt("The Trainee has been successfully registered.", "View Trainee", () => openProfilePage((long) td._id));
            }
            FinishRegistrationProcess(pay);
          }
          catch
          {
            if (reg_page_updates != null)
            {
              reg_page_updates.SnackIt("Sorry there was an error.", null, null);
            }
          }
        }
      }
    }

    /// Commands

    public void openProfilePage(long trainee_id)
    {
      openPage("ProfilePage", new ProfilePageState(appStoreManager, menu, trainee_id));
    }

    public void openRegistrationPage()
    {
      openPage("RegistrationPage", new TraineeRegistrationState(openPage, appStoreManager, menu));
    }

    private void SyncPaymentPanel()
    {
      if (reg_page_updates != null)
      {
        reg_page_updates.UpdatePaymentPanelHeadings(pay_group);
      }
    }

    private void SyncJoiningDate()
    {
      if (reg_page_updates != null)
      {
        reg_page_updates.UpdateJoiningDate(trainee_details.profile_details.joining_date);
      }
    }

    public void EnterJoiningDate(DateTime? date)
    {
      trainee_details.profile_details.joining_date = date;
      pay_group.UpdateAllPaymentDates(date);

      SyncJoiningDate();
    }

    private void SyncPhoto()
    {
      if (reg_page_updates != null)
      {
        reg_page_updates.UpdatePhoto(trainee_details.profile_details.photo_path);
      }
    }

    public void SelectPhoto()
    {
      var photo_path = imagePicker.PickAndGetImage();
      if (photo_path != null)
      {
        trainee_details.profile_details.photo_path = photo_path;

        SyncPhoto();
      }
    }

    private void SyncRegistrationActionArea()
    {
      if (reg_page_updates != null)
      {
        reg_page_updates.UpdateRegistrationActionArea(registration_complete, payment_complete);
      }
    }

    private void FinishRegistrationProcess(bool pay)
    {
      registration_complete = true;
      payment_complete = pay;

      SyncRegistrationActionArea();
    }

    private void SyncSupplementLentState()
    {
      if (reg_page_updates != null)
      {
        reg_page_updates.UpdateSupplementField(pay_group);
      }
    }

    public void DisableSupplementLendPayment()
    {
      pay_group.RevokeLentOnProteinSupplementPayment();

      SyncSupplementLentState();
      SyncPaymentTotalAmount();
    }

    public void EnableSupplementLendAndPay()
    {
      pay_group.LentProteinSupplementPayment();

      SyncSupplementLentState();
      SyncPaymentTotalAmount();
    }

    public void SyncPaymentRelatedAmounts(string name)
    {
      if (reg_page_updates != null)
      {
        reg_page_updates.UpdatePaymentRelatedAmounts(name, pay_group);
      }
    }

    private void SyncPaymentTotalAmount()
    {
      if (reg_page_updates != null)
      {
        reg_page_updates.UpdateTotal(pay_group);
      }
    }

    public void EnterPaymentRelatedAmounts(string name, int? amount)
    {
      switch (name)
      {
        case "SubscriptionAmount":
          pay_group.gym_subscription_details_payment.amount = amount;
          SyncPaymentRelatedAmounts(name);
          SyncPaymentTotalAmount();
          break;
        case "SubscriptionDiscount":
          pay_group.gym_subscription_details_paydata.discount = amount ?? 0;
          SyncPaymentRelatedAmounts(name);
          SyncPaymentTotalAmount();
          break;
        case "AdvanceAmount":
          pay_group.advance_details_payment.amount = amount;
          SyncPaymentRelatedAmounts(name);
          SyncPaymentTotalAmount();
          break;
        case "AdvanceDiscount":
          pay_group.advance_details_paydata.discount = amount ?? 0;
          SyncPaymentRelatedAmounts(name);
          SyncPaymentTotalAmount();
          break;
        case "SupplementAmount":
          pay_group.protein_supplement_details_payment.amount = amount;
          SyncPaymentRelatedAmounts(name);
          SyncPaymentTotalAmount();
          break;
        case "SupplementDiscount":
          pay_group.protein_supplement_details_paydata.discount = amount ?? 0;
          SyncPaymentRelatedAmounts(name);
          SyncPaymentTotalAmount();
          break;
        case "SupplementLendAmount":
          pay_group.protein_supplement_details_lent_payment.amount = amount;
          SyncPaymentRelatedAmounts(name);
          SyncPaymentTotalAmount();
          break;
        case "SupplementLendDiscount":
          pay_group.protein_supplement_details_lent_paydata.discount = amount ?? 0;
          SyncPaymentRelatedAmounts(name);
          SyncPaymentTotalAmount();
          break;
      }
    }

    public void SyncTraineeDetails(string name)
    {
      if (reg_page_updates != null)
      {
        reg_page_updates.UpdateTraineeDetails(name, trainee_details);
      }
    }

    public void EnterTraineeDetails(string name, string? value)
    {
      switch (name)
      {
        case "FirstName":
          trainee_details.profile_details.first_name = value;
          SyncTraineeDetails(name);
          break;
        case "LastName":
          trainee_details.profile_details.last_name = value;
          SyncTraineeDetails(name);
          break;
        case "Comments":
          trainee_details.profile_details.comments = value;
          SyncTraineeDetails(name);
          break;
        case "Phone":
          trainee_details.contact_details.phone_no = value;
          SyncTraineeDetails(name);
          break;
        case "Email":
          trainee_details.contact_details.email = value;
          SyncTraineeDetails(name);
          break;
        case "Address":
          trainee_details.contact_details.address = value;
          SyncTraineeDetails(name);
          break;
        case "City":
          trainee_details.contact_details.city = value;
          SyncTraineeDetails(name);
          break;
      }
    }

    public void EnterTraineeDetails(string name, int? value)
    {
      switch (name)
      {
        case "Age":
          trainee_details.profile_details.age = value;
          SyncTraineeDetails(name);
          break;
      }
    }

    private void SyncTraineeDetailsSex()
    {
      if (reg_page_updates != null)
      {
        reg_page_updates.UpdateTraineeDetailsSex(trainee_details.profile_details.sex);
      }
    }

    public void EnterTraineeDetailsSex(ProfileDetails.Sex? value)
    {
      trainee_details.profile_details.sex = value;

      SyncTraineeDetailsSex();
    }
  }
}
