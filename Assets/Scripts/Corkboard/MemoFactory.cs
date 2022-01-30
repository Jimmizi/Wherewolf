using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoFactory : MonoBehaviour
{
    private static MemoFactory _instance;
    public static MemoFactory instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<MemoFactory>();
            }
            return _instance;
        }
    }

    [SerializeField]
    private Transform memoHolder;

    [SerializeField]
    private Memo memoPrefab;

    [SerializeField]
    private RectTransform defaultRectTransform;

    [SerializeField]
    private int baseNewId = 200;
    
    public delegate void MemoCreatedDelegate(Memo newMemo);
    public event MemoCreatedDelegate OnMemoCreated;

    void Start()
    {
        if (!_instance)
        {
            _instance = this;
        }
    }

    public Memo CreateNew(int memoId, string message, List<Emote> emotes, Vector2 position, Vector2 size, bool highlighted = true, bool editable = false)
    {        
        MemoData memoData = new MemoData();
        memoData.position = position;
        memoData.size = size;
        memoData.highlighted = highlighted;
        memoData.editable = editable;
        memoData.memoId = memoId;
        memoData.message = message;
        memoData.emotes = emotes;
        memoData.title = CreateTitle();

        return CreateFromData(memoData);
    }

    public Memo CreateNew(string message, Vector2 position, bool highlighted = true, bool editable = false)
    {
        Vector2 defaultSize = defaultRectTransform ? defaultRectTransform.sizeDelta : new Vector2(300, 256);
        return CreateNew(++baseNewId, message, null, position, defaultSize, highlighted, editable);
    }

    public Memo CreateNew(List<Emote> emotes, Vector2 position, bool highlighted = true)
    {
        Vector2 defaultSize = defaultRectTransform ? defaultRectTransform.sizeDelta : new Vector2(300, 256);
        return CreateNew(++baseNewId, "", emotes, position, defaultSize, highlighted, false);
    }

    public Memo CreateNew(Vector2 position, bool highlighted = true)
    {        
        return CreateNew("", position, highlighted, true);
    }

    public Memo CreateNew(string message, bool highlighted = true, bool editable = false)
    {
        Vector3 defaultPosition = defaultRectTransform ? defaultRectTransform.position : new Vector3(0, 0);
        return CreateNew(message, defaultPosition, highlighted, editable);
    }

    public Memo CreateNew(List<Emote> emotes, bool highlighted = true)
    {
        Vector3 defaultPosition = defaultRectTransform ? defaultRectTransform.position : new Vector3(0, 0);
        return CreateNew(emotes, defaultPosition, highlighted);
    }

    public Memo CreateNew(bool highlighted = true)
    {
        return CreateNew("", highlighted, true);
    }

    public Memo CreateFromData(MemoData memoData, Vector2 position)
    {
        Memo newMemo = Instantiate(memoPrefab, memoHolder);
        memoData.position = position;
        newMemo.Data = memoData;

        OnMemoCreated?.Invoke(newMemo);

        return newMemo;
    }

    public Memo CreateFromData(MemoData memoData)
    {
        return CreateFromData(memoData, memoData.position);
    }

    private string CreateTitle()
    {
        if (Service.Game)
        {
            return Service.Game.CurrentTimeOfDay.ToString() + " " + Service.Game.CurrentDay;
        }

        return "";
    }
}
