using System;
using System.Collections.Generic;
using GymManagementDataModel;
using GymManagementApi;

namespace GymManagementDataStore
{
  public class FetchBeyondCapacity : Exception
  {
    public FetchBeyondCapacity(string msg) : base(msg)
    {}
  }

  public class BucketFilled : Exception
  {
    public BucketFilled (string msg) : base(msg)
    {}
  }

  public class DataLineBucket
  {
    int bucketCapacity;
    long fetchPosition;
    int fetchedItems = 0;

    public DataLineBucket(int bucketCapacity, long fetchPosition)
    {
      if (bucketCapacity < 0)
      {
        throw new ArgumentException("bucket capacity cannot be lesser than zero.", nameof(bucketCapacity));
      }

      this.bucketCapacity = bucketCapacity;
      this.fetchPosition = fetchPosition;
    }

    public DataLineBucket Copy()
    {
      var d = new DataLineBucket(bucketCapacity, fetchPosition);
      d.IncRight(fetchedItems);
      return d;
    }

    public (int, long) GetState()
    {
      return (fetchedItems, fetchPosition);
    }

    public (long, long) GetFilledBucketRange()
    {
      return (fetchPosition, fetchPosition + fetchedItems);
    }

    private (long, long) GetCleanRangeForLeftInc(int inc)
    {
      return (GetFilledBucketRange().Item2 - GetCleanableItems(inc), GetFilledBucketRange().Item2);
    }

    private (long, long) GetCleanRangeForRightInc(int inc)
    {
      return (fetchPosition, fetchPosition + GetCleanableItems(inc));
    }

    private void _IncLeft(int inc)
    {
      if (GetCleanableItems(inc) != 0)
      {
        throw new BucketFilled("the bucket is full make space before any insertion.");
      }

      IncBucket(inc);
      fetchPosition -= inc;
    }

    public (long, long) IncLeft(int inc)
    {
      var rng = GetFilledBucketRange();
      _IncLeft(inc);
      var rng2 = GetFilledBucketRange();
      return (rng2.Item1, rng.Item1);
    }

    private void _IncRight(int inc)
    {
      if (GetCleanableItems(inc) != 0)
      {
        throw new BucketFilled("the bucket is full make space before any insertion.");
      }

      IncBucket(inc);
    }

    public (long, long) IncRight(int inc)
    {
      var rng = GetFilledBucketRange();
      _IncRight(inc);
      var rng2 = GetFilledBucketRange();
      return (rng.Item2, rng2.Item2);
    }

    private void _CleanBucketForLeftInc(int inc)
    {
      CleanBucket(GetCleanableItems(inc));
    }

    public (long, long) CleanBucketForLeftInc(int inc)
    {
      var rng = GetCleanRangeForLeftInc(inc);
      _CleanBucketForLeftInc(inc);
      return rng;
    }

    private void _CleanBucketForRightInc(int inc)
    {
      var dec = GetCleanableItems(inc);
      CleanBucket(dec);
      fetchPosition += dec;
    }

    public (long, long) CleanBucketForRightInc(int inc)
    {
      var rng = GetCleanRangeForRightInc(inc);
      _CleanBucketForRightInc(inc);
      return rng;
    }

    private void CleanBucket(int dec)
    {
      fetchedItems -= dec;
    }

    private void IncBucket(int inc)
    {
      fetchedItems += inc;
    }

    private int GetCleanableItems(int inc)
    {
      if (inc < 0)
      {
        throw new ArgumentException("increment cannot be lesser than zero.", nameof(inc));
      }

      if (inc > bucketCapacity)
      {
        throw new FetchBeyondCapacity("increment exceeds the bucket capacity.");
      }

      var expectedTotal = (fetchedItems + inc);
      if (expectedTotal > bucketCapacity)
      {
        return (expectedTotal - bucketCapacity);
      }
      return 0;
    }
  }

  public class DataLineList<T>
  {
    List<T> list = new List<T>();

    public List<T> GetList()
    {
      return list;
    }

    public void RemoveFromList((long, long) rng, (long, long) crng)
    {
      var relCrng = GetRelRange(rng, crng);
      if (relCrng.Item1 == 0 || relCrng.Item2 == list.Count)
        RemoveRange(relCrng.Item1 , (int) (crng.Item2 - crng.Item1));
    }

    public void AddToList((long, long) rng, (long, long) arng, List<T> dl)
    {
      var relArng = GetRelRange(rng, arng);
      if (relArng.Item1 == list.Count)
        InsertRange(list.Count, dl);
      else if (relArng.Item2 == 0)
        InsertRange(0, dl);
    }

    public void Clear()
    {
      RemoveRange(0, list.Count);
    }

    protected virtual void RemoveRange(int pos, int cnt)
    {
      list.RemoveRange(pos, cnt);
    }

    protected virtual void InsertRange(int pos, List<T> dl)
    {
      list.InsertRange(pos, dl);
    }

    protected (int, int) GetRelRange((long, long) rng, (long, long) lrng)
    {
      return ((int) (lrng.Item1 - rng.Item1), (int) (lrng.Item2 - rng.Item1));
    }
  }

  public class DataLineTravel<T>
  {
    public delegate long SearchFullCount(SearchData sd);
    public delegate List<T> Search(long offset, int count, SearchData sd);

    long startPosition = 0;
    long totalItems;
    int oneTimeFetch;
    DataLineBucket bucket;
    DataLineList<T> list;

    Search search;
    SearchFullCount searchFullCount;
    SearchData sd;

    public DataLineTravel(SearchData sd, Search search, SearchFullCount searchFullCount, DataLineList<T> dll, ListingViewData viewData)
    {
      list = dll;
      this.search = search;
      this.searchFullCount = searchFullCount;
      this.sd = sd;
      Reset(viewData);
    }

    public DataLineData GetStateData()
    {
      var bs = bucket.GetState();
      return new DataLineData(startPosition, totalItems, bs.Item1, bs.Item2);
    }

    public void Reset(ListingViewData vd)
    {
      Refresh(startPosition, vd);
    }

    public void RefreshStill(ListingViewData vd)
    {
      Refresh(bucket.GetFilledBucketRange().Item1, vd);
    }

    public void Refresh(long fetchPosition, ListingViewData vd)
    {
      if (vd.Validate().Count != 0)
      {
        throw new ArgumentException("invalid view data.", "view data");
      }

      long totalItems;
      try
      {
        totalItems = searchFullCount(sd);
      }
      catch
      {
        totalItems = 0;
      }

      this.totalItems = totalItems;
      oneTimeFetch = (int) vd.oneTimeFetchRows * (int) vd.itemsPerRow;

      var bc = (int) vd.maxRows * (int) vd.itemsPerRow;
      var b = new DataLineBucket(bc, fetchPosition);
      if (IsFilledBucketWithinRange(b))
      {
        SetBucket(b);
      }
      else
      {
        SetBucket(new DataLineBucket(bc, startPosition));
      }
    }

    // sets a fresh bucket (no prior left or right incs)
    private void SetBucket(DataLineBucket b)
    {
      bucket = b;
      list.Clear();

      IncRight();
    }

    public void IncLeft()
    {
      var n = GetLeftInc();

      var rng = bucket.GetFilledBucketRange();
      var crng = bucket.CleanBucketForLeftInc(n);
      list.RemoveFromList(rng, crng);

      rng = bucket.GetFilledBucketRange();
      var arng = bucket.IncLeft(n);
      AddToList(rng, arng);
    }

    public void IncRight()
    {
      var n = GetRightInc();

      var rng = bucket.GetFilledBucketRange();
      var crng = bucket.CleanBucketForRightInc(n);
      list.RemoveFromList(rng, crng);

      rng = bucket.GetFilledBucketRange();
      var arng = bucket.IncRight(n);
      AddToList(rng, arng);
    }

    private void AddToList((long, long) rng, (long, long) arng)
    {
      try
      {
        var dl = search(arng.Item1, (int) (arng.Item2 - arng.Item1), sd);
        list.AddToList(rng, arng, dl);
      }
      catch {}
    }

    private long GetEndPosition()
    {
      return startPosition + totalItems;
    }

    private int GetLeftInc()
    {
      var rng = bucket.GetFilledBucketRange();
      var e = rng.Item1 - startPosition;
      if (e >= oneTimeFetch)
        return oneTimeFetch;
      else
        return (int) e;
    }

    private int GetRightInc()
    {
      var rng = bucket.GetFilledBucketRange();
      var e = GetEndPosition() - rng.Item2;
      if (e >= oneTimeFetch)
        return oneTimeFetch;
      else
        return (int) e;
    }

    private bool IsFilledBucketWithinRange(DataLineBucket b)
    {
      var rng = b.GetFilledBucketRange();
      if (rng.Item1 >= startPosition && rng.Item2 <= GetEndPosition())
      {
        return true;
      }
      return false;
    }
  }
}
