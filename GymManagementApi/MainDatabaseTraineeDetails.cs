using System;
using System.Data.SQLite;
using System.Collections.Generic;
using GymManagementSystem;
using GymManagementDataModel;

namespace GymManagementApi
{
  public class MainDatabaseTraineeDetails
  {
    MainDatabase main_db = new MainDatabase();

    // Valid data is assumed
    public void InsertTraineeDetails(TraineeDetails d)
    {
      main_db.Connection.Open();
      SQLiteTransaction transaction = null;
      transaction = main_db.Connection.BeginTransaction();

      InsertTraineeProfileDetails(d.profile_details);
      long trainee_id = main_db.Connection.LastInsertRowId;
      d._id = trainee_id;
      string photo_path = (new FileStorage()).StoreProfilePhotoToMainStorage(trainee_id.ToString(), d.profile_details.photo_path);
      UpdateTraineeProfilePhoto(photo_path, trainee_id);
      InsertTraineeContactDetails(d.contact_details, trainee_id);

      transaction.Commit();
      main_db.Connection.Close();
    }

    private void UpdateTraineeProfilePhoto(string photo_path, long trainee_id)
    {
      var cmd = main_db.Connection.CreateCommand();
      cmd.CommandText = @"
        UPDATE TraineeProfileDetails
        SET photo_path = $photo_path
        WHERE trainee_id = $trainee_id;
      ";

      cmd.Parameters.AddWithValue("$photo_path", photo_path);
      cmd.Parameters.AddWithValue("$trainee_id", trainee_id);
      cmd.ExecuteNonQuery();
    }

    private void InsertTraineeProfileDetails(ProfileDetails d)
    {
      var cmd = main_db.Connection.CreateCommand();
      cmd.CommandText = @"
        INSERT INTO
        TraineeProfileDetails
        (
          photo_path,
          first_name,
          last_name,
          age,
          sex,
          comments,
          joining_date
        )
        VALUES
        (
          $photo_path,
          $first_name,
          $last_name,
          $age,
          $sex,
          $comments,
          $joining_date
        );
      ";

      cmd.Parameters.AddWithValue("$photo_path", "");
      cmd.Parameters.AddWithValue("$first_name", d.first_name);
      cmd.Parameters.AddWithValue("$joining_date", ((DateTimeOffset)(d.joining_date)).ToUnixTimeSeconds());

      if (d.last_name != null)
        cmd.Parameters.AddWithValue("$last_name", d.last_name);
      else
        cmd.Parameters.AddWithValue("$last_name", DBNull.Value);

      if (d.age != null)
        cmd.Parameters.AddWithValue("$age", d.age);
      else
        cmd.Parameters.AddWithValue("$age", DBNull.Value);

      if (d.sex != null)
        cmd.Parameters.AddWithValue("$sex", ((int)d.sex));
      else
        cmd.Parameters.AddWithValue("$sex", DBNull.Value);

      if (d.comments != null)
        cmd.Parameters.AddWithValue("$comments", d.comments);
      else
        cmd.Parameters.AddWithValue("$comments", DBNull.Value);

      cmd.ExecuteNonQuery();
    }

    private void InsertTraineeContactDetails(ContactDetails d, long trainee_id)
    {
      var cmd = main_db.Connection.CreateCommand();
      cmd.CommandText = @"
        INSERT INTO
        TraineeContactDetails
        (
          trainee_id,
          phone_no,
          email,
          address,
          city
        )
        VALUES
        (
          $trainee_id,
          $phone_no,
          $email,
          $address,
          $city
        );
      ";

      cmd.Parameters.AddWithValue("$trainee_id", trainee_id);

      if (d.phone_no != null)
        cmd.Parameters.AddWithValue("$phone_no", d.phone_no);
      else
        cmd.Parameters.AddWithValue("$phone_no", DBNull.Value);

      if (d.email != null)
        cmd.Parameters.AddWithValue("$email", d.email);
      else
        cmd.Parameters.AddWithValue("$email", DBNull.Value);

      if (d.address != null)
        cmd.Parameters.AddWithValue("$address", d.address);
      else
        cmd.Parameters.AddWithValue("$address", DBNull.Value);

      if (d.city != null)
        cmd.Parameters.AddWithValue("$city", d.city);
      else
        cmd.Parameters.AddWithValue("$city", DBNull.Value);

      cmd.ExecuteNonQuery();
    }

    public TraineeDetails? FetchTraineeDetails(long trainee_id)
    {
      main_db.Connection.Open();

      var td = new TraineeDetails();
      var pd = FetchTraineeProfileDetails(trainee_id);
      if (pd != null)
      {
        td.profile_details = pd;
        var cd = FetchTraineeContactDetails(trainee_id);
        if (cd != null)
        {
          td.contact_details = cd;
        }
      }

      main_db.Connection.Close();

      if (td.Validate().Count == 0)
        return td;
      else
        return null;
    }

    private ProfileDetails? FetchTraineeProfileDetails(long trainee_id)
    {
      var cmd = main_db.Connection.CreateCommand();
      cmd.CommandText = @"
        SELECT
          photo_path,
          first_name,
          last_name,
          age,
          sex,
          comments,
          joining_date
        FROM TraineeProfileDetails
        WHERE trainee_id = $trainee_id;
      ";

      cmd.Parameters.AddWithValue("$trainee_id", trainee_id);
      using (var reader = cmd.ExecuteReader())
      {
        if (reader.Read())
        {
          var pd = new ProfileDetails();
          pd.photo_path = (new DataPath()).GetFullPathOfPhotoFromMainProfilePhotoStorage(reader.GetString(0));
          pd.first_name = reader.GetString(1);
          pd.joining_date = (DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64(6))).LocalDateTime;

          pd.last_name = reader.IsDBNull(2)? null : reader.GetString(2);
          pd.age = reader.IsDBNull(3)? null : reader.GetInt32(3);
          pd.sex = reader.IsDBNull(4)? null : (ProfileDetails.Sex) reader.GetInt32(4);
          pd.comments = reader.IsDBNull(5)? null : reader.GetString(5);
          return pd;
        }
      }
      return null;
    }

    private ContactDetails? FetchTraineeContactDetails(long trainee_id)
    {
      var cmd = main_db.Connection.CreateCommand();
      cmd.CommandText = @"
        SELECT
          phone_no,
          email,
          address,
          city
        FROM TraineeContactDetails
        WHERE trainee_id = $trainee_id;
      ";

      cmd.Parameters.AddWithValue("$trainee_id", trainee_id);
      using (var reader = cmd.ExecuteReader())
      {
        if (reader.Read())
        {
          var cd = new ContactDetails();
          cd.phone_no = reader.IsDBNull(0)? null : reader.GetString(0);
          cd.email = reader.IsDBNull(1)? null : reader.GetString(1);
          cd.address = reader.IsDBNull(2)? null : reader.GetString(2);
          cd.city = reader.IsDBNull(3)? null : reader.GetString(3);
          return cd;
        }
      }
      return null;
    }

    public long GetTraineeDetailsSearchFullCount(SearchData sd)
    {
      main_db.Connection.Open();

      var cmd = main_db.Connection.CreateCommand();
      cmd.CommandText = @"
        SELECT
          COUNT(*) AS count
      "
      + @"
        FROM TraineeProfileDetails
        INNER JOIN TraineeContactDetails USING(trainee_id)
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

    public List<TraineeDetails> SearchTraineeDetails(long offset, int count, SearchData sd)
    {
      main_db.Connection.Open();

      var cmd = main_db.Connection.CreateCommand();
      cmd.CommandText = @"
        SELECT
          photo_path,
          first_name,
          last_name,
          age,
          sex,
          comments,
          joining_date,
          city,
          trainee_id
      "
      + @"
        FROM TraineeProfileDetails
        INNER JOIN TraineeContactDetails USING(trainee_id)
      "
      + sd.GetSearchString()
      + @"
        LIMIT $offset, $count;
      ";
      var result = new List<TraineeDetails>();

      cmd.Parameters.AddWithValue("$offset", offset);
      cmd.Parameters.AddWithValue("$count", count);
      sd.BindSearchParams(cmd);
      using (var reader = cmd.ExecuteReader())
      {
        while (reader.Read())
        {
          var td = new TraineeDetails();
          var pd = new ProfileDetails();
          var cd = new ContactDetails();
          td.profile_details = pd;
          td.contact_details = cd;
          result.Add(td);

          pd.photo_path = (new DataPath()).GetFullPathOfPhotoFromMainProfilePhotoStorage(reader.GetString(0));
          pd.first_name = reader.GetString(1);
          pd.joining_date = (DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64(6))).LocalDateTime;
          td._id = reader.GetInt64(8);

          pd.age = reader.IsDBNull(3)? null : reader.GetInt32(3);
          pd.sex = reader.IsDBNull(4)? null : (ProfileDetails.Sex) reader.GetInt32(4);
          pd.comments = reader.IsDBNull(5)? null : reader.GetString(5);
          pd.last_name = reader.IsDBNull(2)? null : reader.GetString(2);
          cd.city = reader.IsDBNull(7)? null : reader.GetString(7);
        }
      }

      main_db.Connection.Close();

      return result;
    }
  }
}
