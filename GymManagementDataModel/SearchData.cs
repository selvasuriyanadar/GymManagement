using System;
using System.Collections.Generic;

namespace GymManagementDataModel
{
  public class SearchData
  {
    string searchString;
    public Dictionary<string, long> searchParamsLong;
    public Dictionary<string, int> searchParamsInt;
    public Dictionary<string, string> searchParamsString;

    public SearchData(string ss,
        Dictionary<string, long> spl,
        Dictionary<string, int> spi,
        Dictionary<string, string> sps)
    {
      searchString = ss;
      searchParamsLong = spl;
      searchParamsInt = spi;
      searchParamsString = sps;
    }

    public string GetSearchString()
    {
      return searchString;
    }
  }

  public class DataLineData
  {
    public long StartPosition { get; }
    public long TotalItems { get; }
    public int FetchedItems { get; }
    public long FetchPosition { get; }
    public long EndPosition { get { return StartPosition + TotalItems - 1; } }
    public long FetchEndPosition { get { return FetchPosition + FetchedItems - 1; } }

    public DataLineData(long startPosition, long totalItems, int fetchedItems, long fetchPosition)
    {
      StartPosition = startPosition;
      TotalItems = totalItems;
      FetchedItems = fetchedItems;
      FetchPosition = fetchPosition;
    }

    public int GetRelScrollPositionOnFetchedItems(long scrollItemPosition)
    {
      return (int) ((scrollItemPosition + 1) - FetchPosition);
    }
  }

  public class ListingViewData
  {
    public int? itemsPerRow;
    public int? oneTimeFetchRows;
    public int? maxRows;
    public double? itemHeight;

    public ListingViewData(int itemsPerRow, int oneTimeFetchRows, int maxRows, double itemHeight)
    {
      this.itemsPerRow = itemsPerRow;
      this.oneTimeFetchRows = oneTimeFetchRows;
      this.maxRows = maxRows;
      this.itemHeight = itemHeight;
    }

    public enum Error
    {
      IsRequired,
      ValueTooSmall,
    }

    public Dictionary<String, Error> Validate()
    {
      var errors = new Dictionary<String, Error>();

      if (itemsPerRow == null)
      {
        errors.Add("itemsPerRow", Error.IsRequired);
      }
      else if (!(itemsPerRow > 0))
      {
        errors.Add("itemsPerRow", Error.ValueTooSmall);
      }

      if (oneTimeFetchRows == null)
      {
        errors.Add("oneTimeFetchRows", Error.IsRequired);
      }
      else if (!(oneTimeFetchRows > 0))
      {
        errors.Add("oneTimeFetchRows", Error.ValueTooSmall);
      }
      else
      {
        if (maxRows == null)
        {
          errors.Add("maxRows", Error.IsRequired);
        }
        else if (!(maxRows >= oneTimeFetchRows))
        {
          errors.Add("maxRows", Error.ValueTooSmall);
        }
      }

      if (itemHeight == null)
      {
        errors.Add("itemHeight", Error.IsRequired);
      }
      else if (!(itemHeight > 0))
      {
        errors.Add("itemHeight", Error.ValueTooSmall);
      }

      return errors;
    }
  }
}
