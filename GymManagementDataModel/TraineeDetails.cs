using System;
using System.Collections.Generic;

namespace GymManagementDataModel
{
  public class ProfileDetails
  {
    public enum Sex
    {
      Male,
      Female
    }

    public string? photo_path { get; set; }
    public string? first_name { get; set; }
    public string? last_name { get; set; }
    public int? age { get; set; }
    public Sex? sex { get; set; }
    public string? comments { get; set; }
    public DateTime? joining_date { get; set; }
    public string full_name { get { return GetFullName(); } }
    public string about { get { return GetAbout(); } }

    public string GetFullName()
    {
      if (last_name != null)
      {
        return first_name + " " + last_name;
      }
      else
      {
        return first_name;
      }
    }

    public string GetAbout()
    {
      string about = "";
      if (age != null)
        about += " . Age " + age.ToString();
      if (sex != null)
        about += " . " + ((sex == Sex.Male)? "Male" : "Female");
      about += " . Joined on " + ((DateTime)joining_date).ToString("dd/MM/yyyy");
      if (comments != null)
      {
        if (comments.Length > 40)
          about += " . " + comments.Substring(0, 40);
        else
          about += " . " + comments.Substring(0, comments.Length);
      }
      if (comments != null && comments.Length > 40)
        about += "...";
      else
        about += ".";
      return about;
    }

    // photo_path, first_name, joining_date are required
    // first_name cannot be empty string
    public Dictionary<String, TraineeDetails.Error> Validate()
    {
      Dictionary<String, TraineeDetails.Error> errors = new Dictionary<String, TraineeDetails.Error>();

      if (photo_path == null)
      {
        errors.Add("photo_path", TraineeDetails.Error.IsRequired);
      }
      if (first_name == null || first_name == "")
      {
        errors.Add("first_name", TraineeDetails.Error.IsRequired);
      }
      if (joining_date == null)
      {
        errors.Add("joining_date", TraineeDetails.Error.IsRequired);
      }

      return errors;
    }

    public void Copy(ProfileDetails d)
    {
      d.photo_path = photo_path;
      d.first_name = first_name;
      d.last_name = last_name;
      d.age = age;
      d.sex = sex;
      d.comments = comments;
      d.joining_date = joining_date;
    }
  }

  public class ContactDetails
  {
    public string? phone_no { get; set; }
    public string? email { get; set; }
    public string? address { get; set; }
    public string? city { get; set; }
    public bool city_exists { get { return city != null; } }

    public Dictionary<String, TraineeDetails.Error> Validate()
    {
      return new Dictionary<String, TraineeDetails.Error>();
    }

    public void Copy(ContactDetails d)
    {
      d.phone_no = phone_no;
      d.email = email;
      d.address = address;
      d.city = city;
    }
  }

  public class TraineeDetails
  {
    public long? _id;

    public ProfileDetails profile_details { get; set; } = new ProfileDetails();
    public ContactDetails contact_details { get; set; } = new ContactDetails();

    public void Copy(TraineeDetails d)
    {
      d._id = _id;

      profile_details.Copy(d.profile_details);
      contact_details.Copy(d.contact_details);
    }

    public enum Error
    {
      IsRequired,
      InvalidData,
    }

    public Dictionary<String, Error> Validate()
    {
      var errors = new Dictionary<String, Error>();

      if (profile_details.Validate().Count != 0)
      {
        errors.Add("profile_details", Error.InvalidData);
      }
      if (contact_details.Validate().Count != 0)
      {
        errors.Add("contact_details", Error.InvalidData);
      }

      return errors;
    }
  }
}
