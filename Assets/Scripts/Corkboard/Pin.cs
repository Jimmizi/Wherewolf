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

    [SerializeField]
    private int pinId = -1;
    public int PinId
    {
        get => pinId;
        set
        {
            if (pinId != -1)
            {
                if (pinsById.ContainsKey(pinId))
                {
                    pinsById.Remove(pinId);
                }
            }
            
            pinId = value;

            if (pinId != -1)
            {
                Pin existingPin;
                if (pinsById.TryGetValue(pinId, out existingPin))
                {
                    if (existingPin)
                    {
                        existingPin.pinId = -1;
                    }
                }
                pinsById[pinId] = this;
            }
        }
    }

    public List<int> connectedIds = new List<int>();
    public List<int> ConnectedIds
    {
        get { return connectedIds; }
        set
        { 
            foreach (int pinId in value)
            {
                AddConnection(pinId, null);
            }
        }
    }

    private static Dictionary<int, Pin> pinsById;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        PinId = pinId; // update the pin map
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
                if (!otherPin.AddConnection(this, otherPin.stringObject))
                {
                    Destroy(otherPin.stringObject.gameObject);
                }

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

    private bool AddConnection(int pinId, PinString pinString)
    {
        Pin otherPin;
        if (pinsById.TryGetValue(pinId, out otherPin) && otherPin)
        {
            return AddConnection(otherPin, pinString);
        }
        else
        {
            connectedIds.Add(pinId);
        }

        return false;
    }

    private bool AddConnection( Pin pin, PinString pinString )
    {
        if (!pin || connectedIds.Contains(pin.pinId))
        {
            return false;
        }

        connectedIds.Add(pin.pinId);
        
        if (!pin.connectedIds.Contains(pinId))
        {
            pin.connectedIds.Add(pinId);
        }

        if (!pinString)
        {
            stringObject = Instantiate<PinString>(stringPrefab, stringHolder.transform);
        }

        pinString.LineStart = rectTransform.position;
        pinString.LineEnd = pin.rectTransform.position;

        pinString.PinStart = this;
        pinString.PinEnd = pin;

        lineStarts.Add(pinString);
        pin.lineEnds.Add(pinString);
        return true;
    }

    public bool RemoveConnection( Pin pin )
    {
        if (pin)
        {
            return connectedIds.Remove(pin.pinId);
        }

        return false;
    }
}
