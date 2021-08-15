using System;
using GymManagementDataModel;
using GymManagementLogic;
using GymManagementSystem;
using System.Collections.Generic;

namespace GymManagementHILogic
{
  public class ImagePicker
  {
    public string? PickAndGetImage()
    {
      return (new GymManagementSystem.ImagePicker()).PickAndGetImage();
    }
  }
}
