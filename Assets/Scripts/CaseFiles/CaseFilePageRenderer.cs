using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class CaseFilePageRenderer : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler {
    public RectTransform Content;

    private float _contentSize;
    private List<RectTransform> _statements;
    
    private Vector2 _lastMousePosition;

    private void Awake() {
        _statements = new List<RectTransform>();
        _contentSize = 0f;
    }

    public void AddStatement(RectTransform statement) {
        _statements.Add(statement);
        statement.SetParent(Content, false);
        /* TODO: Add dynamic spacing, as set in VerticalLayoutGroup */
        _contentSize += statement.rect.height + 20f;
    }

    public bool TryAddStatement(RectTransform statement) {
        statement.SetParent(Content, false);
        /* TODO: Add dynamic spacing, as set in VerticalLayoutGroup */
        _contentSize += statement.rect.height + 20f;

        if (SpaceLeft() <= 0f) {
            _contentSize -= statement.rect.height;
            return false;
        }
        
        _statements.Add(statement);
        return true;
    }

    public float SpaceLeft() {
        return Content.rect.height - _contentSize;
        //if (_statements.Count == 0) return Content.rect.height;
        //return Content.rect.height - _statements.Last().rect.yMax;
    }


    public void OnBeginDrag(PointerEventData eventData) {
        _lastMousePosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData) {
        Vector2 currentMousePosition = eventData.position;
        Vector2 diff = currentMousePosition - _lastMousePosition;
        RectTransform rect = GetComponent<RectTransform>();

        Vector3 newPosition = rect.position + new Vector3(diff.x, diff.y, transform.position.z);
        Vector3 oldPos = rect.position;
        rect.position = newPosition;
        if (!IsRectTransformInsideSreen(rect)) {
            rect.position = oldPos;
        }

        _lastMousePosition = currentMousePosition;
    }

    public void OnEndDrag(PointerEventData eventData) {
        //Implement your funtionlity here
    }

    private bool IsRectTransformInsideSreen(RectTransform rectTransform) {
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