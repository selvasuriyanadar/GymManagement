using System;
using System.Data.SQLite;
using System.Collections.Generic;
using GymManagementSystem;
using GymManagementDataModel;

namespace GymManagementApi
{
  public class MainDatabasePaymentDetails
  {
    MainDatabase main_db = new MainDatabase();

    // Expects valid data with trainee_id and payment_data
    public void InsertPaymentDetails(PaymentDetails d)
    {
      main_db.Connection.Open();
      SQLiteTransaction transaction = null;
      transaction = main_db.Connection.BeginTransaction();

      InsertPaymentDetails((long) d._trainee_id);
      long payment_id = main_db.Connection.LastInsertRowId;
      d._id = payment_id;
      if (d.payment_type == PaymentDetails.PaymentType.Individual)
        InsertPaymentDetails(payment_id, (IndividualPaymentDetails)d);
      else if (d.payment_type == PaymentDetails.PaymentType.Subscription)
        InsertPaymentDetails(payment_id, (SubscriptionPaymentDetails)d);

      InsertPaymentData(payment_id, d.payment_data);
      if (d.payment_data.pay_data != null)
        InsertPayData(payment_id, d.payment_data.pay_data);

      if (d.payment_data.lent_payment_list != null)
      {
        foreach (var lpd in d.payment_data.lent_payment_list)
        {
          InsertLendPaymentData(payment_id, lpd);
          if (lpd.pay_data != null)
            InsertLendPayData(payment_id, lpd.pay_data);
        }
      }

      transaction.Commit();
      main_db.Connection.Close();
    }

    // Expects valid data with trainee_id 
    public void InsertFullLendPaymentData(long payment_id, PaymentData d)
    {
      main_db.Connection.Open();
      SQLiteTransaction transaction = null;
      transaction = main_db.Connection.BeginTransaction();

      InsertLendPaymentData(payment_id, d);
      if (d.pay_data != null)
        InsertLendPayData(payment_id, d.pay_data);

      transaction.Commit();
      main_db.Connection.Close();
    }

    private void InsertPaymentDetails(long trainee_id)
    {
      var cmd = main_db.Connection.CreateCommand();
      cmd.CommandText = @"
        INSERT INTO
        PaymentDetails
        (
          trainee_id
        )
        VALUES
        (
          $trainee_id
        );
      ";

      cmd.Parameters.AddWithValue("$trainee_id", trainee_id);

      cmd.ExecuteNonQuery();
    }

    private void InsertPaymentDetails(long payment_id, IndividualPaymentDetails d)
    {
      var cmd = main_db.Connection.CreateCommand();
      cmd.CommandText = @"
        INSERT INTO
        IndividualPaymentDetails
        (
          payment_id,
          product
        )
        VALUES
        (
          $payment_id,
          $product
        );
      ";

      cmd.Parameters.AddWithValue("$payment_id", payment_id);
      cmd.Parameters.AddWithValue("$product", (int) d.product);

      cmd.ExecuteNonQuery();
    }

    private void InsertPaymentDetails(long payment_id, SubscriptionPaymentDetails d)
    {
      var cmd = main_db.Connection.CreateCommand();
      cmd.CommandText = @"
        INSERT INTO
        SubscriptionPaymentDetails
        (
          payment_id,
          product,
          plan
        )
        VALUES
        (
          $payment_id,
          $product,
          $plan
        );
      ";

      cmd.Parameters.AddWithValue("$payment_id", payment_id);
      cmd.Parameters.AddWithValue("$product", (int) d.product);
      cmd.Parameters.AddWithValue("$plan", (int) d.plan);

      cmd.ExecuteNonQuery();
    }

    private void InsertPaymentData(long payment_id, PaymentData d)
    {
      var cmd = main_db.Connection.CreateCommand();
      cmd.CommandText = @"
        INSERT INTO
        PaymentData
        (
          payment_id,
          payment_status,
          date_of_payment,
          amount
        )
        VALUES
        (
          $payment_id,
          $payment_status,
          $date_of_payment,
          $amount
        );
      ";

      cmd.Parameters.AddWithValue("$payment_id", payment_id);
      cmd.Parameters.AddWithValue("$payment_status", (int) d.payment_status);
      cmd.Parameters.AddWithValue("$date_of_payment", ((DateTimeOffset)(d.date_of_payment)).ToUnixTimeSeconds());
      cmd.Parameters.AddWithValue("$amount", d.amount);

      cmd.ExecuteNonQuery();
    }

    private void InsertPayData(long payment_id, PayData d)
    {
      var cmd = main_db.Connection.CreateCommand();
      cmd.CommandText = @"
        INSERT INTO
        PayData
        (
          payment_id,
          penalty,
          discount,
          pay_date
        )
        VALUES
        (
          $payment_id,
          $penalty,
          $discount,
          $pay_date
        );
      ";

      cmd.Parameters.AddWithValue("$payment_id", payment_id);
      cmd.Parameters.AddWithValue("$penalty", d.penalty);
      cmd.Parameters.AddWithValue("$discount", d.discount);
      cmd.Parameters.AddWithValue("$pay_date", ((DateTimeOffset)(d.pay_date)).ToUnixTimeSeconds());

      cmd.ExecuteNonQuery();
    }

    private void InsertLendPaymentData(long payment_id, PaymentData d)
    {
      var cmd = main_db.Connection.CreateCommand();
      cmd.CommandText = @"
        INSERT INTO
        LendPaymentData
        (
          payment_id,
          payment_status,
          date_of_payment,
          amount
        )
        VALUES
        (
          $payment_id,
          $payment_status,
          $date_of_payment,
          $amount
        );
      ";

      cmd.Parameters.AddWithValue("$payment_id", payment_id);
      cmd.Parameters.AddWithValue("$payment_status", (int) d.payment_status);
      cmd.Parameters.AddWithValue("$date_of_payment", ((DateTimeOffset)(d.date_of_payment)).ToUnixTimeSeconds());
      cmd.Parameters.AddWithValue("$amount", d.amount);

      cmd.ExecuteNonQuery();
    }

    private void InsertLendPayData(long payment_id, PayData d)
    {
      var cmd = main_db.Connection.CreateCommand();
      cmd.CommandText = @"
        INSERT INTO
        LendPayData
        (
          payment_id,
          penalty,
          discount,
          pay_date
        )
        VALUES
        (
          $payment_id,
          $penalty,
          $discount,
          $pay_date
        );
      ";

      cmd.Parameters.AddWithValue("$payment_id", payment_id);
      cmd.Parameters.AddWithValue("$penalty", d.penalty);
      cmd.Parameters.AddWithValue("$discount", d.discount);
      cmd.Parameters.AddWithValue("$pay_date", ((DateTimeOffset)(d.pay_date)).ToUnixTimeSeconds());

      cmd.ExecuteNonQuery();
    }

    public long GetPaymentDetailsSearchFullCount(SearchData sd)
    {
      main_db.Connection.Open();

      var cmd = main_db.Connection.CreateCommand();
      cmd.CommandText = @"
        SELECT
          COUNT(*) AS count
      "
      + @"
        FROM PaymentDetails o
        INNER JOIN TraineeProfileDetails USING(trainee_id)
        INNER JOIN PaymentData USING(payment_id)
        LEFT JOIN IndividualPaymentDetails i USING(payment_id)
        LEFT JOIN SubscriptionPaymentDetails s USING(payment_id)
      "
      + sd.GetSearchString();
      long result = 0;

      sd.BindSearchParams(cmd);
      using (var reader = cmd.ExecuteReader())
      {
        if (reader.Read())
        {
          result = reader.GetInt64(0);
        }
      }

      main_db.Connection.Close();

      return result;
    }

    public List<PaymentDetails> SearchPaymentDetails(long offset, int count, SearchData sd)
    {
      main_db.Connection.Open();

      var cmd = main_db.Connection.CreateCommand();
      cmd.CommandText = @"
        SELECT
          o.trainee_id,
          o.payment_id,
          photo_path,
          first_name,
          last_name,
          payment_status,
          date_of_payment,
          amount,
          i.product,
          s.product,
          s.plan
      "
      + @"
        FROM PaymentDetails o
        INNER JOIN TraineeProfileDetails USING(trainee_id)
        INNER JOIN PaymentData USING(payment_id)
        LEFT JOIN IndividualPaymentDetails i USING(payment_id)
        LEFT JOIN SubscriptionPaymentDetails s USING(payment_id)
      "
      + sd.GetSearchString()
      + @"
        LIMIT $offset, $count;
      ";
      var result = new List<PaymentDetails>();

      cmd.Parameters.AddWithValue("$offset", offset);
      cmd.Parameters.AddWithValue("$count", count);
      sd.BindSearchParams(cmd);
      using (var reader = cmd.ExecuteReader())
      {
        while (reader.Read())
        {
          PaymentDetails td;
          if (!reader.IsDBNull(8))
          {
            td = new IndividualPaymentDetails();
            ((IndividualPaymentDetails)td).product = (IndividualPaymentDetails.Product) reader.GetInt32(8);
          }
          else if (!reader.IsDBNull(9))
          {
            td = new SubscriptionPaymentDetails();
            ((SubscriptionPaymentDetails)td).product = (SubscriptionPaymentDetails.Product) reader.GetInt32(9);
            ((SubscriptionPaymentDetails)td).plan = (SubscriptionPaymentDetails.Plan) reader.GetInt32(10);
          }
          else
          {
            continue;
          }
          var pd = new ProfileDetails();
          var pt = new PaymentData();
          td.profile_details = pd;
          td.payment_data = pt;
          result.Add(td);

          td._trainee_id = reader.GetInt64(0);
          td._id = reader.GetInt64(1);
          pd.photo_path = (new DataPath()).GetFullPathOfPhotoFromMainProfilePhotoStorage(reader.GetString(2));
          pd.first_name = reader.GetString(3);
          pt.payment_status = (PaymentData.PaymentStatus) reader.GetInt32(5);
          pt.date_of_payment = (DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64(6))).LocalDateTime;
          pt.amount = reader.GetInt32(7);

          pd.last_name = reader.IsDBNull(4)? null : reader.GetString(4);
        }
      }

      main_db.Connection.Close();

      return result;
    }
  }
}
