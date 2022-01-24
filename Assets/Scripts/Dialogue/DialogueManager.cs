using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour {
    public static DialogueManager Instance { get; private set; }
    
    public static UnityEvent OnConversationStart = new UnityEvent();
    public static UnityEvent OnConversationEnd = new UnityEvent();

    private Conversation _currentConversation;

    private int _dialogueIndex;
    private bool _linePlaying;

    [Header("Dialogue UI")] [SerializeField]
    private float timeBetweenCharacters = 0.1f;

    [SerializeField] private Transform choiceButtonHolder;
    [SerializeField] private TextMeshProUGUI speakerText;
    [SerializeField] private TextMeshProUGUI sentenceText;

    private List<GameObject> _choiceButtonInstances;
    [Header("Prefabs")] [SerializeField] private GameObject choiceButton;

    protected void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }
    }

    protected void Start() {
        Conversation testConversation = new Conversation();

        testConversation.Name = "Conversation A";
        testConversation.Dialogues = new List<Dialogue> {
            new Dialogue() {
                Speaker = "NPC",
                Sentence = new Sentence() {Text = "This is the first dialogue."},
                Choices = new List<Choice> {
                    new Choice() {Name = "Ask about B", DialogueIndex = 1}
                }
            },
            new Dialogue() {
                Speaker = "NPC",
                Sentence = new Sentence() {Text = "This is the second dialogue."}
            }
        };

        StartConversation(testConversation);
    }

    private void StartConversation(Conversation conversation) {
        _currentConversation = conversation;
        _dialogueIndex = 0;
        
        ClearDialogueBox(true);
        OnConversationStart.Invoke();
        StartCoroutine(DisplayDialogue());
    }

    private void JumpToDialogue(int index) {
        _dialogueIndex = index;
        ClearDialogueBox(true);
        StartCoroutine(DisplayDialogue());
    }

    /// <summary>
    /// The DisplayDialogue coroutine displays the dialogue character by character in a scrolling manner and sets all other
    /// relevant values.
    /// </summary>
    private IEnumerator DisplayDialogue() {
        Dialogue dialogue = _currentConversation.Dialogues[_dialogueIndex];

        dialogue.Invocations++;
        speakerText.text = dialogue.Speaker;
        sentenceText.text = dialogue.Sentence.Text;
        _linePlaying = true;

        int index = 0;

        while (index < dialogue.Sentence.Text.Length) {
            index++;
            sentenceText.maxVisibleCharacters++;
            yield return new WaitForSeconds(timeBetweenCharacters);
        }

        _linePlaying = false;
        DisplayChoices();
        
        yield return null;
    }

    /// <summary>
    /// Uses the Unity UI system and TextMeshPro to render choice buttons.
    /// </summary>
    protected void DisplayChoices() {
        if (_dialogueIndex >= _currentConversation.Dialogues.Count) return;
        
        Dialogue dialogue = _currentConversation.Dialogues[_dialogueIndex];
        
        _choiceButtonInstances = new List<GameObject>();
        
        if (dialogue.Choices == null || !dialogue.Choices.Any()) return;
        
        foreach (Choice choice in dialogue.Choices) {
            Button choiceButtonInstance = Instantiate(choiceButton, choiceButtonHolder).GetComponent<Button>();
            _choiceButtonInstances.Add(choiceButtonInstance.gameObject);
            choiceButtonInstance.onClick.AddListener(() => JumpToDialogue(choice.DialogueIndex));
        }
    }
    
    /// <summary>
    /// Clears all text and Images in the dialogue box.
    /// </summary>
    private void ClearDialogueBox(bool newConversation = false) {
        _linePlaying = false;
        speakerText.text = string.Empty;
        sentenceText.text = string.Empty;
        sentenceText.maxVisibleCharacters = 0;
        
        if (!newConversation) return;
        if (_choiceButtonInstances == null) return;
        
        foreach (GameObject buttonInstance in _choiceButtonInstances) {
            Destroy(buttonInstance);
        }
    }
}