using System;
using System.Collections.Generic;
using GymManagementDataModel;

namespace GymManagementLogic
{
  public class ValidationFailed: Exception {
    public ValidationFailed(string message): base(message) {
    }
  }

  public class PaymentStatusDoesNotAllowTheOperation: Exception {
    public PaymentStatusDoesNotAllowTheOperation(string message): base(message) {
    }
  }

  public class Payment
  {
    public enum Error
    {
      IsRequired,
      AmountTooSmall,
      AmountTooBig,
      DateTooBig,
      DateTooEarly,
      InvalidData,
      PaymentStatusMustBePaid,
    }

    private PaymentData.PaymentStatus payment_status = PaymentData.PaymentStatus.NotPaid;
    private DateTime date_of_payment;
    private int amount;

    private PayBag? pay_bag;

    private LentPaymentList? lent_payment_list;

    public Payment(PaymentData d)
    {
      if (d.Validate().Count == 0)
      {
        date_of_payment = (DateTime)d.date_of_payment;
        amount = (int)d.amount;
      }
      else
        throw new ValidationFailed("payment data is invalid.");
    }

    /// <summary>
    /// payment status check
    /// </summary>

    // checks if the payment was lent
    public bool WasPaymentLent()
    {
      return payment_status == PaymentData.PaymentStatus.Lent
          || payment_status == PaymentData.PaymentStatus.LentReturned;
    }

    // checks the completion of payment process for lent and normal payments
    public bool IsPaymentComplete()
    {
      return payment_status == PaymentData.PaymentStatus.Paid
          || payment_status == PaymentData.PaymentStatus.LentReturned;
    }

    public bool IsPaymentLent()
    {
      return payment_status == PaymentData.PaymentStatus.Lent;
    }

    public bool IsPaymentLentReturned()
    {
      return payment_status == PaymentData.PaymentStatus.LentReturned;
    }

    public bool IsPaymentPaid()
    {
      return payment_status == PaymentData.PaymentStatus.Paid;
    }

    public bool IsPaymentNotPaid()
    {
      return payment_status == PaymentData.PaymentStatus.NotPaid;
    }

    /// <summary>
    /// privates
    /// </summary>

    private bool CheckIfLentReturned()
    {
      return amount == lent_payment_list.GetTotalLentPaymentAmount();
    }

    /// <summary>
    /// publics
    /// </summary>

    public int GetAmount()
    {
      return amount;
    }

    // if lent returned payment returns the pay_date of the last lent payment
    // otherwise returns pay_date of paid payment
    public DateTime GetPaymentPaidDate()
    {
      if (IsPaymentComplete())
      {
        if (IsPaymentLentReturned())
          return (DateTime)(lent_payment_list.GetLastPaymentPaidDate());
        else
          return pay_bag.GetPayDate();
      }
      else
        throw new PaymentStatusDoesNotAllowTheOperation("paid date does not exist for payment in progress.");
    }

    public DateTime GetPaymentCreationDate()
    {
      return date_of_payment;
    }

    // if lent payment or lentreturned payment then gets the last lent 
    //   payment date if any lent payment available.
    //   otherwise returns the payment creation date
    public DateTime GetLastPaymentCreationDate()
    {
      if (WasPaymentLent())
      {
        DateTime? ld = lent_payment_list.GetLastPaymentCreationDate();
        if (ld != null)
          return (DateTime)ld;
      }

      return date_of_payment;
    }

    public int GetPaidAmount()
    {
      if (WasPaymentLent())
      {
        return lent_payment_list.GetPaidAmount();
      }
      else if (IsPaymentPaid())
      {
        return amount + pay_bag.GetPenaltyDiscountSum();
      }
      else
      {
        return 0;
      }
    }

    public int GetLastPaidAmount()
    {
      if (WasPaymentLent())
      {
        return lent_payment_list.GetLastPaidAmount();
      }

      return GetPaidAmount();
    }

    /// <summary>
    /// paying logic
    /// </summary>

    // creates the pay_bag
    // marks payment_status as Paid
    public void MakePayment(PayData pd)
    {
      if (ValidatePayData(pd).Count == 0)
      {
        pay_bag = new PayBag(pd);
        payment_status = PaymentData.PaymentStatus.Paid;
      }
    }

    // adds the payment to the lent_payment_list
    // if lent has returned then marks the payment_status as LentReturned
    public void MakePayment(Payment pt)
    {
      if (ValidateLentPayment(pt).Count == 0)
      {
        lent_payment_list.AddPayment(pt);
        if (CheckIfLentReturned())
        {
          payment_status = PaymentData.PaymentStatus.LentReturned;
        }
      }
    }

    // marks payment_status as Lent
    public void LentPayment()
    {
      if (IsPaymentNotPaid())
      {
        lent_payment_list = new LentPaymentList();
        payment_status = PaymentData.PaymentStatus.Lent;
      }
      else
      {
        throw new PaymentStatusDoesNotAllowTheOperation("not paid payment can only be lent");
      }
    }

    /// <summary>
    /// compose allowing validations
    /// </summary>

    // PayData shall be valid
    // discount shall not exceed amount
    // pay_date shall be the same or a later date to the date_of_payment
    //
    public Dictionary<String, Error> ValidatePayData(PayData pd)
    {
      if (IsPaymentNotPaid())
      {
        var errors = new Dictionary<String, Error>();

        if (pd.Validate().Count != 0)
        {
          errors.Add("PayData", Error.InvalidData);
          return errors;
        }

        if (!(amount >= pd.discount))
        {
          errors.Add("discount", Error.AmountTooBig);
        }

        if (!(DateTime.Compare(date_of_payment, (DateTime)pd.pay_date) <= 0))
        {
          errors.Add("pay_date", Error.DateTooEarly);
        }

        return errors;
      }
      else
      {
        throw new PaymentStatusDoesNotAllowTheOperation("not paid payment can only be paid");
      }
    }

    // lent payment must be paid
    // total lent paid amount shall not exceed the original lented payment amount
    // date_of_payment shall be later to the last payment date
    // pay_date of lent payment shall be the same as the date_of_payment
    //
    public Dictionary<String, Error> ValidateLentPayment(Payment pt)
    {
      if (IsPaymentLent())
      {
        var errors = new Dictionary<String, Error>();

        if (!(pt.IsPaymentPaid()))
        {
          errors.Add("payment_status", Error.PaymentStatusMustBePaid);
          return errors;
        }

        if (!(amount >= (lent_payment_list.GetTotalLentPaymentAmount() + pt.GetAmount())))
        {
          errors.Add("amount", Error.AmountTooBig);
        }

        if (!(DateTime.Compare(GetLastPaymentCreationDate(), pt.GetPaymentCreationDate()) <= 0))
        {
          errors.Add("date_of_payment", Error.DateTooEarly);
        }

        if (!(DateTime.Compare(pt.GetPaymentCreationDate(), pt.GetPaymentPaidDate()) == 0))
        {
          errors.Add("pay_bag.pay_date", Error.DateTooBig);
        }

        return errors;
      }
      else
      {
        throw new PaymentStatusDoesNotAllowTheOperation("lent payment can only be paid by lent");
      }
    }

    public PaymentData Copy()
    {
      var d = new PaymentData();

      d.payment_status = payment_status;
      d.date_of_payment = date_of_payment;
      d.amount = amount;

      if (WasPaymentLent())
      {
        lent_payment_list.Copy(d);
      }
      else if (IsPaymentPaid())
      {
        pay_bag.Copy(d);
      }
      
      return d;
    }
  }

  public class LentPaymentList
  {
    private List<Payment> lent_payment_list = new List<Payment>();

    public int GetTotalLentPaymentAmount()
    {
      int sum = 0;
      foreach (var lp in lent_payment_list)
      {
        sum += lp.GetAmount();
      }
      return sum;
    }

    public void AddPayment(Payment pt)
    {
      lent_payment_list.Add(pt);
    }

    public DateTime? GetLastPaymentCreationDate()
    {
      if (lent_payment_list.Count == 0)
        return null;
      else
        return lent_payment_list[lent_payment_list.Count-1].GetPaymentCreationDate();
    }

    public DateTime? GetLastPaymentPaidDate()
    {
      if (lent_payment_list.Count == 0)
        return null;
      else
        return lent_payment_list[lent_payment_list.Count-1].GetPaymentPaidDate();
    }

    public int GetPaidAmount()
    {
      int sum = 0;
      foreach (var lp in lent_payment_list)
      {
        sum += lp.GetPaidAmount();
      }
      return sum;
    }

    public int GetLastPaidAmount()
    {
      if (lent_payment_list.Count == 0)
        return 0;
      else
        return lent_payment_list[lent_payment_list.Count-1].GetPaidAmount();
    }

    public void Copy(PaymentData d)
    {
      var lent_list = new List<PaymentData>();

      foreach (var lp in lent_payment_list)
      {
        lent_list.Add(lp.Copy());
      }

      d.lent_payment_list = lent_list;
    }
  }

  public class PayBag
  {
    private int penalty;
    private int discount;
    private DateTime pay_date;

    public PayBag(PayData d)
    {
      if (d.Validate().Count == 0)
      {
        penalty = d.penalty;
        discount = d.discount;
        pay_date = (DateTime)d.pay_date;
      }
      else
        throw new ValidationFailed("pay data is invalid.");
    }

    public DateTime GetPayDate()
    {
      return pay_date;
    }

    public int GetPenalty()
    {
      return penalty;
    }

    public int GetDiscount()
    {
      return discount;
    }

    public int GetPenaltyDiscountSum()
    {
      return penalty - discount;
    }

    public void Copy(PaymentData d)
    {
      var pay_data = new PayData();

      pay_data.penalty = penalty;
      pay_data.discount = discount;
      pay_data.pay_date = pay_date;

      d.pay_data = pay_data;
    }
  }
}
