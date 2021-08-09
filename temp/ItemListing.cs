  public class ItemListing<T>
  {
    List<T> list = new List<T>();
    int start_pos = 0;
    int end_pos = 0;

    int row_count = 4;
    int one_time_fetch;
    int max_in_store;

    public ItemListing()
    {
      one_time_fetch = row_count * row_count;
      max_in_store = row_count * one_time_fetch;
    }

    private int GetNextStartPos()
    {
      return end_pos;
    }

    private int GetPrevEndPos()
    {
      return start_pos;
    }

    public (int, int) GetNextRangeOfMaxElementsRange()
    {
      return (GetNextStartPos(), one_time_fetch);
    }

    public (int, int) GetPrevRangeOfMaxElementsRange()
    {
      var expected = GetPrevEndPos() - one_time_fetch;
      if (expected < 0)
        return (0, GetPrevEndPos());
      else
        return (expected, one_time_fetch);
    }

    public List<T> GetList()
    {
      return list;
    }

    // Atmost only first one_time_fetch from the list will be added
    public void AddData(List<T> dl)
    {
      var new_count = GetNewEntryCount(dl.Count);
      MakeSpace(new_count);
      end_pos += new_count;
      list.AddRange(dl.GetRange(0, new_count));
    }

    // Atmost only last one_time_fetch from the list will be added
    public void ReverseAddData(List<T> dl)
    {
      var new_count = GetNewEntryCount(dl.Count);
      ReverseMakeSpace(new_count);
      start_pos -= new_count;
      list.InsertRange(0, dl.GetRange(dl.Count - new_count, new_count));
    }

    private int GetNewEntryCount(int actual_count)
    {
      if (actual_count > one_time_fetch)
        return one_time_fetch;
      else
        return actual_count;
    }

    private void ReverseMakeSpace(int new_count)
    {
      var overflow_count = GetOverflowCount(new_count);
      end_pos -= overflow_count;
      list.RemoveRange(list.Count - overflow_count, overflow_count);
    }

    private void MakeSpace(int new_count)
    {
      var overflow_count = GetOverflowCount(new_count);
      start_pos += overflow_count;
      list.RemoveRange(0, overflow_count);
    }

    private int GetOverflowCount(int new_count)
    {
      var expected_count = (list.Count + new_count);
      if (expected_count > max_in_store)
        return ((expected_count - max_in_store) / row_count) * row_count;
      else
        return 0;
    }

    //private void Clean(List<T> dl)
    //{
    //  foreach(var e in list)
    //  {
    //    if(e.Equals(dl[0]))
    //    {
    //      var i = list.IndexOf(e);
    //      start_pos += (list.Count - i);
    //      list.RemoveRange(i, list.Count - i);
    //    }
    //  }
    //}

    public void Clear()
    {
      list.RemoveRange(0, list.Count);
      start_pos = 0;
      end_pos = 0;
    }
  }
