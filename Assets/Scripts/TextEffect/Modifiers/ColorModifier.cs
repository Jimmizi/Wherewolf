using TMPro;
using UnityEngine;

public class ColorModifier : BaseModifier {
    private Color[] _colors;
    private bool _gradient = true;

    public bool Gradient {
        get => _gradient;
        set => _gradient = value;
    }

    public Color[] Colors {
        get => _colors;
        set => _colors = value;
    }
    
    private Color32[] newVertexColors;
    private Color32 targetColor;

    public override void Modify(CharacterData characterData) {
        if (_colors == null || _colors.Length == 0) {
            return;
        }

        float p = Mathf.Repeat(_progress, 1f);
        int index = Mathf.FloorToInt(p * (_colors.Length - 1));

        if (_gradient && index < _colors.Length - 1) {
            float localProgress = (p - (((float) index) / (_colors.Length - 1))) * (_colors.Length - 1);
            characterData.Color = Color.Lerp(_colors[index], _colors[index + 1], localProgress);
        } else {
            characterData.Color = _colors[index];
        }
        
    }
}