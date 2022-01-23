using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Pin : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField]
    PinString stringPrefab;

    [SerializeField]
    GameObject stringHolder;

    PinString stringObject;
    RectTransform rectTransform;

    List<PinString> lineStarts = new List<PinString>();
    List<PinString> lineEnds = new List<PinString>();

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rectTransform.hasChanged)
        {
            foreach(PinString pinString in lineStarts)
            {
                if (pinString)
                {
                    pinString.LineStart = rectTransform.position;
                }
            }

            foreach (PinString pinString in lineEnds)
            {
                if (pinString)
                {
                    pinString.LineEnd = rectTransform.position;
                }
            }

            rectTransform.hasChanged = false;
        }    
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        stringObject = Instantiate<PinString>(stringPrefab, stringHolder.transform);
        stringObject.LineStart = rectTransform.position;
        stringObject.LineEnd = rectTransform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (stringObject)
        {
            stringObject.LineEnd = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (stringObject)
        {
            Destroy(stringObject.gameObject);
            stringObject = null;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag)
        {
            var otherPin = eventData.pointerDrag.GetComponent<Pin>();
            if (otherPin)
            {                
                otherPin.stringObject.LineEnd = rectTransform.position;
                otherPin.lineStarts.Add(otherPin.stringObject);
                lineEnds.Add(otherPin.stringObject);
                otherPin.stringObject = null;

                RemoveMissingStrings();
                otherPin.RemoveMissingStrings();
            }
        }
    }

    private void RemoveMissingStrings()
    {
        for (var i = lineStarts.Count - 1; i > -1; i--)
        {
            if (!lineStarts[i])
            {
                lineStarts.RemoveAt(i);
            }
        }

        for (var i = lineEnds.Count - 1; i > -1; i--)
        {
            if (!lineEnds[i])
            {
                lineEnds.RemoveAt(i);
            }
        }
    }    
}
