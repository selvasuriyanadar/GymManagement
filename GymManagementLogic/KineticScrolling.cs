using System;
using System.Collections.Generic;
using GymManagementDataModel;

namespace GymManagementLogic
{
  public class Vector
  {
    public double X;
    public double Y;

    public Vector(double X, double Y)
    {
      this.X = X;
      this.Y = Y;
    }

    public double Length { get { return Math.Sqrt((X * X) + (Y * Y)); } }

    public static Vector operator /(Vector a, double b)
    {
      return new Vector(
          a.X / b,
          a.Y / b
        );
    }

    public static Vector operator *(Vector a, double b)
    {
      return new Vector(
          a.X * b,
          a.Y * b
        );
    }

    public static Vector operator -(Vector a)
    {
      return new Vector(
          -a.X,
          -a.Y
        );
    }

    public static Vector operator -(Vector a, Vector b)
    {
      return new Vector(
          a.X - b.X,
          a.Y - b.Y
        );
    }

    public static Vector operator +(Vector a, Vector b)
    {
      return new Vector(
          a.X + b.X,
          a.Y + b.Y
        );
    }
  }

  public class Point
  {
    public double X;
    public double Y;

    public Point(double X, double Y)
    {
      this.X = X;
      this.Y = Y;
    }

    public static Vector operator -(Point a, Point b)
    {
      return new Vector(
          a.X - b.X,
          a.Y - b.Y
        );
    }
  }

  public abstract class ScrollViewerAccessAbstract
  {
    public abstract bool IsVerticallyCompletelyUnscrolled();
    public abstract bool IsVerticallyCompletelyScrolled();
    public abstract bool IsHorizontallyCompletelyUnscrolled();
    public abstract bool IsHorizontallyCompletelyScrolled();
    public abstract bool IsMouseCaptured();
    public abstract double GetVerticalPosition();
    public abstract double GetHorizontalPosition();
    public abstract Point GetScrollerPosition();
  }

  public class KineticScrolling
  {
    private double staticFrictionalCoefficient = 0.06;
    private double kineticFrictionalCoefficient = 0.04;
    private double normalForce = 9.8;
    private double relativeMassOfScrollerToHumanFinger = 1.8;
    private double dampingConstant = 0.02;
    private Vector scrollerVelocity = new Vector(0, 0);

    public Finger finger;
    public Driver driver;
    ScrollViewerAccessAbstract scroller;

    public KineticScrolling(
        ScrollViewerAccessAbstract scroller
      )
    {
      this.scroller = scroller;
      finger = new Finger(this.scroller);
      driver = new Driver(GetStaticFrictionalForce());
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

    public bool IsScrollerAtRest()
    {
      return scrollerVelocity.Length == 0;
    }

    public void DeccelerateScroller()
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
      if (((scroller.IsVerticallyCompletelyUnscrolled()) && (scrollerVelocity.Y > 0))
        || ((scroller.IsVerticallyCompletelyScrolled()) && (scrollerVelocity.Y < 0))
        )
      {
        scrollerVelocity.Y = -scrollerVelocity.Y;
      }
    }

    private void ElasticallyCollideHorizontally()
    {
      if (((scroller.IsHorizontallyCompletelyUnscrolled()) && (scrollerVelocity.X > 0))
        || ((scroller.IsHorizontallyCompletelyScrolled()) && (scrollerVelocity.X < 0))
        )
      {
        scrollerVelocity.X = -scrollerVelocity.X;
      }
    }

    public int PlasticallyCollideVertically()
    {
      int result = 0;
      if (((scroller.IsVerticallyCompletelyUnscrolled()) && (scrollerVelocity.Y > 0))
        || ((scroller.IsVerticallyCompletelyScrolled()) && (scrollerVelocity.Y < 0))
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

    public int PlasticallyCollideHorizontally()
    {
      int result = 0;
      if (((scroller.IsHorizontallyCompletelyUnscrolled()) && (scrollerVelocity.X > 0))
        || ((scroller.IsHorizontallyCompletelyScrolled()) && (scrollerVelocity.X < 0))
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

    public void Halt()
    {
      scrollerVelocity = new Vector(0, 0);
    }

    public Point GetScrollerPosition()
    {
      return new Point(
          scroller.GetHorizontalPosition() - scrollerVelocity.X,
          scroller.GetVerticalPosition() - scrollerVelocity.Y
        );
    }

    public void AccelerateByFinger()
    {
      finger.CaptureMotion();
      AccelerateScrollerHorizontallyByFinger();
      AccelerateScrollerVerticallyByFinger();
    }

    public void AccelerateByDriver()
    {
      var driver_force = driver.GetForce();
      AccelerateScrollerHorizontally(GetAccelerationByExternalForce(driver_force), driver_force);
      AccelerateScrollerVertically(GetAccelerationByExternalForce(driver_force), driver_force);
    }
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

    ScrollViewerAccessAbstract scroller;

    public Finger(ScrollViewerAccessAbstract scroller)
    {
      this.scroller = scroller;
    }

    public void InitiateFingerContact()
    {
      fingerSlippedHorizontally = false;
      fingerSlippedVertically = false;
      position = scroller.GetScrollerPosition();
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
      if ((scroller.IsHorizontallyCompletelyUnscrolled() && acceleration.X > 0)
        || (scroller.IsHorizontallyCompletelyScrolled() && acceleration.X < 0)
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
      if ((scroller.IsVerticallyCompletelyUnscrolled() && acceleration.Y > 0)
        || (scroller.IsVerticallyCompletelyScrolled() && acceleration.Y < 0)
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
      Point currentPoint = scroller.GetScrollerPosition();
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
