using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MemoPile : MonoBehaviour, IDropHandler, IBeginDragHandler, IDragHandler, IPointerClickHandler
{
    [SerializeField]
    protected bool createNewMemos = true;

    [SerializeField]
    protected bool collectMemos = true;

    protected List<MemoData> memos = new List<MemoData>();

    RectTransform _rectTransform;
    protected RectTransform rectTransform
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

        if (collectMemos)
        {
            OnMemoDropped(memo);
        }
    }

    protected virtual void OnMemoDropped(Memo memo)
    {
        MemoData data = memo.Data;
        bool hasMessage = data.message != null && data.message.Length > 0;
        bool hasEmotes = data.emotes != null && data.emotes.Count > 0;

        // Avoid "collecting" empty notes
        if (hasMessage || hasEmotes)
        {
            memos.Add(memo.Data);
        }

        memo.Destroy();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (memos.Count <= 0)
        {
            if (createNewMemos)
            {
                Memo newMemo = CreateNewMemo();
                eventData.pointerDrag = newMemo.gameObject;

                foreach (var handler in eventData.pointerDrag.GetComponentsInChildren< IPointerDownHandler>())
                {
                    handler.OnPointerDown(eventData);
                }

                foreach (var handler in eventData.pointerDrag.GetComponentsInChildren<IBeginDragHandler>())
                {
                    handler.OnBeginDrag(eventData);
                }
            }
        }
        else
        {
            Memo newMemo = CreateMemoFromPile();
            eventData.pointerDrag = newMemo.gameObject;
        }
    }

    protected virtual Memo CreateNewMemo()
    {
        return MemoFactory.instance.CreateNew(rectTransform.anchoredPosition, false);
    }

    protected virtual Memo CreateMemoFromPile()
    {
        Memo newMemo = MemoFactory.instance.CreateFromData(memos[memos.Count - 1], rectTransform.anchoredPosition);
        memos.RemoveAt(memos.Count - 1);
        return newMemo;
    }

    public void OnDrag(PointerEventData eventData)
    {
    }

    public void OnPointerClick(PointerEventData eventData)
    {
    }
}
