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
using GymManagementDataStore;
using GymManagementHILogic;
using MaterialDesignThemes.Wpf;
using System.Globalization;

namespace GymManagementUserControls
{
  public class ProfilePagePaymentsGrid : DataLineGrid<PaymentDetails>
  {
    protected override UIElement GetUiItem(PaymentDetails d)
    {
      return new PaymentCard(d);
    }
  }

  public class ProfilePageUpdates
  {
    ProfilePage prof_page;

    KineticListing kineticListing = new KineticListing();

    public ProfilePageUpdates(ProfilePage prof_page)
    {
      this.prof_page = prof_page;
    }

    public void ResetComponent()
    {
    }

    public void UpdateListingControl(ProfilePagePaymentsGrid paymentsGrid, KineticListingState kineticListingState)
    {
      var grid = paymentsGrid.GetGrid();
      kineticListing.LoadState(grid, kineticListingState);
      prof_page.paymentsListingArea.Children.Clear();
      prof_page.paymentsListingArea.Children.Add(kineticListing);
    }

    public void UpdateTraineeView(TraineeDetails? td)
    {
      if (td != null)
      {
        prof_page.TraineeName.Text = td.profile_details.GetFullName();
        prof_page.ImgReceiver.ImageSource = new BitmapImage(new Uri(td.profile_details.photo_path));
        prof_page.TraineeAbout.Text = td.profile_details.GetAbout();

        if (td.contact_details.city == null)
        {
          prof_page.TraineeCityIcon.Visibility = Visibility.Hidden;
          prof_page.TraineeCity.Text = "";
        }
        else
        {
          prof_page.TraineeCityIcon.Visibility = Visibility.Visible;
          prof_page.TraineeCity.Text = td.contact_details.city;
        }

        if (td.contact_details.phone_no == null)
        {
          prof_page.TraineePhoneButton.Visibility = Visibility.Collapsed;
        }
        else
        {
          prof_page.TraineePhoneButton.Visibility = Visibility.Visible;
        }

        if (td.contact_details.email == null)
        {
          prof_page.TraineeEmailButton.Visibility = Visibility.Collapsed;
        }
        else
        {
          prof_page.TraineeEmailButton.Visibility = Visibility.Visible;
        }

        if (td.contact_details.address == null)
        {
          prof_page.TraineeAddressButton.Visibility = Visibility.Collapsed;
        }
        else
        {
          prof_page.TraineeAddressButton.Visibility = Visibility.Visible;
        }
      }
      else
      {
        prof_page.TraineeName.Text = "";
        prof_page.ImgReceiver.ImageSource = null;
        prof_page.TraineeAbout.Text = "";
        prof_page.TraineeCityIcon.Visibility = Visibility.Hidden;
        prof_page.TraineeCity.Text = "";
        prof_page.TraineePhoneButton.Visibility = Visibility.Collapsed;
        prof_page.TraineeEmailButton.Visibility = Visibility.Collapsed;
        prof_page.TraineeAddressButton.Visibility = Visibility.Collapsed;
      }
    }
  }
}
