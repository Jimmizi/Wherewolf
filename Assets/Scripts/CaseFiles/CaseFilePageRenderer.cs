using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class CaseFilePageRenderer : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler {
    public RectTransform Content;
    public Vector2 ScreenPadding = new Vector2(20f, 20f);

    private float _contentSize;
    private List<RectTransform> _sections = new List<RectTransform>();

    private RectTransform _rectTransform;
    private Vector2 _lastMousePosition;
    private Vector3 _lastPosition;
    private Vector3 _dragBeginOffset;
    private Canvas _canvas;

    private void Awake() {
        _sections = new List<RectTransform>();
        _contentSize = 0f;

        if (_canvas == null) {
            _canvas = GetComponentInParent<Canvas>();
        }

        if (_rectTransform == null) {
            _rectTransform = GetComponent<RectTransform>();
        }
    }

    public void AddSection(RectTransform section) {
        _sections.Add(section);
        section.SetParent(Content, false);
        /* TODO: Add dynamic spacing, as set in VerticalLayoutGroup */
        _contentSize += section.rect.height + 20f;
    }

    public void SetCanvas(Canvas canvas) {
        _canvas = canvas;
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

    private Vector2 ScreenPointToLocalPointInRectangle(Vector2 screenPoint) {
        if (_canvas == null) return screenPoint;

        Vector2 position = screenPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform, screenPoint,
            _canvas.worldCamera, out position);
        return position;
    }


    public void OnBeginDrag(PointerEventData eventData) {
        _dragBeginOffset = _canvas.transform.TransformPoint(ScreenPointToLocalPointInRectangle(eventData.position)) -
                           _rectTransform.position;
        Service.TooltipManager.DisableTooltips();
    }

    public void OnDrag(PointerEventData eventData) {
        Vector2 position = ScreenPointToLocalPointInRectangle(eventData.position);
        Vector3 oldPosition = _rectTransform.position;

        _rectTransform.position = _canvas.transform.TransformPoint(position) - _dragBeginOffset;

        if (!IsRectTransformInsideScreen(_rectTransform)) {
            _rectTransform.position = oldPosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData) {
        Service.TooltipManager.EnableTooltips();
    }

    private bool IsRectTransformInsideScreen(RectTransform rectTransform) {
        float canvasScale = 1f;

        if (_canvas != null) {
            canvasScale = _canvas.scaleFactor;
        }
        
        bool isInside = false;
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        int visibleCorners = 0;
        Vector2 modifiedScreenPadding = ScreenPadding * canvasScale;

        Rect rect = new Rect(
            modifiedScreenPadding.x, modifiedScreenPadding.y,
            Screen.width - modifiedScreenPadding.x * 2, Screen.height - modifiedScreenPadding.y * 2);

        foreach (Vector3 corner in corners) {
            if (rect.Contains(corner)) {
                visibleCorners++;
            }
        }

        if (visibleCorners > 0) {
            isInside = true;
        }

        return isInside;
    }
}