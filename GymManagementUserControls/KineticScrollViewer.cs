using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace GymManagementUserControls
{
  public class InertiaHandler : IDisposable
  {
    public delegate void VoidCallback();
    public delegate void Scroll(double posH, double posV);
    VoidCallback DrawDropOver;
    Scroll scroll;
    VoidCallback UpdateLeftInc;
    VoidCallback UpdateRightInc;

    private double staticFrictionalCoefficient = 0.06;
    private double kineticFrictionalCoefficient = 0.04;
    private double normalForce = 9.8;
    private double relativeMassOfScrollerToHumanFinger = 1.8;
    private double dampingConstant = 0.02;
    private int framesPerSecond = 60;
    private Vector scrollerVelocity = new Vector(0, 0);

    public Finger finger;
    public Driver driver;
    ScrollViewer scroller;
    DispatcherTimer animationTimer;

    public InertiaHandler(ScrollViewer scroller,
        VoidCallback DrawDropOver,
        Scroll scroll,
        VoidCallback UpdateLeftInc,
        VoidCallback UpdateRightInc
      )
    {
      this.scroller = scroller;
      this.DrawDropOver = DrawDropOver;
      this.scroll = scroll;
      this.UpdateLeftInc = UpdateLeftInc;
      this.UpdateRightInc = UpdateRightInc;
      finger = new Finger(scroller);
      driver = new Driver(GetStaticFrictionalForce());
      animationTimer = new DispatcherTimer();
      animationTimer.Interval =
          new TimeSpan(0, 0, 0, 0, 1000 / framesPerSecond);
      animationTimer.Tick +=
          new EventHandler(HandleWorldTimerTick);
      animationTimer.Start();
    }

    private double GetStaticFrictionalForce()
    {
      return staticFrictionalCoefficient * normalForce;
    }

    private double GetKineticFrictionalForce()
    {
      return kineticFrictionalCoefficient * normalForce;
    }

    private double GetScrollerMass()
    {
      return relativeMassOfScrollerToHumanFinger * finger.GetMass();
    }

    private Vector GetAccelerationByExternalForce(Vector force)
    {
      return (force / GetScrollerMass());
    }

    private double GetAngularFrequencyOfOscillationForCriticallyDamping()
    {
      return dampingConstant / 2;
    }

    private bool IsScrollerFreeFromStaticFriction(Vector force)
    {
      return force.Length > GetStaticFrictionalForce();
    }

    private bool IsScrollerAtRest()
    {
      return scrollerVelocity.Length == 0;
    }

    private Vector GetKineticDecceleration(Vector u)
    {
      return -(u * (GetKineticFrictionalForce() / GetScrollerMass()));
    }

    private Vector GetKineticDecceleration2(Vector u)
    {
      var dc = GetAngularFrequencyOfOscillationForCriticallyDamping();
      return -(u * (dc * dc));
    }

    private void AccelerateScrollerHorizontally(Vector a, Vector f)
    {
      if (IsScrollerFreeFromStaticFriction(f) || !IsScrollerAtRest())
      {
        scrollerVelocity.X += a.X;
      }
    }

    private void AccelerateScrollerVertically(Vector a, Vector f)
    {
      if (IsScrollerFreeFromStaticFriction(f) || !IsScrollerAtRest())
      {
        scrollerVelocity.Y += a.Y;
      }
    }

    private void AccelerateScrollerHorizontallyByFinger()
    {
      var f = finger.GetForce();
      var a = GetAccelerationByExternalForce(f);

      if (!((bool) finger.DidSlipHorizontally())
        && ((finger.GetDirectionOfMotion().X > 0 && a.X > 0)
          || (finger.GetDirectionOfMotion().X < 0 && a.X < 0))
        )
      {
        AccelerateScrollerHorizontally(a, f);
      }
    }

    private void AccelerateScrollerVerticallyByFinger()
    {
      var f = finger.GetForce();
      var a = GetAccelerationByExternalForce(f);

      if (!((bool) finger.DidSlipVertically())
        && ((finger.GetDirectionOfMotion().Y > 0 && a.Y > 0)
          || (finger.GetDirectionOfMotion().Y < 0 && a.Y < 0))
        )
      {
        AccelerateScrollerVertically(a, f);
      }
    }

    private void DeccelerateScroller()
    {
      if (scrollerVelocity.Length != 0)
      {
        var u = (scrollerVelocity / scrollerVelocity.Length);
        var d = GetKineticDecceleration2(u);
        var fd = -(scrollerVelocity * dampingConstant);

        var r = (d + fd);
        if (r.Length > scrollerVelocity.Length)
          scrollerVelocity += fd;
        else
          scrollerVelocity += r;
      }
    }

    private void ElasticallyCollideVertically()
    {
      if (((scroller.VerticalOffset == 0) && (scrollerVelocity.Y > 0))
        || ((scroller.VerticalOffset == scroller.ScrollableHeight) && (scrollerVelocity.Y < 0))
        )
      {
        scrollerVelocity.Y = -scrollerVelocity.Y;
      }
    }

    private void ElasticallyCollideHorizontally()
    {
      if (((scroller.HorizontalOffset == 0) && (scrollerVelocity.X > 0))
        || ((scroller.HorizontalOffset == scroller.ScrollableWidth) && (scrollerVelocity.X < 0))
        )
      {
        scrollerVelocity.X = -scrollerVelocity.X;
      }
    }

    private int PlasticallyCollideVertically()
    {
      int result = 0;
      if (((scroller.VerticalOffset == 0) && (scrollerVelocity.Y > 0))
        || ((scroller.VerticalOffset == scroller.ScrollableHeight) && (scrollerVelocity.Y < 0))
        )
      {
        if (scrollerVelocity.Y > 0)
        {
          result = -1;
        }
        else if (scrollerVelocity.Y < 0)
        {
          result = 1;
        }

        scrollerVelocity.Y = 0;
        driver.VerticalHalt();
      }
      return result;
    }

    private int PlasticallyCollideHorizontally()
    {
      int result = 0;
      if (((scroller.HorizontalOffset == 0) && (scrollerVelocity.X > 0))
        || ((scroller.HorizontalOffset == scroller.ScrollableWidth) && (scrollerVelocity.X < 0))
        )
      {
        if (scrollerVelocity.X > 0)
        {
          result = -1;
        }
        else if (scrollerVelocity.X < 0)
        {
          result = 1;
        }

        scrollerVelocity.X = 0;
        driver.HorizontalHalt();
      }
      return result;
    }

    private void Halt()
    {
      scrollerVelocity = new Vector(0, 0);
    }

    public void FinishFingerContact()
    {
      if (finger.GetMaxDisplacement() == 0)
      {
        if (scrollerVelocity.Length != 0)
        {
          Halt();
        }
        else
        {
          DrawDropOver();
        }
      }
    }

    public void InitiateFingerContact()
    {
      finger.InitiateFingerContact();
      if (driver.IsActive())
      {
        driver.Disable();
      }
    }

    // All events shall be raised within here
    private void HandleWorldTimerTick(object sender,
        EventArgs e)
    {
        if (scroller.IsMouseCaptured)
        {
            finger.CaptureMotion();
            AccelerateScrollerHorizontallyByFinger();
            AccelerateScrollerVerticallyByFinger();
        }
        else if (driver.IsActive())
        {
          var driver_force = driver.GetForce();
          AccelerateScrollerHorizontally(GetAccelerationByExternalForce(driver_force), driver_force);
          AccelerateScrollerVertically(GetAccelerationByExternalForce(driver_force), driver_force);
        }

        if (!IsScrollerAtRest())
        {
          DeccelerateScroller();
          var vert = PlasticallyCollideVertically();
          var hori = PlasticallyCollideHorizontally();
          //ElasticallyCollideVertically();
          //ElasticallyCollideHorizontally();

          scroll(
              scroller.HorizontalOffset - scrollerVelocity.X,
              scroller.VerticalOffset - scrollerVelocity.Y
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

    #region IDisposable Members

    public void Dispose()
    {
      animationTimer.Stop();
    }

    #endregion
  }

  public class Finger
  {
    private bool fingerSlippedHorizontally;
    private bool fingerSlippedVertically;
    private Point position;
    private Vector velocity;
    private Vector maxVelocity;
    private Vector acceleration;
    private double massOfFinger = 1;

    ScrollViewer scroller;

    public Finger(ScrollViewer scroller)
    {
      this.scroller = scroller;
    }

    public void InitiateFingerContact()
    {
      fingerSlippedHorizontally = false;
      fingerSlippedVertically = false;
      position = Mouse.GetPosition(scroller);
      velocity = new Vector(0, 0);
      maxVelocity = new Vector(0, 0);
      acceleration = new Vector(0, 0);
    }

    public double GetMass()
    {
      return massOfFinger;
    }

    public Vector GetForce()
    {
      return acceleration * massOfFinger;
    }

    public double GetMaxDisplacement()
    {
      return maxVelocity.Length;
    }

    public bool DidSlipHorizontally()
    {
      return fingerSlippedHorizontally;
    }

    public bool DidSlipVertically()
    {
      return fingerSlippedVertically;
    }

    public Vector GetDirectionOfMotion()
    {
      if (velocity.X != 0 && velocity.Y != 0)
        return velocity / velocity.Length;
      else if (velocity.X == 0)
        return new Vector(0, velocity.Y / Math.Abs(velocity.Y));
      else if (velocity.Y == 0)
        return new Vector(velocity.X / Math.Abs(velocity.X), 0);
      else
        return new Vector(0, 0);
    }

    private Vector GetVelocity(Point prev, Point curr)
    {
      return curr - prev;
    }

    private Vector GetAcceleration(Vector prev, Vector curr)
    {
      return curr - prev;
    }

    private void CheckFingerSlippedHorizontally()
    {
      if ((scroller.HorizontalOffset == 0 && acceleration.X > 0)
        || (scroller.HorizontalOffset == scroller.ScrollableWidth && acceleration.X < 0)
        )
      {
        fingerSlippedHorizontally = true;
      }
      else
      {
        fingerSlippedHorizontally = false;
      }
    }

    private void CheckFingerSlippedVertically()
    {
      if ((scroller.VerticalOffset == 0 && acceleration.Y > 0)
        || (scroller.VerticalOffset == scroller.ScrollableHeight && acceleration.Y < 0)
        )
      {
        fingerSlippedVertically = true;
      }
      else
      {
        fingerSlippedVertically = false;
      }
    }

    private void CheckMaxVelocity()
    {
      if (velocity.Length > maxVelocity.Length)
      {
        maxVelocity = velocity;
      }
    }

    public void CaptureMotion()
    {
      Point currentPoint = Mouse.GetPosition(scroller);
      Vector currentVelocity = GetVelocity(position, currentPoint);

      acceleration = GetAcceleration(velocity, currentVelocity);
      velocity = currentVelocity;
      position = currentPoint;
      CheckMaxVelocity();
      CheckFingerSlippedHorizontally();
      CheckFingerSlippedVertically();
    }
  }

  public class Driver
  {
    double mass = 5;
    double boost = 0.0002;
    double groundBoost;
      
    bool driverActive = false;
    Vector acceleration = new Vector(0, 0);

    public Driver(double staticFrictionalForce)
    {
      groundBoost = staticFrictionalForce / mass;
    }

    public bool IsActive()
    {
      return driverActive;
    }

    public void Enable()
    {
      driverActive = true;
    }

    public void Disable()
    {
      Halt();
      driverActive = false;
    }

    public Vector GetAcceleration()
    {
      return acceleration;
    }

    public Vector GetForce()
    {
      return acceleration * mass;
    }

    public double GetMass()
    {
      return mass;
    }

    public void AccelerateLeft()
    {
      if (acceleration.X > -groundBoost)
      {
        acceleration.X = -groundBoost;
      }
      acceleration.X -= boost;
    }

    public void AccelerateRight()
    {
      if (acceleration.X < groundBoost)
      {
        acceleration.X = groundBoost;
      }
      acceleration.X += boost;
    }

    public void AccelerateUp()
    {
      if (acceleration.Y > -groundBoost)
      {
        acceleration.Y = -groundBoost;
      }
      acceleration.Y -= boost;
    }

    public void AccelerateDown()
    {
      if (acceleration.Y < groundBoost)
      {
        acceleration.Y = groundBoost;
      }
      acceleration.Y += boost;
    }

    public void VerticalHalt()
    {
      acceleration.Y = 0;
    }

    public void HorizontalHalt()
    {
      acceleration.X = 0;
    }

    public void Halt()
    {
      acceleration = new Vector(0,0);
    }

    public void TogglePauseAndPlay()
    {
      if (IsActive())
      {
        Disable();
      }
      else
      {
        Enable();
      }
    }
  }
}
