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
  public partial class ProfilePage : UserControl
  {
    ProfilePageState? profile_page_state;
    ProfilePageUpdates prof_page_updates;

    public ProfilePage()
    {
      InitializeComponent();
      prof_page_updates = new ProfilePageUpdates(this);
    }

    public ProfilePage LoadState(ProfilePageState profile_page_state)
    {
      UnloadState();
      this.profile_page_state = profile_page_state;
      this.profile_page_state.LoadProfilePageUpdates(prof_page_updates);
      return this;
    }

    public void UnloadState()
    {
      if (profile_page_state != null)
      {
        profile_page_state.UnloadProfilePageUpdates();
        profile_page_state = null;
      }
    }
  }
}
