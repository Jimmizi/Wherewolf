using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[ExecuteInEditMode]
public class StringRenderer : Graphic
{
    Vector2 start;
    Vector2 end;

    public Vector2 LineStart
    {
        get => start;
        set { start = value; SetAllDirty(); }
    }

    public Vector2 LineEnd
    {
        get => end;
        set { end = value; SetAllDirty(); }
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        Vector2 norm = new Vector2(start.y - end.y, end.x - start.x);
        norm.Normalize();
        norm *= 1.0f;

        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;

        vertex.position = start + norm;
        vh.AddVert(vertex);

        vertex.position = start - norm;
        vh.AddVert(vertex);

        vertex.position = end - norm;
        vh.AddVert(vertex);

        vertex.position = end + norm;
        vh.AddVert(vertex);

        vh.AddTriangle(0, 1, 2);
        vh.AddTriangle(2, 3, 0);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        SetAllDirty();
    }
}
