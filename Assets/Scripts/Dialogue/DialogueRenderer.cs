using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using DialogueActionEmoteMap = System.Collections.Generic.Dictionary<DialogueActionType, Emote.EmoteSubType>;

public class DialogueRenderer : MonoBehaviour {
    
    public static UnityEvent OnConversationStart = new UnityEvent();
    public static UnityEvent OnConversationEnd = new UnityEvent();

    [Header("Dialogue UI")] [SerializeField]
    private float timeBetweenCharacters = 0.1f;

    public Transform container;
    public Transform choiceButtonHolder;
    public TextMeshProUGUI speakerText;
    public EmoteTextRenderer sentenceText;
    public DropDownList charactersDropdown;
    public GameObject corkboardOpenerGo;
    
    private bool _linePlaying;
    
    [Header("Prefabs")]
    public GameObject choiceButton;
    
    private List<GameObject> _choiceButtonInstances;

    private Character _character;
    private Dialogue _currentDialogue;

    IEnumerator ExecuteAfterTime(float time) {
        yield return new WaitForSeconds(time);
        // Code to execute after the delay
        StartConversation(Service.Population.GetRandomCharacter());
    }

    private void Start() {
        container.gameObject.SetActive(false);
        //StartConversation(Service.Population.GetRandomCharacter());
        //StartCoroutine(ExecuteAfterTime(10f));
    }

    public void StartConversation(Character character) {
        _character = character;
        ClearDialogueBox(true);
        container.gameObject.SetActive(true);
        OnConversationStart.Invoke();
        StartCoroutine(DisplayDialogue(DialogueActionType.IssueGreeting, null));
        PopulateCharactersDropdown();
        DisplayChoices();
        corkboardOpenerGo?.SetActive(false);
    }

    private void EndConversation() {
        ClearDialogueBox(true);
        container.gameObject.SetActive(false);
        OnConversationEnd.Invoke();
        corkboardOpenerGo?.SetActive(true);
    }
    
    private Dialogue GenerateResponseDialogue(DialogueActionType dialogueActionType, Character relatedCharacter) {
        ClueObject clue;

        _character?.TriggerSpeechSound();

        switch (dialogueActionType) {
            case DialogueActionType.IssueGreeting:
            case DialogueActionType.Gossip:
                clue = _character.TryServeSpecificClueToPlayer(ClueObject.ClueType.CommentGossip, relatedCharacter);
                break;
            case DialogueActionType.QuerySawAtLocation:
                clue = _character.TryServeSpecificClueToPlayer(ClueObject.ClueType.SawInLocation, relatedCharacter);
                break;
            case DialogueActionType.QuerySawAtWork:
                clue = _character.TryServeSpecificClueToPlayer(ClueObject.ClueType.SawAtWork, relatedCharacter);
                break;
            case DialogueActionType.QuerySawPassing:
                clue = _character.TryServeSpecificClueToPlayer(ClueObject.ClueType.SawPassingBy, relatedCharacter);
                break;
            default:
                clue = _character.TryServeSpecificClueToPlayer(ClueObject.ClueType.CommentGossip, relatedCharacter);
                break;
        }

        if (dialogueActionType != DialogueActionType.IssueFarewell)
        {
            // Discover names when receiving clues about them.
            clue.RelatesToCharacter?.SetNameDiscovered();
            Service.Player.AddClueGiven(clue, dialogueActionType != DialogueActionType.IssueGreeting);
        }

        return Dialogue.FromClue(clue);
    }

    private void PopulateCharactersDropdown() 
    {
        if (charactersDropdown == null) return;

        charactersDropdown.ResetItems();

        // Add a dummy character so we can always select to get a random character clue
        Character dummyCharacter = new Character()
        {
            Name = "Random"
        };

        charactersDropdown.AddItem(new DropDownListItem<Character>(data: dummyCharacter));

        foreach (Character character in Service.Population.ActiveCharacters) 
        {
            // Can't ask about self
            if(character == _character)
            {
                continue;
            }

            // Only add discovered characters (otherwise we wouldn't be able to know to to ask about)
            if (!Service.InfoManager.EmoteMapBySubType[character.GetHeadshotEmoteSubType()].HasDiscovered)
            {
                continue;
            }

            charactersDropdown.AddItem(new DropDownListItem<Character>(data: character));
        }
    }
    
    /// <summary>
    /// The DisplayDialogue coroutine displays the dialogue character by character in a scrolling manner.
    /// </summary>
    private IEnumerator DisplayDialogue(DialogueActionType dialogueActionType, Character relatedCharacter) {
        Dialogue dialogue = GenerateResponseDialogue(dialogueActionType, relatedCharacter);

        if (dialogue.Speaker != null) {
            speakerText.text = dialogue.Speaker.Name;
        } else {
            speakerText.text = "Unknown";
        }

        sentenceText.Render(dialogue.Sentence);
        _linePlaying = true;

        int index = 0;

        while (index < dialogue.Sentence.Count) {
            index++;
            sentenceText.MaxVisibleCharacters++;
            yield return new WaitForSeconds(timeBetweenCharacters);
        }

        _linePlaying = false;
        //DisplayChoices();
        
        yield return null;
    }
    
    private void DoAction(DialogueActionType dialogueActionType, Character relatedCharacter) {
        ClearDialogueBox(true);

        switch (dialogueActionType) {
            case DialogueActionType.IssueFarewell:

                // Fixes the tooltip staying around when leaving conversation.
                TooltipManager.Instance?.HideActiveTooltip();

                EndConversation();
                break;
            default:
                StartCoroutine(DisplayDialogue(dialogueActionType, relatedCharacter));
                break;
        }
    }

    const Emote.EmoteSubType eTypeForFarewell = Emote.EmoteSubType.Specific_Handshake;

    private static readonly DialogueActionEmoteMap _dialogueActions = new DialogueActionEmoteMap() {
        //{DialogueActionType.IssueGreeting, Emote.EmoteSubType.Specific_Approves},
        {DialogueActionType.Gossip, Emote.EmoteSubType.Gossip_Gossip},
        {DialogueActionType.QuerySawAtLocation, Emote.EmoteSubType.Specific_Eyes},
        {DialogueActionType.QuerySawPassing, Emote.EmoteSubType.Specific_Footsteps},
        {DialogueActionType.QuerySawAtWork, Emote.EmoteSubType.Occupation_Bank},
        {DialogueActionType.IssueFarewell, eTypeForFarewell},
    };
    
    protected void DisplayChoices() {
        if (_choiceButtonInstances == null) 
        {
            _choiceButtonInstances = new List<GameObject>();
            
            foreach (KeyValuePair<DialogueActionType, Emote.EmoteSubType> dialogueAction in _dialogueActions) 
            {
                GameObject choiceButtonInstance = Instantiate(choiceButton, choiceButtonHolder);
                Button button = choiceButtonInstance.GetComponent<Button>();
                EmoteRenderer emoteRenderer = choiceButtonInstance.GetComponentInChildren<EmoteRenderer>();

                _choiceButtonInstances.Add(choiceButtonInstance.gameObject);
                emoteRenderer.Emote = Service.InfoManager.EmoteMapBySubType[dialogueAction.Value];
                button.onClick.AddListener(() => 
                {
                    Character characterToQueryAbout = null;
                    DropDownListItem<Character> itemToUse = (DropDownListItem<Character>)charactersDropdown.SelectedItem;

                    if(itemToUse != null && itemToUse.Data.Name != "Random")
                    {
                        characterToQueryAbout = itemToUse.Data;
                    }

                    Debug.LogFormat("The player has chosen dialog action {0}, to find out about {1}", 
                        dialogueAction.Key, characterToQueryAbout != null ? characterToQueryAbout.Name : "Random");

                    DoAction(dialogueAction.Key, characterToQueryAbout);
                    HideChoices(true);
                });
            }
        }

        if (_character?.IsAlive ?? true)
        {
            ShowAllButFarewell();
        }
        else
        {
            HideChoices(true);
        }
    }
    
    void ShowAllButFarewell()
    {
        foreach (GameObject choiceButtonInstance in _choiceButtonInstances)
        {
            EmoteRenderer emoteRenderer = choiceButtonInstance.GetComponentInChildren<EmoteRenderer>();
            if (emoteRenderer && emoteRenderer.Emote.SubType == eTypeForFarewell)
            {
                choiceButtonInstance.SetActive(false);
            }
            else
            {
                choiceButtonInstance.SetActive(true);
            }
        }

    }

    protected void HideChoices(bool bKeepIssueFarewell = false) {
        foreach (GameObject choiceButtonInstance in _choiceButtonInstances) 
        {
            if(bKeepIssueFarewell)
            {
                EmoteRenderer emoteRenderer = choiceButtonInstance.GetComponentInChildren<EmoteRenderer>();
                if(emoteRenderer && emoteRenderer.Emote.SubType == eTypeForFarewell)
                {
                    choiceButtonInstance.SetActive(true);
                    continue;
                }
            }

            choiceButtonInstance.SetActive(false);
        }
    }
    
    private void ClearDialogueBox(bool newConversation = false) {
        _linePlaying = false;
        speakerText.text = String.Empty;
        sentenceText.Clear();
        sentenceText.MaxVisibleCharacters = 0;
        
        if (!newConversation) return;
        if (_choiceButtonInstances == null) return;
        
        //HideChoices();
        // foreach (GameObject buttonInstance in _choiceButtonInstances) {
        //     Destroy(buttonInstance);
        // }
    }
}