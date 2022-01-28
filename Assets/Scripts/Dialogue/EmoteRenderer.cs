using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EmoteRenderer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public Image Image;
    
    [SerializeField] private Emote _emote;

    public Emote Emote {
        get => _emote;
        set {
            _emote = value;
            SetSprite(_emote.SubType);
        }
    }

    private void SetSprite(Emote.EmoteSubType type) {
        Image.sprite = EmoteLibrary.Instance.FindSprite(type);
    }

    public void Start() {
        SetEmote(new Emote((Emote.EmoteSubType) Random.Range(0, 10)));
    }

    public void SetEmote(Emote emote) {
        _emote = emote;
        SetSprite(emote.SubType);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        TooltipManager.Instance.ShowEmoteTooltip(Emote, transform.position);
    }

    public void OnPointerExit(PointerEventData eventData) {
        TooltipManager.Instance.HideActiveTooltip();
    }
}