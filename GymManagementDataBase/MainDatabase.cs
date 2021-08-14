using System;
using System.Data.SQLite;
using GymManagementSystem;
using GymManagementDataModel;

namespace GymManagementDataBase
{
  public class MainDatabase
  {
    public SQLiteConnection Connection;

    public MainDatabase()
    {
      Connection = new SQLiteConnection($"DataSource={(new DataPath()).GetMainDatabasePath()};");

      Connection.Open();
      SQLiteTransaction transaction = null;
      transaction = Connection.BeginTransaction();

      CreateTraineeDetailsTables();
      CreatePaymentDetailsTables();

      transaction.Commit();
      Connection.Close();
    }

    private void CreateTraineeDetailsTables()
    {
      var cmd = Connection.CreateCommand();
      cmd.CommandText = @"
        CREATE TABLE IF NOT EXISTS
        TraineeProfileDetails
        (
          trainee_id INTEGER,
          photo_path TEXT NOT NULL,
          first_name TEXT NOT NULL,
          last_name TEXT,
          age INTEGER,
          sex INTEGER,
          comments TEXT,
          joining_date INTEGER NOT NULL,

          CONSTRAINT unique_key PRIMARY KEY (trainee_id)
        );

        CREATE TABLE IF NOT EXISTS
        TraineeContactDetails
        (
          trainee_id INTEGER NOT NULL,
          phone_no TEXT,
          email TEXT,
          address TEXT,
          city TEXT,

          UNIQUE (trainee_id),
          FOREIGN KEY (trainee_id) REFERENCES TraineeProfileDetails (trainee_id)
        );
      ";

      cmd.ExecuteNonQuery();
    }

    private void CreatePaymentDetailsTables()
    {
      var cmd = Connection.CreateCommand();
      cmd.CommandText = @"
        CREATE TABLE IF NOT EXISTS
        PaymentDetails
        (
          payment_id INTEGER,
          trainee_id INTEGER NOT NULL,

          FOREIGN KEY (trainee_id) REFERENCES TraineeProfileDetails (trainee_id),
          CONSTRAINT unique_key PRIMARY KEY (payment_id)
        );

        CREATE TABLE IF NOT EXISTS
        IndividualPaymentDetails
        (
          payment_id INTEGER NOT NULL,
          product INTEGER NOT NULL,

          UNIQUE (payment_id),
          FOREIGN KEY (payment_id) REFERENCES PaymentDetails (payment_id)
        );

        CREATE TABLE IF NOT EXISTS
        SubscriptionPaymentDetails
        (
          payment_id INTEGER NOT NULL,
          product INTEGER NOT NULL,
          plan INTEGER NOT NULL,

          UNIQUE (payment_id),
          FOREIGN KEY (payment_id) REFERENCES PaymentDetails (payment_id)
        );

        CREATE TABLE IF NOT EXISTS
        PaymentData
        (
          payment_id INTEGER NOT NULL,
          payment_status INTEGER NOT NULL,
          date_of_payment INTEGER NOT NULL,
          amount INTEGER NOT NULL,

          UNIQUE (payment_id),
          FOREIGN KEY (payment_id) REFERENCES PaymentDetails (payment_id)
        );

        CREATE TABLE IF NOT EXISTS
        PayData
        (
          payment_id INTEGER NOT NULL,
          penalty INTEGER NOT NULL,
          discount INTEGER NOT NULL,
          pay_date INTEGER NOT NULL,

          UNIQUE (payment_id),
          FOREIGN KEY (payment_id) REFERENCES PaymentDetails (payment_id)
        );

        CREATE TABLE IF NOT EXISTS
        LendPaymentData
        (
          payment_id INTEGER NOT NULL,
          payment_status INTEGER NOT NULL,
          date_of_payment INTEGER NOT NULL,
          amount INTEGER NOT NULL,

          FOREIGN KEY (payment_id) REFERENCES PaymentDetails (payment_id)
        );

        CREATE TABLE IF NOT EXISTS
        LendPayData
        (
          payment_id INTEGER NOT NULL,
          penalty INTEGER NOT NULL,
          discount INTEGER NOT NULL,
          pay_date INTEGER NOT NULL,

          FOREIGN KEY (payment_id) REFERENCES PaymentDetails (payment_id)
        );
      ";

      cmd.ExecuteNonQuery();
    }
  }
}
