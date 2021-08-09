using System;
using System.Collections.Generic;

namespace GymManagementDataModel
{
  public class SubscriptionDetails
  {
    public int? _id;

    public SubscriptionPaymentDetails.Product? product;
    public SubscriptionPaymentDetails.Plan? plan;
    public DateTime? subscription_initiation_date;
    public DateTime? last_payment_ensured_date;
    public int? amount;

    public enum Error
    {
      IsRequired,
    }

    public void Copy(SubscriptionDetails d)
    {
      d._id = _id;

      d.product = product;
      d.plan = plan;
      d.subscription_initiation_date = subscription_initiation_date;
      d.last_payment_ensured_date = last_payment_ensured_date;
      d.amount = amount;
    }

    // product, plan, subscription_initiation_date, amount are required
    public Dictionary<String, Error> Validate()
    {
      var errors = new Dictionary<String, Error>();

      if (product == null)
      {
        errors.Add("product", Error.IsRequired);
      }
      if (plan == null)
      {
        errors.Add("plan", Error.IsRequired);
      }
      if (subscription_initiation_date == null)
      {
        errors.Add("subscription_initiation_date", Error.IsRequired);
      }
      if (amount == null)
      {
        errors.Add("amount", Error.IsRequired);
      }

      return errors;
    }
  }
}
