using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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
    private List<QuestionType> _questionsNotAsked;

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
        _questionsNotAsked = new List<QuestionType>() {
            QuestionType.Greeting,
            QuestionType.Gossip,
            QuestionType.Location
        };
        
        ClearDialogueBox(true);
        container.gameObject.SetActive(true);
        OnConversationStart.Invoke();
        StartCoroutine(DisplayDialogue(QuestionType.Greeting));
    }

    private void EndConversation() {
        ClearDialogueBox(true);
        container.gameObject.SetActive(true);
        OnConversationEnd.Invoke();
    }
    
    private Dialogue GenerateResponseDialogue(QuestionType questionType) {
        // switch (questionType) {
        //     case QuestionType.Greeting:
        //         return Dialogue.FromClue(new ClueObject(ClueObject.ClueType.CommentClothing));
        //     default:
        //         return Dialogue.FromClue(new ClueObject(ClueObject.ClueType.CommentClothing));
        // }

        //Service.Player.TryGetClueFromCharacter(_character);
        ClueObject clue = _character.TryServeSpecificClueToPlayer(
            ClueObject.ClueType.SawInLocation,
            Service.Population.GetRandomCharacter());
        return Dialogue.FromClue(clue);
    }

    /// <summary>
    /// The DisplayDialogue coroutine displays the dialogue character by character in a scrolling manner.
    /// </summary>
    private IEnumerator DisplayDialogue(QuestionType questionType) {
        Dialogue dialogue = GenerateResponseDialogue(questionType);

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
        DisplayChoices();
        
        yield return null;
    }
    
    private void AskQuestion(QuestionType questionType) {
        ClearDialogueBox(true);
        StartCoroutine(DisplayDialogue(questionType));
    }

    protected void DisplayChoices() {
        _choiceButtonInstances = new List<GameObject>();

        if (_questionsNotAsked.Count == 0) {
            EndConversation();
            return;
        }
        
        foreach (QuestionType questionType in _questionsNotAsked) {
            Button choiceButtonInstance = Instantiate(choiceButton, choiceButtonHolder).GetComponent<Button>();
            _choiceButtonInstances.Add(choiceButtonInstance.gameObject);
            choiceButtonInstance.onClick.AddListener(() => {
                _questionsNotAsked.Remove(questionType);
                AskQuestion(questionType);
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