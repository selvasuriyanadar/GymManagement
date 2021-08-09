using System;
using System.Collections.Generic;

namespace GymManagementDataModel
{

  public class PaymentDetails
  {
    public enum PaymentType
    {
      Subscription,
      Individual,
    }

    public long? _id;
    public long? _trainee_id;

    public PaymentType payment_type;
    // reference
    public PaymentData? payment_data;
    // reference
    public ProfileDetails? profile_details;

    public PaymentDetails(PaymentType payment_type)
    {
      this.payment_type = payment_type;
    }

    public virtual String SelectionAsString()
    {
      return "";
    }

    public virtual void Copy(PaymentDetails d)
    {
      d._id = _id;
      d._trainee_id = _trainee_id;
    }

    public enum Error
    {
      IsRequired,
      InvalidData,
    }

    public virtual Dictionary<String, Error> Validate()
    {
      return new Dictionary<String, Error>();
    }
  }

  public class SubscriptionPaymentDetails : PaymentDetails
  {
    public static Dictionary<Product, String> ProductDict = new Dictionary<Product, String>() {
      { Product.GymSubscription, "Gym Subscription" }
    };
    public enum Product
    {
      GymSubscription,
    }

    public static Dictionary<Plan, String> PlanDict = new Dictionary<Plan, String>() {
      { Plan.Monthly, "Monthly" }
    };
    public enum Plan
    {
      Monthly,
    }

    public Product? product;
    public Plan? plan;

    public SubscriptionPaymentDetails() : base(PaymentDetails.PaymentType.Subscription)
    {}

    public override String SelectionAsString()
    {
      if (Validate().Count == 0)
        return ProductDict[(Product)product] + ", " + PlanDict[(Plan)plan];
      else
        return base.SelectionAsString();
    }

    public override void Copy(PaymentDetails d)
    {
      base.Copy(d);
      try
      {
        ((SubscriptionPaymentDetails)d).product = product;
        ((SubscriptionPaymentDetails)d).plan = plan;
      }
      catch {}
    }

    // product and plan are required
    public override Dictionary<String, Error> Validate()
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

      return errors;
    }
  }

  public class IndividualPaymentDetails : PaymentDetails
  {
    public static Dictionary<Product, String> ProductDict = new Dictionary<Product, String>() {
      { Product.GymProteinBodyPowder, "Protein Supplement" },
      { Product.Advance, "Advance" },
    };
    public enum Product
    {
      GymProteinBodyPowder,
      Advance,
    }

    public Product? product;

    public IndividualPaymentDetails() : base(PaymentDetails.PaymentType.Individual)
    {}

    public override String SelectionAsString()
    {
      if (Validate().Count == 0)
        return ProductDict[(Product)product];
      else
        return base.SelectionAsString();
    }

    public override void Copy(PaymentDetails d)
    {
      base.Copy(d);
      try
      {
        ((IndividualPaymentDetails)d).product = product;
      }
      catch {}
    }

    // product is required
    public override Dictionary<String, Error> Validate()
    {
      var errors = new Dictionary<String, Error>();

      if (product == null)
      {
        errors.Add("product", Error.IsRequired);
      }

      return errors;
    }
  }
}
