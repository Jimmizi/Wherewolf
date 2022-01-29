using System;
using TMPro;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class Tooltip : MonoBehaviour {
    [SerializeField] private string _title;
    [SerializeField] private string _description;

    public TextMeshProUGUI TitleText;
    public TextMeshProUGUI DescriptionText;

    public string Title {
        get => _title;
        set {
            _title = value;
            SetText();;
        }
    }
    
    public string Description {
        get => _description;
        set {
            _description = value;
            SetText();
        }
    }

    private void SetText() {
        if (TitleText != null) {
            TitleText.text = _title;
        }

        if (DescriptionText != null) {
            DescriptionText.text = _description;
        }
    }

    private void Start() {
        SetText();
    }
}