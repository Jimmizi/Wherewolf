using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Pin : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField]
    StringRenderer stringPrefab;

    [SerializeField]
    GameObject stringHolder;

    StringRenderer stringObject;
    RectTransform rectTransform;
    bool stringAttached;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        stringObject = Instantiate<StringRenderer>(stringPrefab, stringHolder.transform);
        stringObject.LineStart = rectTransform.position;
        stringObject.LineEnd = rectTransform.position;
        stringAttached = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        stringObject.LineEnd = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!stringAttached)
        {
            Destroy(stringObject);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag)
        {
            var otherPin = eventData.pointerDrag.GetComponent<Pin>();
            if (otherPin)
            {
                otherPin.stringAttached = true;
                otherPin.stringObject.LineEnd = rectTransform.position;
            }
        }
    }
}
