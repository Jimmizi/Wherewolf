using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class CaseFilePageRenderer : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler {
    public RectTransform Content;

    private float _contentSize;
    private List<RectTransform> _sections;

    private RectTransform _rectTransform;
    private Vector2 _lastMousePosition;
    private Canvas _canvas;
    private float _canvasScale;

    private void Start() {
        _sections = new List<RectTransform>();
        _contentSize = 0f;
        _canvas = GetComponentInParent<Canvas>();
        _canvasScale = (_canvas != null) ? _canvas.scaleFactor : 1f;
        _rectTransform = GetComponent<RectTransform>();
    }

    public void AddSection(RectTransform section) {
        _sections.Add(section);
        section.SetParent(Content, false);
        /* TODO: Add dynamic spacing, as set in VerticalLayoutGroup */
        _contentSize += section.rect.height + 20f;
    }

    public bool TryAddSection(RectTransform section) {
        section.SetParent(Content, false);
        /* TODO: Add dynamic spacing, as set in VerticalLayoutGroup */
        _contentSize += section.rect.height + 20f;

        if (SpaceLeft() <= 0f) {
            _contentSize -= section.rect.height;
            return false;
        }
        
        _sections.Add(section);
        return true;
    }

    public float SpaceLeft() {
        return Content.rect.height - _contentSize;
        //if (_sections.Count == 0) return Content.rect.height;
        //return Content.rect.height - _sections.Last().rect.yMax;
    }
    
    public void OnBeginDrag(PointerEventData eventData) {
        _lastMousePosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData) {
        if (_canvas == null) return;
        
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform, eventData.position, _canvas.worldCamera, out pos);
        _rectTransform.position = _canvas.transform.TransformPoint(pos);
        
        // Vector2 currentMousePosition = eventData.position;
        // Vector2 diff = currentMousePosition - _lastMousePosition;
        // RectTransform rect = GetComponent<RectTransform>();
        //
        // Vector3 newPosition = rect.position / _canvasScale + new Vector3(diff.x, diff.y, transform.position.z);
        // Vector3 oldPos = rect.position;
        //
        // rect.position = newPosition * _canvasScale;
        //
        // if (!IsRectTransformInsideScreen(rect)) {
        //     rect.position = oldPos;
        // }
        //
        // _lastMousePosition = currentMousePosition;
    }

    public void OnEndDrag(PointerEventData eventData) {
        // Does nothing.
    }

    private bool IsRectTransformInsideScreen(RectTransform rectTransform) {
        bool isInside = false;
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        int visibleCorners = 0;
        Rect rect = new Rect(0, 0, Screen.width, Screen.height);
        foreach (Vector3 corner in corners) {
            if (rect.Contains(corner)) {
                visibleCorners++;
            }
        }

        if (visibleCorners == 4) {
            isInside = true;
        }

        return isInside;
    }
}