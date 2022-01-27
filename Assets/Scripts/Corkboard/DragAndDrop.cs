using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragAndDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private RectTransform _rectTransform;
    private Image _image;
    private Canvas _canvas;

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

    private Image image
    {
        get
        {
            if (!_image)
            {
                _image = GetComponent<Image>();
            }
            return _image;
        }
    }

    private Canvas canvas
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

    private static int currentSortOrder=1;

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
