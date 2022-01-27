using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MemoPile : MonoBehaviour, IDropHandler, IBeginDragHandler, IDragHandler, IPointerClickHandler
{
    [SerializeField]
    Memo memoPrefab;

    [SerializeField]
    Transform memoHolder;

    List<MemoData> memos = new List<MemoData>();

    RectTransform _rectTransform;
    private RectTransform rectTransform
    {
        get
        {
            if (!_rectTransform)
            {
                _rectTransform = GetComponent<RectTransform>();
            }

            return _rectTransform;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (!eventData.pointerDrag)
        {
            return;
        }

        var memo = eventData.pointerDrag.GetComponent<Memo>();
        if (!memo)
        {
            return;
        }

        memos.Add( memo.Data );
        memo.Destroy();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (memos.Count <= 0)
        {
            return;
        }

        Memo newMemo = Instantiate(memoPrefab, memoHolder);
        MemoData memoData = memos[memos.Count - 1];
        memoData.position = rectTransform.anchoredPosition;
        newMemo.Data = memoData;
        memos.RemoveAt(memos.Count - 1);
        
        eventData.pointerDrag = newMemo.gameObject;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDrag(PointerEventData eventData)
    {
    }

    public void OnPointerClick(PointerEventData eventData)
    {
    }
}
