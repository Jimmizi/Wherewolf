using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MemoSorter : MonoBehaviour
{
    [SerializeField]
    private int minSortOrder = 10;

    [SerializeField]
    private int maxSortOrder = 160;

    List<Memo> memoList;

    private static MemoSorter _instance;
    public static MemoSorter instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<MemoSorter>();
            }
            return _instance;
        }
    }

    void Start()
    {
        memoList = new List<Memo>(FindObjectsOfType<Memo>());
        MemoFactory.instance.OnMemoCreated += OnMemoCreated;
        UpdateOrder();
    }

    public void PlaceOnTop(Memo memo)
    {
        memo.sortOrder = memoList.Count + minSortOrder;
        UpdateOrder();
    }

    private void OnMemoCreated(Memo newMemo)
    {
        if (newMemo)
        {
            memoList.Add(newMemo);
            PlaceOnTop(newMemo);            
        }
    }

    private void UpdateOrder()
    {
        RemoveMissing();
        SortBySortOrder();
        ApplyNewOrder();
    }

    private void RemoveMissing()
    {
        for (int i= memoList.Count-1; i>=0; i--)
        {
            if (!memoList[i])
            {
                memoList.RemoveAt(i);
            }
        }
    }

    private void SortBySortOrder()
    {
        memoList.Sort((memoA, memoB) => memoA.sortOrder.CompareTo(memoB.sortOrder));
    }

    private void ApplyNewOrder()
    {
        int topSortOrder = Mathf.Min(minSortOrder + memoList.Count - 1, maxSortOrder);
        int offset = topSortOrder - memoList.Count + 1;
        for (int i = memoList.Count - 1; i >= 0; i--)
        {            
            memoList[i].sortOrder = Mathf.Max(i + offset, minSortOrder);
        }
    }
}
