using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TMP_DecoratedText : MonoBehaviour {
    //private Camera _camera;
    private TextMeshProUGUI _textMesh;
    private Tooltip _tooltip;
    private Transform _transform;

    private void Awake() {
        /* No camera required, since canvas is screen space overlay. */
        //_camera = Camera.main;
        _textMesh = GetComponent<TextMeshProUGUI>();
        _transform = gameObject.GetComponent<Transform>();
    }

    private void Start() {
        _tooltip = Service.TooltipManager.NewTooltip();
    }

    private Vector3 CalcLinkCenterPosition(int linkIndex) {
        Vector3 bottomLeft = Vector3.zero;
        Vector3 topRight = Vector3.zero;

        float maxAscender = -Mathf.Infinity;
        float minDescender = Mathf.Infinity;

        TMP_TextInfo textInfo = _textMesh.textInfo;
        TMP_LinkInfo linkInfo = textInfo.linkInfo[linkIndex];
        
        TMP_CharacterInfo firstCharInfo = textInfo.characterInfo[linkInfo.linkTextfirstCharacterIndex];
        TMP_CharacterInfo lastCharInfo = textInfo.characterInfo[linkInfo.linkTextfirstCharacterIndex + linkInfo.linkTextLength];

        minDescender = Mathf.Min(minDescender, firstCharInfo.descender);
        maxAscender = Mathf.Max(maxAscender, lastCharInfo.ascender);

        bottomLeft = new Vector3(firstCharInfo.bottomLeft.x, firstCharInfo.descender, 0);
        topRight = new Vector3(lastCharInfo.topRight.x, lastCharInfo.ascender, 0);

        bottomLeft = _transform.TransformPoint(new Vector3(bottomLeft.x, minDescender, 0));
        topRight = _transform.TransformPoint(new Vector3(topRight.x, maxAscender, 0));

        float width = topRight.x - bottomLeft.x;
        float height = topRight.y - bottomLeft.y;

        Vector3 centerPosition = bottomLeft;
        centerPosition.x += width / 2;
        centerPosition.y += height / 2;

        return centerPosition;
    }

    private void Update() {
        if (_tooltip == null) return;

        int linkIndex = TMP_TextUtilities.FindIntersectingLink(_textMesh, Input.mousePosition, null);
        if (linkIndex != -1) {
            //TMP_LinkInfo linkInfo = _textMesh.textInfo.linkInfo[linkIndex];
            Service.TooltipManager.ShowTooltip(_tooltip, CalcLinkCenterPosition(linkIndex));
        } else {
            Service.TooltipManager.HideTooltip(_tooltip);
        }
    }
}