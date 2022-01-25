using System;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class CharacterData {
    private int _vertexIndex;
    private int _characterIndex;
    
    private Color _color;
    
    private Vector3 _translation = Vector3.zero;
    private Vector3 _scale = Vector3.one;
    private Quaternion _rotation = quaternion.identity;
    
    public int VertexIndex {
        get { return _vertexIndex; }
    }
    
    public int CharacterIndex => _characterIndex;
    
    public Color Color {
        get { return _color;  }
        set { _color = value; }
    }
    
    public Vector3 Translation {
        get => _translation;
        set => _translation = value;
    }

    public Vector3 Scaling {
        get => _scale;
        set => _scale = value;
    }

    public Quaternion Rotation {
        get => _rotation;
        set => _rotation = value;
    }

    public void Translate(Vector3 to) {
        _translation += to;
    }

    public void Scale(Vector3 to) {
        _scale.Scale(to);
    }

    public void Rotate(Quaternion to) {
        _rotation *= to;
    }

    public void Reset() {
        _translation = Vector3.zero;
        _scale = Vector3.one;
        _rotation = Quaternion.identity;
    }

    public CharacterData(int characterIndex) {
        _characterIndex = characterIndex;
    }
}