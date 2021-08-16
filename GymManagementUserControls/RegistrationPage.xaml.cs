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
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// Always LoadState shall be called before showing the usercontrol to the user
  /// </summary>
  public partial class RegistrationPage : UserControl
  {
    TraineeRegistrationState? trainee_reg_state;
    RegistrationPageUpdates reg_page_updates;

    public RegistrationPage()
    {
      InitializeComponent();
      reg_page_updates = new RegistrationPageUpdates(this);
    }

    public RegistrationPage LoadState(TraineeRegistrationState trainee_reg_state)
    {
      UnloadState();

      ResetComponent();

      this.trainee_reg_state = trainee_reg_state;
      this.trainee_reg_state.LoadRegistrationPageUpdates(reg_page_updates);
      return this;
    }

    public void UnloadState()
    {
      if (trainee_reg_state != null)
      {
        trainee_reg_state.UnloadRegistrationPageUpdates();
        trainee_reg_state = null;
      }
    }

    ///Event + Controller

    // Clicked

    public void ImageButton_Click(object sender, RoutedEventArgs e)
    {
      trainee_reg_state.SelectPhoto();
    }

    public void OnSupplementLendAndPayButton_Clicked(object sender, RoutedEventArgs e)
    {
      trainee_reg_state.EnableSupplementLendAndPay();
    }

    public void OnSupplementLendAndPayCancelButton_Clicked(object sender, RoutedEventArgs e)
    {
      trainee_reg_state.DisableSupplementLendPayment();
    }

    public void OnSubmitButton_Clicked(object sender, RoutedEventArgs e)
    {
      trainee_reg_state.Submit(true);
    }

    public void OnPayLaterButton_Clicked(object sender, RoutedEventArgs e)
    {
      trainee_reg_state.Submit(false);
    }

    public void OnRegisterAgainButton_Clicked(object sender, RoutedEventArgs e)
    {
      trainee_reg_state.openRegistrationPage();
    }

    // Changed

    public void JoiningDate_Changed(object sender, SelectionChangedEventArgs e)
    {
      DateTime? result = null;
      if (((DatePicker)sender).SelectedDate != null)
        result = (((DatePicker)sender).SelectedDate).Value.Date;

      ((DatePicker)sender).SelectedDateChanged -= JoiningDate_Changed;
      ((DatePicker)sender).SelectedDate = null;
      ((DatePicker)sender).SelectedDateChanged += JoiningDate_Changed;

      trainee_reg_state.EnterJoiningDate(result);

      e.Handled = true;
    }

    public void Text_Changed(object sender, TextChangedEventArgs e)
    {
      var result = String.IsNullOrEmpty(((TextBox)sender).Text) ? null : ((TextBox)sender).Text;

      ((TextBox)sender).TextChanged -= Text_Changed;
      ((TextBox)sender).Text = "";
      ((TextBox)sender).TextChanged += Text_Changed;

      trainee_reg_state.EnterTraineeDetails((e.Source as TextBox).Name, result);

      e.Handled = true;
    }

    public void Int_Changed(object sender, TextChangedEventArgs e)
    {
      var result = ConvertStringToInt(((TextBox)sender).Text);

      ((TextBox)sender).TextChanged -= Int_Changed;
      ((TextBox)sender).Text = "";
      ((TextBox)sender).TextChanged += Int_Changed;

      if (result.Item2)
      {
        trainee_reg_state.EnterTraineeDetails(((e.Source as TextBox).Name), result.Item1);
      }
      else
      {
        trainee_reg_state.SyncTraineeDetails(((e.Source as TextBox).Name));
      }

      e.Handled = true;
    }

    public void Amount_Changed(object sender, TextChangedEventArgs e)
    {
      var result = ConvertStringToInt(((TextBox)sender).Text);

      ((TextBox)sender).TextChanged -= Amount_Changed;
      ((TextBox)sender).Text = "";
      ((TextBox)sender).TextChanged += Amount_Changed;

      if (result.Item2)
      {
        trainee_reg_state.EnterPaymentRelatedAmounts(((e.Source as TextBox).Name), result.Item1);
      }
      else
      {
        trainee_reg_state.SyncPaymentRelatedAmounts(((e.Source as TextBox).Name));
      }

      e.Handled = true;
    }

    public void TraineeDetailsSexSelectionChanged(object sender, RoutedEventArgs e)
    {
      var result = GetTraineeDetailsSexSelection();

      SexMale.Checked -= TraineeDetailsSexSelectionChanged;
      SexFemale.Checked -= TraineeDetailsSexSelectionChanged;
      ClearTraineeDetailsSexSelection();
      SexMale.Checked += TraineeDetailsSexSelectionChanged;
      SexFemale.Checked += TraineeDetailsSexSelectionChanged;

      trainee_reg_state.EnterTraineeDetailsSex(result);

      e.Handled = true;
    }

    ///UILIB

    // Filters

    private (int?, bool) ConvertStringToInt(string text)
    {
      if(!String.IsNullOrEmpty(text))
      {
        try
        {
          return (int.Parse(text), true);
        }
        catch
        {
          return (null, false);
        }
      }
      else
      {
        return (null, true);
      }
    }

    /*
    private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
    {
      if (!String.IsNullOrEmpty(e.Text))
        e.Handled = !int.TryParse(e.Text, out int result);
    }

    private void TextBoxPasting(object sender, DataObjectPastingEventArgs e)
    {
      if (e.DataObject.GetDataPresent(typeof(String)))
      {
        String text = (String)e.DataObject.GetData(typeof(String));
        if (!String.IsNullOrEmpty(text) && !int.TryParse(text, out int result))
        {
          e.CancelCommand();
        }
      }
      else
      {
        e.CancelCommand();
      }
    }
    */

    // Inputs

    private ProfileDetails.Sex? GetTraineeDetailsSexSelection()
    {
      if (SexMale.IsChecked != null && (bool)(SexMale.IsChecked))
      {
        return ProfileDetails.Sex.Male;
      }
      else if (SexFemale.IsChecked != null && (bool)(SexFemale.IsChecked))
      {
        return ProfileDetails.Sex.Female;
      }
      else
        return null;

    }

    private void ClearTraineeDetailsSexSelection()
    {
      SexMale.IsChecked = false;
      SexFemale.IsChecked = false;
    }

    // stateless
    //
    public void ResetComponent()
    {
      TraineeDetailsTransitioner.SelectedIndex = 0;
    }
  }
}
