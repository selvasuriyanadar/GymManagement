using System;
using System.Collections.Generic;
using GymManagementDataModel;

namespace GymManagementLogic
{
  // an interval can be in Peace or Alert
  // if this interval is in Peace then the payment can be in Peace
  // if this interval is in Alert then the payment can be in Today or Alert
  // Upcomming and the NotiStats of the current interval can occur simultaneously
  // if next interval is in Alert then the payment can be in Upcomming
  //
  // for all payments a start interval with start date at negative infinity exists and the interval is only in Peace
  // for UnPaid payment the last (also the second) interval ends at positive infinity and the interval is only in Alert
  // for Lent payment the second and following intervals are finite, the intervals can alternate between Peace and Alert

  public abstract class Notification
  {
    private int upcomming_interval = 5;

    public enum IntvStat
    {
      Peace,
      Alert,
    }

    public enum NotiStat
    {
      Peace,
      Upcomming,
      Today,
      Alert,
    }

    // Utilities

    private bool IsDayValid(int day, int month, int year)
    {
      int total_days = DateTime.DaysInMonth(year, month);
      if(day > total_days || day < 1)
        return false;
      else
        return true;
    }

    protected DateTime MapDateToMonth(DateTime payment_date, DateTime ref_date)
    {
      int ref_month = ref_date.Month;
      int ref_year = ref_date.Year;
      int payment_day = payment_date.Day;

      if (IsDayValid(payment_day, ref_month, ref_year))
      {
        return new DateTime(ref_year, ref_month, payment_day);
      }
      else
      {
        return new DateTime(ref_year, ref_month, DateTime.DaysInMonth(ref_year, ref_month));
      }
    }

    // Get infinite intervals

    protected (DateTime?, DateTime?) GetFirstInfinitePaymentInterval(DateTime payment_date)
    {
      return (null, payment_date);
    }

    protected (DateTime?, DateTime?) GetSecondInfinitePaymentInterval(DateTime payment_date)
    {
      return (payment_date, null);
    }

    protected (DateTime?, DateTime?) GetInfinitePaymentInterval()
    {
      return (null, null);
    }

    // Check Payment Interval type

    protected bool IsFirstInfinitePaymentInterval((DateTime?, DateTime?) intv)
    {
      if (intv.Item1 == null && intv.Item2 != null)
        return true;
      return false;
    }

    protected bool IsSecondInfinitePaymentInterval((DateTime?, DateTime?) intv)
    {
      if (intv.Item1 != null && intv.Item2 == null)
        return true;
      return false;
    }

    protected bool IsFinitePaymentInterval((DateTime?, DateTime?) intv)
    {
      if (intv.Item1 != null && intv.Item2 != null)
        return true;
      return false;
    }

    protected bool IsInFinitePaymentInterval((DateTime?, DateTime?) intv)
    {
      if (intv.Item1 == null && intv.Item2 == null)
        return true;
      return false;
    }

    // Check the location of payment in time

    protected bool IsPaymentWithinInterval((DateTime?, DateTime?) intv)
    {
      if (IsInFinitePaymentInterval(intv))
        return true;
      else if (IsFirstInfinitePaymentInterval(intv))
        return DateTime.Compare(DateTime.Today, (DateTime)intv.Item2) < 0;
      else if (IsSecondInfinitePaymentInterval(intv))
        return DateTime.Compare(DateTime.Today, (DateTime)intv.Item1) >= 0;
      else
        return DateTime.Compare(DateTime.Today, (DateTime)intv.Item2) < 0 && DateTime.Compare(DateTime.Today, (DateTime)intv.Item1) >= 0;
    }

    // checks if the payment is in upcomming area of the given interval
    protected bool IsPaymentInUpcommingArea((DateTime?, DateTime?) intv)
    {
      if (IsSecondInfinitePaymentInterval(intv))
        return false;
      else
      {
        int diff = ((DateTime)intv.Item2).Subtract(DateTime.Today).Days;
        return diff <= upcomming_interval && diff > 0;
      }
    }

    // checks if the payment date of the given interval is today
    protected bool IsPaymentDateToday((DateTime?, DateTime?) intv)
    {
      if (IsFirstInfinitePaymentInterval(intv))
        return false;
      else
        return DateTime.Compare(DateTime.Today, (DateTime)intv.Item1) == 0;
    }

    // check Payment State in the given interval

    protected bool IsPaymentPeace(IntvStat intv_state)
    {
      if (intv_state == IntvStat.Peace)
        return true;
      return false;
    }

    protected bool IsPaymentUpcomming((DateTime?, DateTime?) intv, IntvStat next_intv_state)
    {
      if (next_intv_state == IntvStat.Alert && IsPaymentInUpcommingArea(intv))
        return true;
      return false;
    }

    protected bool IsPaymentToday((DateTime?, DateTime?) intv, IntvStat intv_state)
    {
      if (intv_state == IntvStat.Alert && IsPaymentDateToday(intv))
        return true;
      return false;
    }

    protected bool IsPaymentAlert((DateTime?, DateTime?) intv, IntvStat intv_state)
    {
      if (intv_state == IntvStat.Alert && !IsPaymentDateToday(intv))
        return true;
      return false;
    }

    // get payment state list

    public List<NotiStat> GetPaymentNotiStats((DateTime?, DateTime?) intv, IntvStat intv_state)
    {
      var noti_stats = new List<NotiStat>();

      if (IsPaymentPeace(intv_state))
        noti_stats.Add(NotiStat.Peace);

      if (IsPaymentToday(intv, intv_state))
        noti_stats.Add(NotiStat.Today);

      if (IsPaymentAlert(intv, intv_state))
        noti_stats.Add(NotiStat.Alert);

      return noti_stats;
    }

    public List<NotiStat> GetPaymentNotiStats((DateTime?, DateTime?) intv, IntvStat intv_state, IntvStat next_intv_state)
    {
      var noti_stats = new List<NotiStat>();

      foreach (var noti_stat in GetPaymentNotiStats(intv, intv_state))
      {
        noti_stats.Add(noti_stat);
      }

      if (IsPaymentUpcomming(intv, next_intv_state))
        noti_stats.Add(NotiStat.Upcomming);

      return noti_stats;
    }

    public abstract (DateTime?, DateTime?) GetCurrentPaymentInterval(Payment pt);

    public abstract (DateTime?, DateTime?)? GetNextPaymentInterval((DateTime?, DateTime?) intv, Payment pt);

    public abstract IntvStat GetPaymentIntervalState((DateTime?, DateTime?) intv, Payment pt);

    public List<NotiStat> GetPaymentNotiStats(Payment pt)
    {
      (DateTime?, DateTime?) curr_intv = GetCurrentPaymentInterval(pt);
      (DateTime?, DateTime?)? next_intv = GetNextPaymentInterval(curr_intv, pt);

      IntvStat curr_intv_stat = GetPaymentIntervalState(curr_intv, pt);
      IntvStat? next_intv_stat = null;
      if (next_intv != null)
        next_intv_stat = GetPaymentIntervalState(((DateTime?, DateTime?))next_intv, pt);

      if (next_intv_stat != null)
        return GetPaymentNotiStats(curr_intv, curr_intv_stat, (IntvStat)next_intv_stat);
      else
        return GetPaymentNotiStats(curr_intv, curr_intv_stat);
    }
  }

  public class InvalidInterval : Exception {
    public InvalidInterval(string message) : base(message) {
    }
  }

  // FirstInfinitePaymentInterval + SecondInfinitePaymentInterval
  public class NotPaidNotification : Notification
  {
    public override (DateTime?, DateTime?) GetCurrentPaymentInterval(Payment pt)
    {
      if (DateTime.Compare(DateTime.Today, pt.GetPaymentCreationDate()) < 0)
        return GetFirstInfinitePaymentInterval(pt.GetPaymentCreationDate());
      else
        return GetSecondInfinitePaymentInterval(pt.GetPaymentCreationDate());
    }

    public override (DateTime?, DateTime?)? GetNextPaymentInterval((DateTime?, DateTime?) intv, Payment pt)
    {
      if (IsFirstInfinitePaymentInterval(intv))
        return GetSecondInfinitePaymentInterval(pt.GetPaymentCreationDate());
      else if (IsSecondInfinitePaymentInterval(intv))
        return null;
      else
        throw new InvalidInterval("only first infinite and second infinite intervals are allowed for NotPaid payment");
    }

    public override IntvStat GetPaymentIntervalState((DateTime?, DateTime?) intv, Payment pt)
    {
      if (IsFirstInfinitePaymentInterval(intv))
        return IntvStat.Peace;
      else if (IsSecondInfinitePaymentInterval(intv))
        return IntvStat.Alert;
      else
        throw new InvalidInterval("only first infinite and second infinite intervals are allowed for NotPaid payment");
    }
  }

  // FirstInfinitePaymentInterval + ( FinitePaymentInterval + FinitePaymentInterval + ... )
  public class LentNotification : Notification
  {
    private int finite_interval_months = 1;

    private (DateTime, DateTime) GetCurrentFinitePaymentInterval(DateTime payment_date)
    {
      DateTime this_month_first_date = (new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1));
      DateTime this_month_payment_date = MapDateToMonth(payment_date, this_month_first_date);
      DateTime next_month_payment_date = MapDateToMonth(payment_date, this_month_first_date.AddMonths(finite_interval_months));
      DateTime prev_month_payment_date = MapDateToMonth(payment_date, this_month_first_date.AddMonths(-finite_interval_months));

      if (DateTime.Compare(DateTime.Today, this_month_payment_date) < 0)
      {
        return (prev_month_payment_date, this_month_payment_date);
      }
      else
      {
        return (this_month_payment_date, next_month_payment_date);
      }
    }

    private (DateTime, DateTime) GetFirstFinitePaymentInterval(DateTime payment_date)
    {
      DateTime first_payment_month_first_date = (new DateTime(payment_date.Year, payment_date.Month, 1));
      DateTime next_month_payment_date = MapDateToMonth(payment_date, first_payment_month_first_date.AddMonths(finite_interval_months));
      return (payment_date, next_month_payment_date);
    }

    private (DateTime, DateTime) GetNextFinitePaymentInterval((DateTime, DateTime) intv)
    {
      DateTime next_month_first_date = (new DateTime(intv.Item2.Year, intv.Item2.Month, 1));
      DateTime next_payment_intv_end_date = MapDateToMonth(intv.Item2, next_month_first_date.AddMonths(finite_interval_months));
      return (intv.Item2, next_payment_intv_end_date);
    }

    public override (DateTime?, DateTime?) GetCurrentPaymentInterval(Payment pt)
    {
      if (DateTime.Compare(DateTime.Today, pt.GetPaymentCreationDate()) < 0)
        return GetFirstInfinitePaymentInterval(pt.GetPaymentCreationDate());
      else
        return GetCurrentFinitePaymentInterval(pt.GetPaymentCreationDate());
    }

    public override (DateTime?, DateTime?)? GetNextPaymentInterval((DateTime?, DateTime?) intv, Payment pt)
    {
      if (IsFirstInfinitePaymentInterval(intv))
        return GetFirstFinitePaymentInterval(pt.GetPaymentCreationDate());
      else if (IsFinitePaymentInterval(intv))
        return GetNextFinitePaymentInterval(((DateTime, DateTime))intv);
      else
        throw new InvalidInterval("only first infinite and finite intervals are allowed for Lent payment");
    }

    public override IntvStat GetPaymentIntervalState((DateTime?, DateTime?) intv, Payment pt)
    {
      if (IsFirstInfinitePaymentInterval(intv))
        return IntvStat.Peace;
      else if (IsFinitePaymentInterval(intv))
      {
        //WasTheLastLentReceivedOnOrBeyondTheDate
        if (DateTime.Compare((DateTime)intv.Item1, pt.GetLastPaymentCreationDate()) <= 0)
          return IntvStat.Peace;
        else
          return IntvStat.Alert;
      }
      else
        throw new InvalidInterval("only first infinite and finite intervals are allowed for Lent payment");
    }
  }
}
