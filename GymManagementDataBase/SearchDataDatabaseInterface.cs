using System;
using System.Data.SQLite;
using System.Collections.Generic;
using GymManagementSystem;
using GymManagementDataModel;

namespace GymManagementDataBase
{
  public class SearchDataDatabaseInterface
  {
    public void BindSearchParams(SQLiteCommand cmd, SearchData sd)
    {
      foreach (var i in sd.searchParamsLong)
      {
        cmd.Parameters.AddWithValue(i.Key, i.Value);
      }
      foreach (var i in sd.searchParamsInt)
      {
        cmd.Parameters.AddWithValue(i.Key, i.Value);
      }
      foreach (var i in sd.searchParamsString)
      {
        cmd.Parameters.AddWithValue(i.Key, i.Value);
      }
    }
  }
}
