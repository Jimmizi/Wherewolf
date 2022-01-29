using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour {
    public static TooltipManager Instance { get; private set; }
    
    public GameObject TooltipPrefab;
    public Canvas TooltipCanvas;
    public Vector2 ScreenPadding = Vector2.zero;

    private List<Tooltip> _tooltips;
    private Tooltip _activeTooltip;

    private Dictionary<Emote.EmoteSubType, Tooltip> _emoteTooltips;
    private static readonly Vector3 offset = new Vector3(0f, 40f, 0f);

    protected void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
            Service.TooltipManager = this;
        }

        _tooltips = new List<Tooltip>();
        _emoteTooltips = new Dictionary<Emote.EmoteSubType, Tooltip>();
    }

    public Tooltip NewTooltip() {
        if (TooltipPrefab == null || TooltipCanvas == null) {
            return null;
        }
        
        GameObject gameObject = Instantiate(TooltipPrefab, TooltipCanvas.transform);
        Tooltip tooltip = gameObject.GetComponent<Tooltip>();
        _tooltips.Add(tooltip);
        return tooltip;
    }

    public void ShowTooltip(Tooltip tooltip, Vector3 position) {
        tooltip.gameObject.SetActive(true);
        tooltip.transform.position = ScreenBoundPosition(position + offset, tooltip.RectTransform);
        HideActiveTooltip();
        _activeTooltip = tooltip;
    }
    
    public void HideTooltip(Tooltip tooltip) {
        tooltip.gameObject.SetActive(false);
    }
    
    public void ShowEmoteTooltip(Emote emote, Vector3 position) {
        Tooltip tooltip;
        
        if (!_emoteTooltips.ContainsKey(emote.SubType)) {
            tooltip = NewTooltip();
            tooltip.Title = emote.HasDiscovered ? emote.Name : "???";
            tooltip.Description = emote.Description;
            _emoteTooltips[emote.SubType] = tooltip;
            
            /* Force rebuild layout on tooltip instantiation. */
            tooltip.transform.position = position + offset;
            LayoutRebuilder.ForceRebuildLayoutImmediate(tooltip.RectTransform);
        } else {
            tooltip = _emoteTooltips[emote.SubType];

            tooltip.Title = emote.HasDiscovered ? emote.Name : "???";
        }
        
        tooltip.gameObject.SetActive(true);
        tooltip.transform.position = ScreenBoundPosition(position + offset, tooltip.RectTransform);
        
        HideActiveTooltip();
        _activeTooltip = tooltip;
    }

    public void HideActiveTooltip() {
        if (_activeTooltip != null) {
            _activeTooltip.gameObject.SetActive(false);
            _activeTooltip = null;
        }
    }
    
    private Vector3 ScreenBoundPosition(Vector3 position, RectTransform rectTransform) {
        rectTransform.position = position;
        float halfWidth = LayoutUtility.GetPreferredWidth(rectTransform) * 0.5f * TooltipCanvas.scaleFactor;
        float halfHeight = LayoutUtility.GetFlexibleHeight(rectTransform) * 0.5f * TooltipCanvas.scaleFactor;
        Vector3 positionOffset = Vector3.zero;
        
        if (position.x - halfWidth < ScreenPadding.x) {
            positionOffset.x += ScreenPadding.x - (position.x - halfWidth);
        }

        if (position.x + halfWidth > Screen.width - ScreenPadding.x) {
            positionOffset.x -= position.x + halfWidth - (Screen.width - ScreenPadding.x);
        }

        if (position.y - halfHeight < ScreenPadding.y) {
            positionOffset.y += ScreenPadding.y - (position.y - halfHeight);
        }

        if (position.y + halfHeight > Screen.height - ScreenPadding.y) {
            positionOffset.y -= position.y + halfHeight - (Screen.height - ScreenPadding.y);
        }

        return position + positionOffset;
    }
}
