///Credit perchik
///Sourced from - http://forum.unity3d.com/threads/receive-onclick-event-and-pass-it-on-to-lower-ui-elements.293642/

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

[RequireComponent(typeof(RectTransform), typeof(Button))]
public class DropDownCharacterListItemRenderer : MonoBehaviour, IDropDownListItemRenderer {
    public RectTransform RectTransform;
    public CharacterEmoteRenderer EmoteRenderer;
    public TextMeshProUGUI Name;
    public Button Button;


    public RectTransform rectTransform {
        get => RectTransform;
    }

    public Button btn {
        get => Button;
    }

    public GameObject gameobject {
        get => gameObject;
    }

    public void Render(IDropDownListItem item) {
        if (item is DropDownListItem<Character> characterItem) {
            if (characterItem.Data != null) {
                EmoteRenderer.Render(characterItem.Data);
                Name.text = characterItem.Data.Name;
            }
        }
    }
}