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
using GymManagementHILogic;
using GymManagementLogic;

namespace GymManagementUserControls
{
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

    public KineticListing LoadState(System.Windows.Controls.Primitives.UniformGrid grid, KineticListingState kineticListingState)
    {
      UnloadState();

      Initialise(grid);

      this.kineticListingState = kineticListingState;
      kineticListingState.LoadKineticListingUpdates(kineticListingUpdates);
      return this;
    }

    public void UnloadState()
    {
      Finalise();

      if (kineticListingState != null)
      {
        kineticListingState.UnloadKineticListingUpdates();
        kineticListingState = null;
      }
    }

    /// Statefull

    public void Scroll(double horizontalScrollPosition, double verticalScrollPosition)
    {
      kineticListingState.UpdateScrollItemPositionByVerticalScrollPosition(verticalScrollPosition);
      kineticListingUpdates.UpdateHorizontalScrollPosition(horizontalScrollPosition);
    }

    public void UpdateLeftInc()
    {
      kineticListingState.UpdateLeftInc();
    }

    public void UpdateRightInc()
    {
      kineticListingState.UpdateRightInc();
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

    /// stateless

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
        inertiaProcessor.Drive(e.Key);
        e.Handled = true;
      }
      else if (e.Key == Key.Space)
      {
        inertiaProcessor.TogglePauseAndPlay();
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

    private void Initialise(System.Windows.Controls.Primitives.UniformGrid grid)
    {
      CollapseAndStopDropOverDrop();
      InitialiseDropOver();
      kineticListingUpdates.Initialise();
      inertiaProcessor = new InertiaHandler(
          new ScrollViewerAccess(scrollViewer),
          DrawDropOver,
          Scroll,
          UpdateLeftInc,
          UpdateRightInc
        );
      kineticListingUpdates.LoadGrid(grid);
    }

    private void Finalise()
    {
      if (inertiaProcessor != null)
      {
        inertiaProcessor.Dispose();
        inertiaProcessor = null;
      }
      kineticListingUpdates.UnloadGrid();
    }
  }

  public class ScrollViewerAccess : ScrollViewerAccessAbstract
  {
    public ScrollViewer scroller;

    public ScrollViewerAccess(ScrollViewer scroller)
    {
      this.scroller = scroller;
    }

    public override bool IsVerticallyCompletelyUnscrolled()
    {
      return scroller.VerticalOffset == 0;
    }

    public override bool IsVerticallyCompletelyScrolled()
    {
      return scroller.VerticalOffset == scroller.ScrollableHeight;
    }

    public override bool IsHorizontallyCompletelyUnscrolled()
    {
      return scroller.HorizontalOffset == 0;
    }

    public override bool IsHorizontallyCompletelyScrolled()
    {
      return scroller.HorizontalOffset == scroller.ScrollableWidth;
    }

    public override bool IsMouseCaptured()
    {
      return scroller.IsMouseCaptured;
    }

    public override double GetVerticalPosition()
    {
      return scroller.VerticalOffset;
    }

    public override double GetHorizontalPosition()
    {
      return scroller.HorizontalOffset;
    }

    public override GymManagementLogic.Point GetScrollerPosition()
    {
      var r = Mouse.GetPosition(scroller);
      return new GymManagementLogic.Point(r.X, r.Y);
    }
  }

  public class InertiaHandler : IDisposable
  {
    public delegate void VoidCallback();
    public delegate void Scroll(double posH, double posV);

    private int framesPerSecond = 60;

    DispatcherTimer animationTimer;
    KineticScrolling kineticScrolling;
    ScrollViewerAccessAbstract scroller;
    Scroll scroll;
    VoidCallback UpdateLeftInc;
    VoidCallback UpdateRightInc;
    VoidCallback DrawDropOver;

    public InertiaHandler(
        ScrollViewerAccessAbstract scroller,
        VoidCallback DrawDropOver,
        Scroll scroll,
        VoidCallback UpdateLeftInc,
        VoidCallback UpdateRightInc
      )
    {
      this.scroll = scroll;
      this.UpdateLeftInc = UpdateLeftInc;
      this.UpdateRightInc = UpdateRightInc;
      this.DrawDropOver = DrawDropOver;
      this.scroller = scroller;
      this.kineticScrolling = new KineticScrolling(scroller);
      animationTimer = new DispatcherTimer();
      animationTimer.Interval =
          new TimeSpan(0, 0, 0, 0, 1000 / framesPerSecond);
      animationTimer.Tick +=
          new EventHandler(HandleWorldTimerTick);
      animationTimer.Start();
    }

    public void Dispose()
    {
      animationTimer.Stop();
    }

    // All events shall be raised within here
    private void HandleWorldTimerTick(object sender,
        EventArgs e)

    {
      if (scroller.IsMouseCaptured())
      {
        kineticScrolling.AccelerateByFinger();
      }
      else if (kineticScrolling.driver.IsActive())
      {
        kineticScrolling.AccelerateByDriver();
      }

      if (!kineticScrolling.IsScrollerAtRest())
      {
        kineticScrolling.DeccelerateScroller();
        var vert = kineticScrolling.PlasticallyCollideVertically();
        var hori = kineticScrolling.PlasticallyCollideHorizontally();
        var pos = kineticScrolling.GetScrollerPosition();

        scroll(
            pos.X,
            pos.Y
          );
        if (vert < 0)
        {
          UpdateLeftInc();
        }
        else if (vert > 0)
        {
          UpdateRightInc();
        }
      }
    }

    public void FinishFingerContact()
    {
      if (kineticScrolling.finger.GetMaxDisplacement() == 0)
      {
        if (!kineticScrolling.IsScrollerAtRest())
        {
          kineticScrolling.Halt();
        }
        else
        {
          DrawDropOver();
        }
      }
    }

    public void InitiateFingerContact()
    {
      kineticScrolling.finger.InitiateFingerContact();
      if (kineticScrolling.driver.IsActive())
      {
        kineticScrolling.driver.Disable();
      }
    }

    public void Drive(Key key)
    {
      if (!kineticScrolling.driver.IsActive())
      {
        kineticScrolling.driver.Enable();
      }

      if (key == Key.Left)
      {
        kineticScrolling.driver.AccelerateRight();
      }
      else if (key == Key.Right)
      {
        kineticScrolling.driver.AccelerateLeft();
      }
      else if (key == Key.Up)
      {
        kineticScrolling.driver.AccelerateDown();
      }
      else if (key == Key.Down)
      {
        kineticScrolling.driver.AccelerateUp();
      }
    }

    public void TogglePauseAndPlay()
    {
      kineticScrolling.driver.TogglePauseAndPlay();
    }
  }
}
