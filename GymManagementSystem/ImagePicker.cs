using System;
using System.IO;

namespace GymManagementSystem
{
  public class ImagePicker
  {
    public string? PickAndGetImage()
    {
      Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
      openFileDialog.Filter = "Only jpg, jpeg or png|*.jpg;*.jpeg;*.png";
      if (openFileDialog.ShowDialog() == true)
      {
        return openFileDialog.FileName;
      }
      else
      {
        return null;
      }
    }
  }
}
