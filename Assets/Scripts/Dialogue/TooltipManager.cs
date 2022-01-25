using TMPro;
using UnityEngine;

public class TooltipManager : MonoBehaviour {
    public static TooltipManager Instance { get; private set; }
    
    public GameObject TooltipPrefab;
    public Canvas TooltipCanvas;

    protected void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }
    }

    public Tooltip NewTooltip() {
        if (TooltipPrefab == null || TooltipCanvas == null) {
            return null;
        }
        
        GameObject gameObject = Instantiate(TooltipPrefab, TooltipCanvas.transform);
        Tooltip tooltip = gameObject.GetComponent<Tooltip>();
        return tooltip;
    }

    public void ShowTooltip(Tooltip tooltip, Vector3 position) {
        tooltip.gameObject.SetActive(true);
        tooltip.transform.position = position + new Vector3(0f, 40f, 0f);
    }

    public void HideTooltip(Tooltip tooltip) {
        tooltip.gameObject.SetActive(false);
    }
}