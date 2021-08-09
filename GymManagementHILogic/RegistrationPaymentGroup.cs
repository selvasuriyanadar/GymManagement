using System;
using GymManagementDataModel;
using GymManagementLogic;
using System.Collections.Generic;

namespace GymManagementHILogic
{
  public class RegistrationPaymentGroup
  {
    public SubscriptionPaymentDetails gym_subscription_details = new SubscriptionPaymentDetails();
    public IndividualPaymentDetails advance_details = new IndividualPaymentDetails();
    public IndividualPaymentDetails protein_supplement_details = new IndividualPaymentDetails();

    public PaymentData gym_subscription_details_payment = new PaymentData();
    public PaymentData advance_details_payment = new PaymentData();
    public PaymentData protein_supplement_details_payment = new PaymentData();
    public PaymentData protein_supplement_details_lent_payment = new PaymentData();

    public PayData gym_subscription_details_paydata = new PayData();
    public PayData advance_details_paydata = new PayData();
    public PayData protein_supplement_details_paydata = new PayData();
    public PayData protein_supplement_details_lent_paydata = new PayData();

    private bool lent_protein_supplement_details_payment = false;

    public enum Error
    {
      InvalidData,
      InvalidPayData,
    }

    private Dictionary<String, Error> ValidateForPayLater()
    {
      var errors = new Dictionary<String, Error>();

      if (gym_subscription_details_payment.Validate().Count != 0)
      {
        errors.Add("gym_subscription_details_payment", Error.InvalidData);
      }

      if (advance_details_payment.Validate().Count != 0)
      {
        errors.Add("advance_details_payment", Error.InvalidData);
      }

      return errors;
    }

    private Dictionary<String, Error> ValidateForPayNow()
    {
      var errors = new Dictionary<String, Error>();

      foreach(var error in ValidateForPayLater())
      {
        errors.Add(error.Key, error.Value);
      }

      if (errors.Count == 0)
      {
        var pt = new Payment(gym_subscription_details_payment);
        if (gym_subscription_details_paydata.Validate().Count != 0)
        {
          errors.Add("gym_subscription_details_paydata", Error.InvalidData);
        }
        else if (pt.ValidatePayData(gym_subscription_details_paydata).Count != 0)
        {
          errors.Add("gym_subscription_details_paydata", Error.InvalidPayData);
        }

        pt = new Payment(advance_details_payment);
        if (advance_details_paydata.Validate().Count != 0)
        {
          errors.Add("advance_details_paydata", Error.InvalidData);
        }
        else if (pt.ValidatePayData(advance_details_paydata).Count != 0)
        {
          errors.Add("advance_details_paydata", Error.InvalidPayData);
        }
      }

      return errors;
    }

    private Payment? PayUnPaidPayment(Payment pt, PayData py)
    {
      if (py.Validate().Count == 0)
      {
        if (pt.ValidatePayData(py).Count == 0)
        {
          pt.MakePayment(py);
        }
        else
        {
          return null;
        }
      }
      else
      {
        PayData tpd = new PayData();
        tpd.SetCurrentDateAsPayDate();
        pt.MakePayment(tpd);
      }
      return pt;
    }

    private Payment? PayLentPayment(Payment pt, PaymentData lpd, PayData py)
    {
      Payment lpt = ConstructPayment(true, lpd, py);

      if ((lpt != null) && pt.ValidateLentPayment(lpt).Count == 0)
      {
        pt.MakePayment(lpt);
      }
      return pt;
    }

    private Payment? ConstructPayment(bool pay, PaymentData pd, PayData py)
    {
      if (pd.Validate().Count == 0)
      {
        Payment pt = new Payment(pd);

        if (pay)
        {
          return PayUnPaidPayment(pt, py);
        }
        else
        {
          return pt;
        }
      }
      return null;
    }

    private Payment? ConstructLentPayment(bool pay, PaymentData pd, PaymentData lpd, PayData py)
    {
      if (pd.Validate().Count == 0)
      {
        Payment pt = new Payment(pd);
        pt.LentPayment();

        if (pay)
        {
          return PayLentPayment(pt, lpd, py);
        }
        else
        {
          return pt;
        }
      }
      return null;
    }

    public void InitiateWithDefaultSelections()
    {
      gym_subscription_details.product = SubscriptionPaymentDetails.Product.GymSubscription;
      gym_subscription_details.plan = SubscriptionPaymentDetails.Plan.Monthly;
      advance_details.product = IndividualPaymentDetails.Product.Advance;
      protein_supplement_details.product = IndividualPaymentDetails.Product.GymProteinBodyPowder;
    }

    public void UpdateAllPaymentDates(DateTime? date)
    {
      gym_subscription_details_payment.date_of_payment = date;
      advance_details_payment.date_of_payment = date;
      protein_supplement_details_payment.date_of_payment = date;
      protein_supplement_details_lent_payment.date_of_payment = date;

      gym_subscription_details_paydata.pay_date = date;
      advance_details_paydata.pay_date = date;
      protein_supplement_details_paydata.pay_date = date;
      protein_supplement_details_lent_paydata.pay_date = date;
    }

    public void RevokeLentOnProteinSupplementPayment()
    {
      lent_protein_supplement_details_payment = false;
    }

    public void LentProteinSupplementPayment()
    {
      lent_protein_supplement_details_payment = true;
    }

    public bool IsProteinSupplementPaymentLent()
    {
      return lent_protein_supplement_details_payment;
    }

    public List<Payment> ExtractAllPayments(bool pay)
    {
      var lsp = new List<Payment>();

      Payment? p1 = ConstructPayment(pay, gym_subscription_details_payment, gym_subscription_details_paydata);
      if (p1 != null)
        lsp.Add(p1);
      Payment? p2 = ConstructPayment(pay, advance_details_payment, advance_details_paydata);
      if (p2 != null)
        lsp.Add(p2);
      Payment? p3 = !IsProteinSupplementPaymentLent()?
          ConstructPayment(pay, protein_supplement_details_payment, protein_supplement_details_paydata)
          : ConstructLentPayment(pay, protein_supplement_details_payment, protein_supplement_details_lent_payment, protein_supplement_details_lent_paydata);
      if (p3 != null)
        lsp.Add(p3);

      return lsp;
    }

    public List<PaymentDetails> ExtractAllPaymentDetails(bool pay)
    {
      var lsp = new List<PaymentDetails>();

      Payment? p1 = ConstructPayment(pay, gym_subscription_details_payment, gym_subscription_details_paydata);
      if (p1 != null)
      {
        gym_subscription_details.payment_data = p1.Copy();
        lsp.Add(gym_subscription_details);
      }
      Payment? p2 = ConstructPayment(pay, advance_details_payment, advance_details_paydata);
      if (p2 != null)
      {
        advance_details.payment_data = p2.Copy();
        lsp.Add(advance_details);
      }
      Payment? p3 = !IsProteinSupplementPaymentLent()?
          ConstructPayment(pay, protein_supplement_details_payment, protein_supplement_details_paydata)
          : ConstructLentPayment(pay, protein_supplement_details_payment, protein_supplement_details_lent_payment, protein_supplement_details_lent_paydata);
      if (p3 != null)
      {
        protein_supplement_details.payment_data = p3.Copy();
        lsp.Add(protein_supplement_details);
      }

      return lsp;
    }

    public String CalculateTotalPayment()
    {
      List<Payment> lsp = ExtractAllPayments(true);

      Int64 sum = 0;
      foreach (var pt in lsp)
      {
        sum += pt.GetLastPaidAmount();
      }
      return (sum).ToString();
    }

    public Dictionary<String, Error> Validate(bool pay)
    {
      if (pay)
        return ValidateForPayNow();
      else
        return ValidateForPayLater();
    }
  }
}
