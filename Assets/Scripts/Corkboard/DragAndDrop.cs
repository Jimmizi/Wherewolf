using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragAndDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private RectTransform rectTransform;
    private Image image;
    private Canvas canvas;

    private static int currentSortOrder=1;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        canvas = GetComponent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        currentSortOrder++;
        canvas.sortingOrder = currentSortOrder;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Color color = image.color;
        color.a = 0.8f;
        image.color = color;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Color color = image.color;
        color.a = 1.0f;
        image.color = color;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta;
    }
}
