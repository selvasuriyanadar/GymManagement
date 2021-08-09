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
using System.Windows.Threading;
using GymManagementDataModel;
using GymManagementDataStore;
using GymManagementHILogic;

namespace GymManagementUserControls
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class KineticListing : UserControl
  {
    InertiaHandler inertiaProcessor;
    DispatcherTimer dropOverTimer;
    DispatcherTimer dropOverAnimationTimer;
    DispatcherTimer dropOverDarkeningTimer;
    DispatcherTimer dropOverDelayTimer;

    KineticListingUpdates kineticListingUpdates;
    KineticListingState? kineticListingState;

    public KineticListing()
    {
      InitializeComponent();
      dropOverTimer = new DispatcherTimer();
      dropOverAnimationTimer = new DispatcherTimer();
      dropOverDarkeningTimer = new DispatcherTimer();
      dropOverDelayTimer = new DispatcherTimer();
      dropOverTimer.Interval =
          new TimeSpan(0, 0, 0, 0, 3000);
      dropOverAnimationTimer.Interval =
          new TimeSpan(0, 0, 0, 0, 20);
      dropOverDarkeningTimer.Interval =
          new TimeSpan(0, 0, 0, 0, 20);
      dropOverDelayTimer.Interval =
          new TimeSpan(0, 0, 0, 0, 300);
      dropOverTimer.Tick +=
          new EventHandler(HandleDropOverTimerTick);
      dropOverAnimationTimer.Tick +=
          new EventHandler(HandleDropOverAnimationTimerTick);
      dropOverDarkeningTimer.Tick +=
          new EventHandler(HandleDropOverDarkeningTimerTick);
      dropOverDelayTimer.Tick +=
          new EventHandler(HandleDropOverDelayTimerTick);
      kineticListingUpdates = new KineticListingUpdates(this);
    }

    private void Initialise()
    {
      InitialiseDropOver();
      CollapseDropOver();
    }

    public KineticListing LoadState(System.Windows.Controls.Primitives.UniformGrid grid, KineticListingState kineticListingState)
    {
      UnloadState();

      Initialise();
      this.kineticListingState = kineticListingState;
      inertiaProcessor = new InertiaHandler(scrollViewer,
          DrawDropOver,
          Scroll,
          kineticListingState.UpdateLeftInc,
          kineticListingState.UpdateRightInc
        );
      kineticListingState.LoadKineticListingUpdates(kineticListingUpdates, grid);
      return this;
    }

    public void UnloadState()
    {
      if (kineticListingState != null)
      {
        kineticListingState.UnloadKineticListingUpdates();
        inertiaProcessor.Dispose();
        inertiaProcessor = null;
        kineticListingState = null;
      }
    }

    public void Scroll(double horizontalScrollPosition, double verticalScrollPosition)
    {
      kineticListingState.UpdateScrollItemPositionByVerticalScrollPosition(verticalScrollPosition);
      kineticListingUpdates.UpdateHorizontalScrollPosition(horizontalScrollPosition);
    }

    public void Slider_ValueChanged(
        object sender,
        RoutedPropertyChangedEventArgs<double> e)
    {
      ResetDropOverDrop();
      kineticListingState.UpdateScrollItemPosition((long) e.NewValue);
    }

    private void OnRefresh_Clicked(object sender, RoutedEventArgs e)
    {
      ResetDropOverDrop();
      kineticListingState.RefreshAtCurrentItemPosition();
    }

    private void OnExpandSlider_Clicked(object sender, RoutedEventArgs e)
    {
      ResetDropOverDrop();
      kineticListingState.UpdateExpandSlider(true);
    }

    private void OnCollapseSlider_Clicked(object sender, RoutedEventArgs e)
    {
      ResetDropOverDrop();
      kineticListingState.UpdateExpandSlider(false);
    }

    private void OnPreviewMouseDown(object sender,
        MouseButtonEventArgs e)
    {
      if (scrollViewer.IsMouseOver && e.ClickCount == 1)
      {
        inertiaProcessor.InitiateFingerContact();
        scrollViewer.CaptureMouse();
      }
    }

    private void OnPreviewMouseUp(object sender,
        MouseButtonEventArgs e)
    {
      if (scrollViewer.IsMouseCaptured)
      {
        scrollViewer.ReleaseMouseCapture();
        inertiaProcessor.FinishFingerContact();
      }
    }

    private void OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
      {
        if (e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.Up || e.Key == Key.Down)
        {
          e.Handled = true;
        }
      }

      if (e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.Up || e.Key == Key.Down)
      {
        if (!inertiaProcessor.driver.IsActive())
        {
          inertiaProcessor.driver.Enable();
        }

        if (e.Key == Key.Left)
        {
          inertiaProcessor.driver.AccelerateRight();
        }
        else if (e.Key == Key.Right)
        {
          inertiaProcessor.driver.AccelerateLeft();
        }
        else if (e.Key == Key.Up)
        {
          inertiaProcessor.driver.AccelerateDown();
        }
        else if (e.Key == Key.Down)
        {
          inertiaProcessor.driver.AccelerateUp();
        }

        e.Handled = true;
      }
      else if (e.Key == Key.Space)
      {
        inertiaProcessor.driver.TogglePauseAndPlay();
        e.Handled = true;
      }
    }

    private void OnMouseDownDropOver(object sender,
        MouseButtonEventArgs e)
    {
      CollapseAndStopDropOverDrop();
      e.Handled = true;
    }

    private void OnMouseDownFullScreenLoader(object sender,
        MouseButtonEventArgs e)
    {
      DrawDropOver();
      e.Handled = true;
    }

    public void CollapseDropOver()
    {
      dropOver.Visibility = Visibility.Collapsed;
      scrollViewer.Focus();
      Keyboard.Focus(scrollViewer);
    }

    private void CollapseAndStopDropOverDrop()
    {
      CollapseDropOver();
      dropOverTimer.Stop();
    }

    private void ResetDropOverDrop()
    {
      dropOverTimer.Stop();
      dropOverTimer.Start();
    }

    private void InitialiseDropOver()
    {
      dropOver.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
      dropOverTop.Margin = new Thickness(0, -100, 0, 0);
      dropOverLeft.Margin = new Thickness(-100, 0, 0, 0);
    }

    public void DrawDropOver()
    {
      dropOverDelayTimer.Start();
    }

    private void HandleDropOverDelayTimerTick(object sender, EventArgs e)
    {
      dropOver.Visibility = Visibility.Visible;
      dropOverTimer.Start();
      InitialiseDropOver();
      dropOverAnimationTimer.Start();
      dropOverDelayTimer.Stop();
    }

    private void HandleDropOverTimerTick(object sender, EventArgs e)
    {
      CollapseAndStopDropOverDrop();
    }

    private void HandleDropOverAnimationTimerTick(object sender, EventArgs e)
    {
      byte inc = 25;

      Thickness t = dropOverTop.Margin;
      byte a = ((SolidColorBrush)dropOver.Background).Color.A;
      if (t.Top < 0)
      {
        dropOverTop.Margin = new Thickness(0, t.Top + inc, 0, 0);
        dropOverLeft.Margin = new Thickness(t.Top + inc, 0, 0, 0);
        dropOver.Background = new SolidColorBrush(Color.FromArgb((byte)(a + 5), 106, 106, 106));
      }
      else
      {
        dropOver.Background = new SolidColorBrush(Color.FromArgb(20, 106, 106, 106));
        dropOverTop.Margin = new Thickness(0, 0, 0, 0);
        dropOverLeft.Margin = new Thickness(0, 0, 0, 0);
        dropOverAnimationTimer.Stop();
        dropOverDarkeningTimer.Start();
      }
    }

    private void HandleDropOverDarkeningTimerTick(object sender, EventArgs e)
    {
      byte a = ((SolidColorBrush)dropOver.Background).Color.A;
      if (a < 120)
      {
        dropOver.Background = new SolidColorBrush(Color.FromArgb((byte)(a + 10), 106, 106, 106));
      }
      else
      {
        dropOver.Background = new SolidColorBrush(Color.FromArgb(120, 106, 106, 106));
        dropOverDarkeningTimer.Stop();
      }
    }
  }
}
