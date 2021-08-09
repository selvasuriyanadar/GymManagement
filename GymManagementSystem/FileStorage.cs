using System;
using System.IO;

namespace GymManagementSystem
{
  public class FileStorage
  {
    public string StoreProfilePhotoToMainStorage(string unique_file_name, string photo_path)
    {
      var data_path = new DataPath();
      string rel_path = unique_file_name + Path.GetExtension(photo_path);
      string path = Path.Combine(
          data_path.GetMainProfilePhotoStoragePath(),
          rel_path
          );
      File.Copy(photo_path, path);
      return rel_path;
    }
  }
}
