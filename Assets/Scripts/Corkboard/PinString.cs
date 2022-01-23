using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinString : MonoBehaviour
{
    RectTransform rectTransform;
    Vector2 start;
    Vector2 end;

    const float destroyDistance = 5.0f;

    public Vector2 LineStart
    {
        get => start;
        set { start = value; UpdateTransform(); }
    }

    public Vector2 LineEnd
    {
        get => end;
        set { end = value; UpdateTransform(); }
    }

    private void UpdateTransform()
    {
        if (!rectTransform)
        {
            rectTransform = GetComponent<RectTransform>();
        }

        Vector2 stringVector = end - start;
        float angle = Mathf.Atan2(stringVector.y, stringVector.x) * Mathf.Rad2Deg;
        rectTransform.sizeDelta = new Vector2(stringVector.magnitude, 3);
        rectTransform.rotation = Quaternion.Euler(0, 0, angle);
        rectTransform.position = start;
    }

    public void Update()
    {
        if (Input.GetMouseButton(1))
        {
            float distance = GetDistanceToPoint(Input.mousePosition);
            if (distance < destroyDistance)
            {
                Destroy(gameObject);
            }
        }
    }

    public float GetDistanceToPoint(Vector2 point)
    {
        Vector2 line = end - start;
        Vector2 normal = new Vector2(line.y, -line.x);
        normal.Normalize();

        Vector2 startPoint = point - start;
        Vector2 pointProjection = point + Vector2.Dot(startPoint, normal) * normal;

        if (Vector2.Dot((pointProjection - start), line) < 0)
        {
            return startPoint.magnitude;
        }
        if (Vector2.Dot((pointProjection - end), -line) < 0)
        {
            return (point - end).magnitude;
        }
        
        return (point - pointProjection).magnitude;
    }
}