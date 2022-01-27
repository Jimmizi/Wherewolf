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
                Memo newMemo = Instantiate(memoPrefab, memoHolder);
                MemoData memoData = new MemoData();
                memoData.position = rectTransform.anchoredPosition;
                memoData.size = new Vector2(300, 256);
                memoData.highlighted = false;
                memoData.editable = true;
                memoData.memoId = baseNewId;
                baseNewId++;

                newMemo.Data = memoData;
                eventData.pointerDrag = newMemo.gameObject;
            }
        }
        else
        {
            Memo newMemo = Instantiate(memoPrefab, memoHolder);
            MemoData memoData = memos[memos.Count - 1];
            memoData.position = rectTransform.anchoredPosition;
            newMemo.Data = memoData;
            memos.RemoveAt(memos.Count - 1);

            eventData.pointerDrag = newMemo.gameObject;
        }
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
