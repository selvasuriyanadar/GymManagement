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
  public partial class TraineeListing : UserControl
  {
    TraineeListingState? trainee_listing_state;
    TraineeListingUpdates trainee_listing_updates;

    public TraineeListing()
    {
      InitializeComponent();
      trainee_listing_updates = new TraineeListingUpdates(this);
    }

    public TraineeListing LoadState(TraineeListingState trainee_listing_state)
    {
      UnloadState();
      this.trainee_listing_state = trainee_listing_state;
      this.trainee_listing_state.LoadTraineeListingUpdates(trainee_listing_updates);
      return this;
    }

    public void UnloadState()
    {
      if (trainee_listing_state != null)
      {
        trainee_listing_state.UnloadTraineeListingUpdates();
        trainee_listing_state = null;
      }
    }
  }
}
