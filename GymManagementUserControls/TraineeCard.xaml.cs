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

namespace GymManagementUserControls
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class TraineeCard : UserControl
  {
    public TraineeCard(TraineeDetails td, RoutedEventHandler card_button_click)
    {
      InitializeComponent();

      CardButton.Click += card_button_click;
      TraineePhoto.ImageSource =  new BitmapImage(new Uri(td.profile_details.photo_path));
      TraineeFullName.Text = td.profile_details.full_name;
      if (td.contact_details.city == null)
      {
        TraineeCityIcon.Visibility = Visibility.Collapsed;
        TraineeCity.Text = "";
      }
      else
      {
        TraineeCityIcon.Visibility = Visibility.Visible;
        TraineeCity.Text = td.contact_details.city;
      }
      TraineeAbout.Text = td.profile_details.about;
    }

    public TraineeCard()
    {
      InitializeComponent();

      TraineePhoto.ImageSource =  new BitmapImage(new Uri(@"pack://application:,,,/GymManagement;component/Resources/Images/photo_placeholder.png", UriKind.RelativeOrAbsolute));
      TraineeFullName.Text = "aaaaaa aaaaaa";
      TraineeCityIcon.Visibility = Visibility.Visible;
      TraineeCity.Text = "aaaaa";
      TraineeAbout.Text = "aaaaaaaaaaaaaaaaaaaaaaaaaaaa aaaaaaaaaaaaaaaaaa.";
    }
  }
}
