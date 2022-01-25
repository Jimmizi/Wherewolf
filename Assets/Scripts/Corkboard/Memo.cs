using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct MemoData
{
    public int memoId;
    public string message;
    public Vector2 position;
    public Vector2 size;
    public List<int> connectedIds;
}

public class Memo : MonoBehaviour
{
    [SerializeField]
    private Pin pin;

    [SerializeField]
    private Text note;

    RectTransform rectTransform;

    MemoData Data
    {
        get
        {
            return new MemoData
            {
                memoId = pin ? pin.PinId : -1,
                connectedIds = pin ? pin.ConnectedIds : new List<int>(),
                message = note ? note.text : "",
                position = rectTransform ? rectTransform.anchoredPosition : Vector2.zero,
                size = rectTransform ? rectTransform.sizeDelta : new Vector2(300, 256)
            };
        }

        set
        {
            if (pin)
            {
                pin.PinId = value.memoId;
                pin.ConnectedIds = value.connectedIds;
            }

            if (note) { note.text = value.message; }

            if (rectTransform)
            {
                rectTransform.anchoredPosition = value.position;
                rectTransform.sizeDelta = value.size;
            }
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
