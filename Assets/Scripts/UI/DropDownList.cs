///Credit perchik
///Sourced from - http://forum.unity3d.com/threads/receive-onclick-event-and-pass-it-on-to-lower-ui-elements.293642/


using System.Collections.Generic;

namespace UnityEngine.UI.Extensions {
    /// <summary>
    ///  Extension to the UI class which creates a dropdown list 
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("UI/Extensions/Dropdown List")]
    public class DropDownList : MonoBehaviour {
        public Color disabledTextColor;
        public IDropDownListItem SelectedItem { get; private set; } //outside world gets to get this, not set it

        public bool OverrideHighlighted = true;

        //private bool isInitialized = false;
        private bool _isPanelActive = false;
        private bool _hasDrawnOnce = false;

        private IDropDownListItemRenderer _mainButton;

        private RectTransform _rectTransform;

        private RectTransform _overlayRT;
        private RectTransform _scrollPanelRT;
        private RectTransform _scrollBarRT;
        private RectTransform _slidingAreaRT;
        private RectTransform _scrollHandleRT;
        private RectTransform _itemsPanelRT;
        private Canvas _canvas;
        private RectTransform _canvasRT;

        private ScrollRect _scrollRect;

        [SerializeField] private List<IDropDownListItem> Items;

        private List<IDropDownListItemRenderer> _panelItems;

        public GameObject ItemTemplate;

        [SerializeField] private float _scrollBarWidth = 20.0f;

        public float ScrollBarWidth {
            get { return _scrollBarWidth; }
            set {
                _scrollBarWidth = value;
                RedrawPanel();
            }
        }

        //    private int scrollOffset; //offset of the selected item
        private int _selectedIndex = -1;

        [SerializeField] private int _itemsToDisplay;

        public int ItemsToDisplay {
            get { return _itemsToDisplay; }
            set {
                _itemsToDisplay = value;
                RedrawPanel();
            }
        }

        public bool SelectFirstItemOnStart = false;

        [SerializeField] private bool _displayPanelAbove = false;

        [System.Serializable]
        public class SelectionChangedEvent : UnityEngine.Events.UnityEvent<int> {
        }

        // fires when item is changed;
        public SelectionChangedEvent OnSelectionChanged;

        public bool Initialised = false;

        public void Start() {
            //if (!Initialised)
                Initialize();
            if (SelectFirstItemOnStart && Items.Count > 0) {
                ToggleDropdownPanel(false);
                OnItemClicked(0);
            }
            
            RedrawPanel();
        }

        private bool bAddedDefaultItems = false;
        private void Update()
        {
            if (!bAddedDefaultItems && Service.Population.CharacterCreationDone)
            {
                // Give the player some names to work with from the start

                bAddedDefaultItems = true;
                //for(int i = 0; i < 3; ++i)
                //{
                //    Character c = Service.Population.GetRandomCharacter();
                //    c.SetNameDiscovered();
                //    AddItem(new DropDownListItem<Character>(data: c));
                //}
            }
        }

        public void DoInitialise()
        {
            Initialize();
            if (SelectFirstItemOnStart && Items.Count > 0)
            {
                ToggleDropdownPanel(false);
                OnItemClicked(0);
            }

            RedrawPanel();
        }

        private bool Initialize() {

            if (Initialised)
            {
                return true;
            }
            bool success = true;
            try {
                Items = new List<IDropDownListItem>();
                
                _rectTransform = GetComponent<RectTransform>();
                //_mainButton = new IDropDownListItemRenderer(_rectTransform.Find("MainButton").gameObject);
                _mainButton = _rectTransform.Find("MainButton").gameObject.GetComponent<IDropDownListItemRenderer>();

                _overlayRT = _rectTransform.Find("Overlay").GetComponent<RectTransform>();
                _overlayRT.gameObject.SetActive(false);


                _scrollPanelRT = _overlayRT.Find("ScrollPanel").GetComponent<RectTransform>();
                _scrollBarRT = _scrollPanelRT.Find("Scrollbar").GetComponent<RectTransform>();
                _slidingAreaRT = _scrollBarRT.Find("SlidingArea").GetComponent<RectTransform>();
                _scrollHandleRT = _slidingAreaRT.Find("Handle").GetComponent<RectTransform>();
                _itemsPanelRT = _scrollPanelRT.Find("Viewport").Find("Content").GetComponent<RectTransform>();
                //itemPanelLayout = itemsPanelRT.gameObject.GetComponent<LayoutGroup>();

                _canvas = GetComponentInParent<Canvas>();
                _canvasRT = _canvas.GetComponent<RectTransform>();

                _scrollRect = _scrollPanelRT.GetComponent<ScrollRect>();
                _scrollRect.scrollSensitivity = _rectTransform.sizeDelta.y / 2;
                _scrollRect.movementType = ScrollRect.MovementType.Clamped;
                _scrollRect.content = _itemsPanelRT;


                //ItemTemplate = _rectTransform.Find("ItemTemplate").gameObject;
                //ItemTemplate.SetActive(false);
            } catch (System.NullReferenceException ex) {
                Debug.LogException(ex);
                Debug.LogError(
                    "Something is setup incorrectly with the DropDownList component causing a Null Reference Exception");
                success = false;
            }

            Initialised = success;
            _panelItems = new List<IDropDownListItemRenderer>();
            _mainButton.btn.onClick.AddListener(() => {
                Debug.Log("Dropdown toggle");
                ToggleDropdownPanel(true);
            });

            RebuildPanel();
            RedrawPanel();
            return success;
        }

        // currently just using items in the list instead of being able to add to it.
        /// <summary>
        /// Rebuilds the list from a new collection.
        /// </summary>
        /// <remarks>
        /// NOTE, this will clear all existing items
        /// </remarks>
        /// <param name="list"></param>
        public void RefreshItems(params object[] list) {
            Items.Clear();
            List<IDropDownListItem> ddItems = new List<IDropDownListItem>();
            foreach (var obj in list) {
                if (obj is IDropDownListItem) {
                    ddItems.Add((IDropDownListItem) obj);
                } /*else if (obj is string) {
                    ddItems.Add(new IDropDownListItem(caption: (string) obj));
                } else if (obj is Sprite) {
                    ddItems.Add(new IDropDownListItem(image: (Sprite) obj));
                } else {
                    throw new System.Exception("Only ComboBoxItems, Strings, and Sprite types are allowed");
                }*/
            }

            Items.AddRange(ddItems);
            RebuildPanel();
        }

        /// <summary>
        /// Adds an additional item to the drop down list (recommended)
        /// </summary>
        /// <param name="item">Item of type IDropDownListItem</param>
        public void AddItem(IDropDownListItem item) {
            Items.Add(item);
            RebuildPanel();
        }

        /// <summary>
        /// Removes an item from the drop down list (recommended)
        /// </summary>
        /// <param name="item">Item of type IDropDownListItem</param>
        public void RemoveItem(IDropDownListItem item) {
            Items.Remove(item);
            RebuildPanel();
        }
        
        public void ResetItems() {
            Items?.Clear();
            RebuildPanel();
        }

        /// <summary>
        /// Rebuilds the contents of the panel in response to items being added.
        /// </summary>
        private void RebuildPanel() {
            if (Items == null || Items.Count == 0) return;

            int indx = _panelItems.Count;
            while (_panelItems.Count < Items.Count) {
                GameObject newItem = Instantiate(ItemTemplate) as GameObject;
                newItem.name = "Item " + indx;
                newItem.transform.SetParent(_itemsPanelRT, false);

                _panelItems.Add(newItem.GetComponent<IDropDownListItemRenderer>());
                indx++;
            }

            for (int i = 0; i < _panelItems.Count; i++) {
                if (i < Items.Count) {
                    IDropDownListItem item = Items[i];
                    _panelItems[i].Render(item);
                    
                    int ii = i; //have to copy the variable for use in anonymous function
                    
                    _panelItems[i].btn.onClick.RemoveAllListeners();
                    _panelItems[i].btn.onClick.AddListener(() => {
                        OnItemClicked(ii);
                        if (item.OnSelect != null) item.OnSelect();
                    });
                }

                _panelItems[i].gameobject
                    .SetActive(i < Items.Count); // if we have more thanks in the panel than Items in the list hide them
            }
        }

        private void OnItemClicked(int indx) {
            //Debug.Log("item " + indx + " clicked");
            if (indx != _selectedIndex && OnSelectionChanged != null) OnSelectionChanged.Invoke(indx);

            _selectedIndex = indx;
            ToggleDropdownPanel(true);
            UpdateSelected();
        }

        private void UpdateSelected() {
            SelectedItem = (_selectedIndex > -1 && _selectedIndex < Items.Count) ? Items[_selectedIndex] : null;
            if (SelectedItem == null) return;

            _mainButton.Render(SelectedItem);

            //update selected index color
            // if (OverrideHighlighted) {
            //     for (int i = 0; i < _itemsPanelRT.childCount; i++) {
            //         _panelItems[i].btnImg.color = (_selectedIndex == i)
            //             ? _mainButton.btn.colors.highlightedColor
            //             : new Color(0, 0, 0, 0);
            //     }
            // }
        }


        private void RedrawPanel() {
            float scrollbarWidth =
                Items.Count > ItemsToDisplay ? _scrollBarWidth : 0f; //hide the scrollbar if there's not enough items

            if (!_hasDrawnOnce) { // || _rectTransform.sizeDelta != _mainButton.rectTransform.sizeDelta) {
                _hasDrawnOnce = true;
                // _mainButton.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                //     _rectTransform.sizeDelta.x);
                // _mainButton.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
                //     _rectTransform.sizeDelta.y);
                //_mainButton.txt.rectTransform.offsetMax = new Vector2(4, 0);

                _scrollPanelRT.SetParent(transform, true); //break the scroll panel from the overlay
                _scrollPanelRT.anchoredPosition = _displayPanelAbove
                    ? new Vector2(0, _rectTransform.sizeDelta.y * ItemsToDisplay - 1)
                    : new Vector2(0, -_rectTransform.sizeDelta.y);

                //make the overlay fill the screen
                _overlayRT.SetParent(_canvas.transform, false); //attach it to top level object
                _overlayRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _canvasRT.sizeDelta.x);
                _overlayRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _canvasRT.sizeDelta.y);

                _overlayRT.SetParent(transform, true); //reattach to this object
                _scrollPanelRT.SetParent(_overlayRT, true); //reattach the scrollpanel to the overlay            
            }

            if (Items.Count < 1) return;

            float dropdownHeight = _rectTransform.sizeDelta.y * Mathf.Min(_itemsToDisplay, Items.Count);

            _scrollPanelRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, dropdownHeight);
            _scrollPanelRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _rectTransform.sizeDelta.x);

            // _itemsPanelRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
            //     _scrollPanelRT.sizeDelta.x - scrollbarWidth - 5);
            // _itemsPanelRT.anchoredPosition = new Vector2(5, 0);

            _scrollBarRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scrollbarWidth);
            _scrollBarRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, dropdownHeight);
            if (scrollbarWidth == 0) _scrollHandleRT.gameObject.SetActive(false);
            else _scrollHandleRT.gameObject.SetActive(true);

            _slidingAreaRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
            _slidingAreaRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
                dropdownHeight - _scrollBarRT.sizeDelta.x);
        }

        /// <summary>
        /// Toggle the drop down list
        /// </summary>
        /// <param name="directClick"> whether an item was directly clicked on</param>
        public void ToggleDropdownPanel(bool directClick) {
            _overlayRT.transform.localScale = new Vector3(1, 1, 1);
            _scrollBarRT.transform.localScale = new Vector3(1, 1, 1);
            _isPanelActive = !_isPanelActive;
            _overlayRT.gameObject.SetActive(_isPanelActive);
            if (_isPanelActive) {
                transform.SetAsLastSibling();
            } else if (directClick) {
                // scrollOffset = Mathf.RoundToInt(itemsPanelRT.anchoredPosition.y / _rectTransform.sizeDelta.y); 
            }
        }
    }
}