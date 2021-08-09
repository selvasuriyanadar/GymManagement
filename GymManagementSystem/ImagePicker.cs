using System;
using System.IO;
using System.Windows.Forms;

namespace GymManagementSystem
{
  public class ImagePicker
  {
    public string? PickAndGetImage()
    {
      OpenFileDialog openFileDialog = new OpenFileDialog();
      openFileDialog.Filter = "Only jpg, jpeg or png|*.jpg;*.jpeg;*.png";
      if (openFileDialog.ShowDialog() == DialogResult.OK)
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
