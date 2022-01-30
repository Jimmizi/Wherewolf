using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EmoteRenderer : MonoBehaviour, IEmoteRenderer, IPointerEnterHandler, IPointerExitHandler {
    public Image Image;
    public CharacterEmoteRenderer CharacterEmoteRenderer;

    [SerializeField] private Emote _emote;

    public Emote Emote {
        get => _emote;
        set {
            _emote = value;
            Render(_emote.SubType);
        }
    }

    private void Awake() {
        if (CharacterEmoteRenderer != null) 
        {
            if (!CharacterEmoteRenderer.ValidCharacterRenderer)
            {
                CharacterEmoteRenderer.gameObject.SetActive(false);
            }
        }
    }

    private void Render(Emote.EmoteSubType type) {
        switch (type) {
            case Emote.EmoteSubType.CharacterHeadshot_1:
            case Emote.EmoteSubType.CharacterHeadshot_2:
            case Emote.EmoteSubType.CharacterHeadshot_3:
            case Emote.EmoteSubType.CharacterHeadshot_4:
            case Emote.EmoteSubType.CharacterHeadshot_5:
            case Emote.EmoteSubType.CharacterHeadshot_6:
            case Emote.EmoteSubType.CharacterHeadshot_7:
            case Emote.EmoteSubType.CharacterHeadshot_8:
            case Emote.EmoteSubType.CharacterHeadshot_9:
            case Emote.EmoteSubType.CharacterHeadshot_10:
            case Emote.EmoteSubType.CharacterHeadshot_11:
            case Emote.EmoteSubType.CharacterHeadshot_12:
            case Emote.EmoteSubType.CharacterHeadshot_13:
            case Emote.EmoteSubType.CharacterHeadshot_14:
            case Emote.EmoteSubType.CharacterHeadshot_15:
            case Emote.EmoteSubType.CharacterHeadshot_16:
            case Emote.EmoteSubType.CharacterHeadshot_17:
            case Emote.EmoteSubType.CharacterHeadshot_18:
            case Emote.EmoteSubType.CharacterHeadshot_19:
            case Emote.EmoteSubType.CharacterHeadshot_20: 
                {
                Image.gameObject.SetActive(false);
                if (CharacterEmoteRenderer != null && CharacterGenerator.Instance != null) 
                {
                    CharacterEmoteRenderer.gameObject.SetActive(true);
                    CharacterEmoteRenderer.ValidCharacterRenderer = true;
                    CharacterEmoteRenderer.SetAttributes(CharacterGenerator.Instance.AttributesForEmoteType(type));
                }

                break;
            }
            default: {
                Image.gameObject.SetActive(true);

                if (Service.InfoManager != null && Service.InfoManager.EmoteMapBySubType.ContainsKey(type)) {
                    if (Service.InfoManager.EmoteMapBySubType[type].EmoteImage) {
                        Image.sprite = Service.InfoManager.EmoteMapBySubType[type].EmoteImage;
                    }
                } else {
                    /* For debug purposes only */
                    Image.sprite = Service.EmoteLibrary.FindSprite(type);
                }

                if (CharacterEmoteRenderer != null) {
                    CharacterEmoteRenderer.gameObject.SetActive(false);
                }

                break;
            }
        }
    }

    public void Start() {
        //SetEmote(new Emote((Emote.EmoteSubType) Random.Range(0, 10)));
    }

    public void SetEmote(Emote emote) {
        _emote = emote;
        Render(emote.SubType);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        Service.TooltipManager.ShowEmoteTooltip(Emote, transform.position);
    }

    public void OnPointerExit(PointerEventData eventData) {
        Service.TooltipManager.HideActiveTooltip();
    }
}