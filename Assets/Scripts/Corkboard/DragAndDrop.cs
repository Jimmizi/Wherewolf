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
    private Memo _memo;

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

    private Memo memo
    {
        get
        {
            if (!_memo)
            {
                _memo = GetComponent<Memo>();
            }
            return _memo;
        }
    }

    private static int currentSortOrder=1;

    public void OnPointerDown(PointerEventData eventData)
    {
        currentSortOrder++;
        canvas.sortingOrder = currentSortOrder;

        if (memo)
        {
            memo.Highlighted = false;
        }
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

        if (_rectTransform.anchoredPosition.x >= Screen.width / 2)
        {
            _rectTransform.anchoredPosition = new Vector2(0.0f, _rectTransform.anchoredPosition.y);
        }
        else if (_rectTransform.anchoredPosition.x < -(Screen.width / 2))
        {
            _rectTransform.anchoredPosition = new Vector2(0.0f, _rectTransform.anchoredPosition.y);
        }

        if (_rectTransform.anchoredPosition.y >= Screen.height / 2)
        {
            _rectTransform.anchoredPosition = new Vector2(_rectTransform.anchoredPosition.x, 0.0f);
        }
        else if (_rectTransform.anchoredPosition.y < -(Screen.height / 2))
        {
            _rectTransform.anchoredPosition = new Vector2(_rectTransform.anchoredPosition.x, 0.0f);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta;
    }
}
