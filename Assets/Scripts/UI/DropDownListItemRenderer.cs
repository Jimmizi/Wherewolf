///Credit perchik
///Sourced from - http://forum.unity3d.com/threads/receive-onclick-event-and-pass-it-on-to-lower-ui-elements.293642/

namespace UnityEngine.UI.Extensions {
    public interface IDropDownListItemRenderer {
        public RectTransform rectTransform { get; }
        public Button btn { get; }
        public GameObject gameobject { get; }

        public void Render(IDropDownListItem item);
    }
    
    [RequireComponent(typeof(RectTransform), typeof(Button))]
    public class DropDownListItemRenderer : MonoBehaviour, IDropDownListItemRenderer {
        // public Text txt;
        // public Image btnImg;
        // public Image img;

        public RectTransform RectTransform;
        public Button Button;

        // public DropDownListItemRenderer(GameObject btnObj) {
        //     gameobject = btnObj;
        //     rectTransform = btnObj.GetComponent<RectTransform>();
        //     btnImg = btnObj.GetComponent<Image>();
        //     btn = btnObj.GetComponent<Button>();
        //     txt = rectTransform.Find("Text").GetComponent<Text>();
        //     img = rectTransform.Find("Image").GetComponent<Image>();
        // }

        public RectTransform rectTransform { get => RectTransform; }
        public Button btn { get => Button; }
        public GameObject gameobject { get => gameObject; }
        
        public void Render(IDropDownListItem item) {
            //throw new System.NotImplementedException();
        }
    }
}