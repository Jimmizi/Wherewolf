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

    [SerializeField]
    bool createNew = true;

    List<MemoData> memos = new List<MemoData>();

    static int baseNewId = 2000;

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
            if (createNew)
            {
                Memo newMemo = MemoFactory.instance.CreateNew(rectTransform.anchoredPosition, false);
                eventData.pointerDrag = newMemo.gameObject;
            }
        }
        else
        {
            Memo newMemo = MemoFactory.instance.CreateFromData(memos[memos.Count - 1], rectTransform.anchoredPosition);
            memos.RemoveAt(memos.Count - 1);

            eventData.pointerDrag = newMemo.gameObject;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
    }

    public void OnPointerClick(PointerEventData eventData)
    {
    }
}
