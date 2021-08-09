using System;
using System.IO;

namespace GymManagementSystem
{
  public class DataPath
  {
    private string GetMainUserDataParentFolderPath()
    {
      string path = Path.Combine(
          Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
          "ApGymManagementApplicationData"
          );
      Directory.CreateDirectory(path);
      return path;
    }

    public string GetMainDatabasePath()
    {
      return Path.Combine(
          GetMainUserDataParentFolderPath(),
          "main.db"
          );
    }

    public string GetMainProfilePhotoStoragePath()
    {
      string path = Path.Combine(
          GetMainUserDataParentFolderPath(),
          "ProfilePhotoStorage"
          );
      Directory.CreateDirectory(path);
      return path;
    }

    public string GetFullPathOfPhotoFromMainProfilePhotoStorage(string rel_path)
    {
      return Path.Combine(
          GetMainProfilePhotoStoragePath(),
          rel_path
          );
    }
  }
}
