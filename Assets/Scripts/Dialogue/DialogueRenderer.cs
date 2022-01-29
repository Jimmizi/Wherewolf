using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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
        StartCoroutine(ExecuteAfterTime(10f));
    }

    public void StartConversation(Character character) {
        _character = character;
        ClearDialogueBox(true);
        container.gameObject.SetActive(true);
        OnConversationStart.Invoke();
        StartCoroutine(DisplayDialogue(DialogueActionType.IssueGreeting, null));
        DisplayChoices();
    }

    private void EndConversation() {
        ClearDialogueBox(true);
        container.gameObject.SetActive(true);
        OnConversationEnd.Invoke();
    }
    
    private Dialogue GenerateResponseDialogue(DialogueActionType dialogueActionType, Character relatedCharacter) {
        ClueObject clue;
        
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

        return Dialogue.FromClue(clue);
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
        StartCoroutine(DisplayDialogue(dialogueActionType, relatedCharacter));
    }

    private static readonly DialogueActionEmoteMap _dialogueActions = new DialogueActionEmoteMap() {
        //{DialogueActionType.IssueGreeting, Emote.EmoteSubType.Specific_Approves},
        {DialogueActionType.Gossip, Emote.EmoteSubType.Gossip_RelationshipFight},
        {DialogueActionType.QuerySawAtLocation, Emote.EmoteSubType.Specific_Footsteps},
        {DialogueActionType.QuerySawPassing, Emote.EmoteSubType.Specific_Eyes},
        {DialogueActionType.QuerySawAtWork, Emote.EmoteSubType.Occupation_Bank},
        {DialogueActionType.IssueFarewell, Emote.EmoteSubType.Specific_Disapproves},
    };
    
    protected void DisplayChoices() {
        _choiceButtonInstances = new List<GameObject>();

        foreach (KeyValuePair<DialogueActionType, Emote.EmoteSubType> dialogueAction in _dialogueActions) {
            GameObject choiceButtonInstance = Instantiate(choiceButton, choiceButtonHolder);
            Button button = choiceButtonInstance.GetComponent<Button>();
            EmoteRenderer emoteRenderer = choiceButtonInstance.GetComponentInChildren<EmoteRenderer>();
            
            _choiceButtonInstances.Add(choiceButtonInstance.gameObject);
            emoteRenderer.Emote = new Emote(dialogueAction.Value);
            button.onClick.AddListener(() => {
                DoAction(dialogueAction.Key, Service.Population.GetRandomCharacter());
            });
        }
    }
    
    private void ClearDialogueBox(bool newConversation = false) {
        _linePlaying = false;
        speakerText.text = String.Empty;
        sentenceText.Clear();
        sentenceText.MaxVisibleCharacters = 0;
        
        if (!newConversation) return;
        if (_choiceButtonInstances == null) return;
        
        foreach (GameObject buttonInstance in _choiceButtonInstances) {
            Destroy(buttonInstance);
        }
    }
}