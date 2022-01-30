using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct MemoData
{
    public int memoId;
    public string title;
    public string message;
    public List<Emote> emotes;
    public Vector2 position;
    public Vector2 size;
    public List<int> connectedIds;
    public bool highlighted;
    public bool editable;
}

public class Memo : MonoBehaviour
{
    [SerializeField]
    private Pin pin;

    [SerializeField]
    private Text title;

    [SerializeField]
    private InputField note;    

    [SerializeField]
    private EmoteTextRenderer emoteRenderer; 

    [SerializeField]
    private GameObject highlight;

    private List<Emote> emotes = new List<Emote>();

    RectTransform _rectTransform;
    public RectTransform rectTransform
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

    Canvas _canvas;
    public Canvas canvas
    {
        get
        {
            if (!_canvas)
            {
                _canvas = GetComponent<Canvas>();
            }

            return _canvas;
        }
    }

    public int sortOrder
    {
        get { return canvas ? canvas.sortingOrder : 0; }
        set { if (canvas) canvas.sortingOrder = value; }
    }

    public MemoData Data
    {
        get
        {
            return new MemoData
            {
                memoId = pin ? pin.PinId : -1,
                title = title ? title.text : "",
                connectedIds = pin ? pin.ConnectedIds : new List<int>(),
                message = note ? note.text : "",
                position = rectTransform ? rectTransform.anchoredPosition : Vector2.zero,
                size = rectTransform ? rectTransform.sizeDelta : new Vector2(300, 256),
                highlighted = highlight ? highlight.activeSelf : false,
                editable = note ? note.interactable : false,
                emotes = emotes,
            };
        }

        set
        {
            if (pin)
            {
                pin.PinId = value.memoId;
                pin.ConnectedIds = value.connectedIds;
            }

            if (rectTransform)
            {
                rectTransform.anchoredPosition = value.position;
                rectTransform.sizeDelta = value.size;
            }

            if (highlight)
            {
                highlight.SetActive(value.highlighted);
            }

            if (title)
            {
                title.text = value.title;
            }

            bool useEmotes = true;
            emotes = value.emotes;
            if (emoteRenderer)
            {
                useEmotes = (emotes != null && emotes.Count > 0);
                if (useEmotes)
                {
                    emoteRenderer.Render(emotes);
                }
                else
                {
                    emoteRenderer.Clear();
                }
            }

            if (note)
            {
                note.text = value.message;
                note.interactable = value.editable;
                var texts = note.GetComponentsInChildren<Text>();
                foreach (Text text in texts)
                {
                    text.raycastTarget = value.editable;
                }

                note.gameObject.SetActive(!useEmotes);
            }
        }

    }

    public bool Highlighted
    {
        get { return highlight ? highlight.activeSelf : false; }
        set { if (highlight) { highlight.SetActive(value); } }
    }

    public void Destroy()
    {
        if (pin)
        {
            pin.DestroyStrings();
        }

        Destroy(gameObject);
    }    
}
