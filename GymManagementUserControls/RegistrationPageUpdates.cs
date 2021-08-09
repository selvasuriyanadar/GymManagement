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
using MaterialDesignThemes.Wpf;
using System.Globalization;

namespace GymManagementUserControls
{
  public class RegistrationPageUpdates
  {
    RegistrationPage reg_page;

    public RegistrationPageUpdates(RegistrationPage reg_page)
    {
      this.reg_page = reg_page;
    }

    // Stateless
    public void ResetComponent()
    {
      reg_page.ResetComponent();
    }

    //Updates

    public void UpdateRegistrationActionArea(bool registration_complete, bool payment_complete)
    {
      if (registration_complete)
      {
        if (payment_complete)
        {
          reg_page.PaymentCompleteChip.Visibility = Visibility.Visible;
          reg_page.PrintBillButton.Visibility = Visibility.Visible;
        }

        reg_page.CompletePaymentButton.Visibility = Visibility.Collapsed;
        reg_page.PayLaterButton.Visibility = Visibility.Collapsed;
        reg_page.RegisterAnotherClientButton.Visibility = Visibility.Visible;
      }
      else
      {
        reg_page.PaymentCompleteChip.Visibility = Visibility.Collapsed;
        reg_page.PrintBillButton.Visibility = Visibility.Collapsed;
        reg_page.CompletePaymentButton.Visibility = Visibility.Visible;
        reg_page.PayLaterButton.Visibility = Visibility.Visible;
        reg_page.RegisterAnotherClientButton.Visibility = Visibility.Collapsed;
      }
    }

    public void UpdatePaymentPanelHeadings(RegistrationPaymentGroup pay_group)
    {
      reg_page.SubscriptionProductPlan.Text = pay_group.gym_subscription_details.SelectionAsString();
      reg_page.AdvanceProduct.Text = pay_group.advance_details.SelectionAsString();
      reg_page.GymProteinBodyPowderProduct.Text = pay_group.protein_supplement_details.SelectionAsString();
    }

    public void UpdateJoiningDate(DateTime? joining_date)
    {
      if (joining_date != null)
      {
        reg_page.SubscriptionInitialDate.Text = ((DateTime)(joining_date)).ToString("dd/MM/yyyy");

        reg_page.JoiningDate.SelectedDateChanged -= reg_page.JoiningDate_Changed;
        reg_page.JoiningDate.SelectedDate = ((DateTime)joining_date);
        reg_page.JoiningDate.SelectedDateChanged += reg_page.JoiningDate_Changed;
      }
      else
      {
        reg_page.SubscriptionInitialDate.Text = "";

        reg_page.JoiningDate.SelectedDateChanged -= reg_page.JoiningDate_Changed;
        reg_page.JoiningDate.SelectedDate = null;
        reg_page.JoiningDate.SelectedDateChanged += reg_page.JoiningDate_Changed;
      }
    }

    public void UpdatePhoto(string? photo_path)
    {
      if (photo_path != null)
        reg_page.ImgReceiver.ImageSource = new BitmapImage(new Uri(photo_path));
      else
        reg_page.ImgReceiver.ImageSource = new BitmapImage(new Uri(@"pack://application:,,,/GymManagement;component/Resources/Images/photo_placeholder.png", UriKind.RelativeOrAbsolute));
    }

    public void UpdateSupplementField(RegistrationPaymentGroup pay_group)
    {
      if (pay_group.IsProteinSupplementPaymentLent())
      {
        reg_page.SupplementLendAmount.Visibility = Visibility.Visible;
        reg_page.SupplementLendAmountHeader.Visibility = Visibility.Visible;
        reg_page.SupplementLendDiscount.Visibility = Visibility.Visible;
        reg_page.SupplementLendDiscountHeader.Visibility = Visibility.Visible;
        reg_page.SupplementLendAndPayCancelButton.Visibility = Visibility.Visible;

        reg_page.SupplementDiscount.Visibility = Visibility.Collapsed;
        reg_page.SupplementDiscountHeader.Visibility = Visibility.Collapsed;
        reg_page.SupplementLendAndPayButton.Visibility = Visibility.Collapsed;
      }
      else
      {
        reg_page.SupplementLendAmount.Visibility = Visibility.Collapsed;
        reg_page.SupplementLendAmountHeader.Visibility = Visibility.Collapsed;
        reg_page.SupplementLendDiscount.Visibility = Visibility.Collapsed;
        reg_page.SupplementLendDiscountHeader.Visibility = Visibility.Collapsed;
        reg_page.SupplementLendAndPayCancelButton.Visibility = Visibility.Collapsed;

        reg_page.SupplementDiscount.Visibility = Visibility.Visible;
        reg_page.SupplementDiscountHeader.Visibility = Visibility.Visible;
        reg_page.SupplementLendAndPayButton.Visibility = Visibility.Visible;
      }
    }

    public void UpdateTotal(RegistrationPaymentGroup pay_group)
    {
      reg_page.Total.Text = pay_group.CalculateTotalPayment();
    }

    private void UpdatePaymentRelatedAmount(TextBox textbox, int? amount)
    {
      textbox.TextChanged -= reg_page.Amount_Changed;
      textbox.Text = amount == null ? "" : amount.ToString();
      textbox.CaretIndex = amount == null ? 0 : amount.ToString().Length;
      textbox.TextChanged += reg_page.Amount_Changed;
    }

    public void UpdatePaymentRelatedAmounts(string name, RegistrationPaymentGroup pay_group)
    {
      int? amount;
      switch (name)
      {
        case "SubscriptionAmount":
          amount = pay_group.gym_subscription_details_payment.amount;
          UpdatePaymentRelatedAmount(reg_page.SubscriptionAmount, amount);
          break;
        case "SubscriptionDiscount":
          amount = pay_group.gym_subscription_details_paydata.discount;
          UpdatePaymentRelatedAmount(reg_page.SubscriptionDiscount, amount);
          break;
        case "AdvanceAmount":
          amount = pay_group.advance_details_payment.amount;
          UpdatePaymentRelatedAmount(reg_page.AdvanceAmount, amount);
          break;
        case "AdvanceDiscount":
          amount = pay_group.advance_details_paydata.discount;
          UpdatePaymentRelatedAmount(reg_page.AdvanceDiscount, amount);
          break;
        case "SupplementAmount":
          amount = pay_group.protein_supplement_details_payment.amount;
          UpdatePaymentRelatedAmount(reg_page.SupplementAmount, amount);
          break;
        case "SupplementDiscount":
          amount = pay_group.protein_supplement_details_paydata.discount;
          UpdatePaymentRelatedAmount(reg_page.SupplementDiscount, amount);
          break;
        case "SupplementLendAmount":
          amount = pay_group.protein_supplement_details_lent_payment.amount;
          UpdatePaymentRelatedAmount(reg_page.SupplementLendAmount, amount);
          break;
        case "SupplementLendDiscount":
          amount = pay_group.protein_supplement_details_lent_paydata.discount;
          UpdatePaymentRelatedAmount(reg_page.SupplementLendDiscount, amount);
          break;
      }
    }

    private void UpdateTraineeDetail(TextBox textbox, string? value)
    {
      textbox.TextChanged -= reg_page.Text_Changed;
      textbox.Text = value == null ? "" : value;
      textbox.CaretIndex = value == null ? 0 : value.Length;
      textbox.TextChanged += reg_page.Text_Changed;
    }

    private void UpdateTraineeDetail(TextBox textbox, int? value)
    {
      textbox.TextChanged -= reg_page.Int_Changed;
      textbox.Text = value == null ? "" : value.ToString();
      textbox.CaretIndex = value == null ? 0 : value.ToString().Length;
      textbox.TextChanged += reg_page.Int_Changed;
    }

    public void UpdateTraineeDetails(string name, TraineeDetails trainee_details)
    {
      string? value;
      int? int_value;
      switch (name)
      {
        case "FirstName":
          value = trainee_details.profile_details.first_name;
          UpdateTraineeDetail(reg_page.FirstName, value);
          break;
        case "LastName":
          value = trainee_details.profile_details.last_name;
          UpdateTraineeDetail(reg_page.LastName, value);
          break;
        case "Age":
          int_value = trainee_details.profile_details.age;
          UpdateTraineeDetail(reg_page.Age, int_value);
          break;
        case "Comments":
          value = trainee_details.profile_details.comments;
          UpdateTraineeDetail(reg_page.Comments, value);
          break;
        case "Phone":
          value = trainee_details.contact_details.phone_no;
          UpdateTraineeDetail(reg_page.Phone, value);
          break;
        case "Email":
          value = trainee_details.contact_details.email;
          UpdateTraineeDetail(reg_page.Email, value);
          break;
        case "Address":
          value = trainee_details.contact_details.address;
          UpdateTraineeDetail(reg_page.Address, value);
          break;
        case "City":
          value = trainee_details.contact_details.city;
          UpdateTraineeDetail(reg_page.City, value);
          break;
      }
    }

    public void UpdateTraineeDetailsSex(ProfileDetails.Sex? value)
    {
      reg_page.SexMale.Checked -= reg_page.TraineeDetailsSexSelectionChanged;
      reg_page.SexFemale.Checked -= reg_page.TraineeDetailsSexSelectionChanged;

      if (value == ProfileDetails.Sex.Male)
        reg_page.SexMale.IsChecked = true;
      if (value == ProfileDetails.Sex.Female)
        reg_page.SexFemale.IsChecked = true;
      if (value == null)
      {
        reg_page.SexMale.IsChecked = false;
        reg_page.SexFemale.IsChecked = false;
      }

      reg_page.SexMale.Checked += reg_page.TraineeDetailsSexSelectionChanged;
      reg_page.SexFemale.Checked += reg_page.TraineeDetailsSexSelectionChanged;
    }

    public void SnackIt(string msg, string? action_str, Action? callback)
    {
      if (action_str != null)
        reg_page.SnackBar.MessageQueue.Enqueue(msg, action_str, callback);
      else
        reg_page.SnackBar.MessageQueue.Enqueue(msg);
    }
  }
}
