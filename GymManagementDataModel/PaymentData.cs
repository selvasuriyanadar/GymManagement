using System;
using System.Collections.Generic;

namespace GymManagementDataModel
{
  public class PayData
  {
    public int penalty = 0;
    public int discount = 0;
    public DateTime? pay_date;

    public void SetCurrentDateAsPayDate()
    {
      pay_date = DateTime.Today;
    }

    // penalty and discount cannot be negative
    // pay_date is required
    //
    public Dictionary<String, PaymentData.Error> Validate()
    {
      var errors = new Dictionary<String, PaymentData.Error>();

      if (!(penalty >= 0))
      {
        errors.Add("penalty", PaymentData.Error.AmountTooSmall);
      }

      if (!(discount >= 0))
      {
        errors.Add("discount", PaymentData.Error.AmountTooSmall);
      }

      if (pay_date == null)
      {
        errors.Add("pay_date", PaymentData.Error.IsRequired);
      }

      return errors;
    }
  }

  public class PaymentData
  {
    public static Dictionary<PaymentStatus, String> PaymentStatusDict = new Dictionary<PaymentStatus, String>() {
      { PaymentStatus.Paid, "Paid" },
      { PaymentStatus.NotPaid, "Not Paid" },
      { PaymentStatus.Lent, "Lent" },
      { PaymentStatus.LentReturned, "Lent Returned" }
    };
    public enum PaymentStatus
    {
      Paid,
      NotPaid,
      Lent,
      LentReturned,
    }

    public PaymentStatus? payment_status;
    public DateTime? date_of_payment;
    public int? amount;

    public PayData? pay_data;
    public List<PaymentData>? lent_payment_list;

    public enum Error
    {
      IsRequired,
      AmountTooSmall,
    }

    // date_of_payment and amount are required
    // amount can not be negative
    //
    public Dictionary<String, Error> Validate()
    {
      var errors = new Dictionary<String, Error>();

      if (date_of_payment == null)
      {
        errors.Add("date_of_payment", Error.IsRequired);
      }

      if (amount == null)
      {
        errors.Add("amount", Error.IsRequired);
      }
      else if (!(amount >= 0))
      {
        errors.Add("amount", Error.AmountTooSmall);
      }

      return errors;
    }
  }
}
