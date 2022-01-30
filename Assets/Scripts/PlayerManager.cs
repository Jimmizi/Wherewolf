using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public List<ClueObject> CollectedClues = new List<ClueObject>();
    public SortedDictionary<int, Dictionary<WerewolfGame.TOD, List<ClueObject>>> SortedByDayAndPhaseClues = new SortedDictionary<int, Dictionary<WerewolfGame.TOD, List<ClueObject>>>();

    public int NumberOfCluesAboutWerewolf = 0;

#if UNITY_EDITOR
    public bool DebugGrabSomeClues = false;
#endif

    [SerializeField]
    public int NumberOfTimesCanTalkToCharactersPerPhase = 3;

    [SerializeField]
    public CanvasGroup ActionsCanvasGroup;

    [SerializeField]
    public List<GameObject> DayTimeActionObjects;

    [SerializeField]
    public List<GameObject> NightTimeActionObjects;

    [SerializeField]
    public Color ActionSpentColor;

    [SerializeField]
    public Color ActionAvailableColor;

    // True when the player has a characters dialogue box up
    public bool IsTalkingToCharacter = false;

    public bool PerformingFade => isFading;
    private bool isFading = false;

    public int NumberOfActionsLeft = 3;

    public void AddClueGiven(ClueObject givenClue, bool bSpendAction = true)
    {
        // Should NEVER be null but handle it just in case
        if (givenClue == null)
        {
            Debug.LogError("Something went wrong when trying to get a clue");
            // If something went really wrong, give the action back to the player
            if (bSpendAction)
            {
                ++NumberOfActionsLeft;
            }
            VisuallyUpdateAmountOfUsedActionPoints();
            return;
        }

        if (bSpendAction && HasActionPointsLeft())
        {
            --NumberOfActionsLeft;
            VisuallyUpdateAmountOfUsedActionPoints();
        }

        if (givenClue.RelatesToCharacter?.IsWerewolf ?? false)
        {
            NumberOfCluesAboutWerewolf++;
        }

        // Don't add in responses of "didn't have something to tell you" to memos, case files, or collected clues in general
        if (!givenClue.DidntHaveClueAboutPerson)
        {
            MemoFactory.instance.CreateNew(givenClue.GivenByCharacter.Name, givenClue.Emotes, true);

            // DON'T add clues given by ghosts, otherwise it shows on someones record that they're the werewolf
            if (givenClue.Type != ClueObject.ClueType.VisualFromGhost)
            {
                Service.CaseFile.AddClueToFile(givenClue);
            }

            CollectedClues.Add(givenClue);

            if (!SortedByDayAndPhaseClues.ContainsKey(Service.Game.CurrentDay))
            {
                SortedByDayAndPhaseClues.Add(Service.Game.CurrentDay, new Dictionary<WerewolfGame.TOD, List<ClueObject>>());
                SortedByDayAndPhaseClues[Service.Game.CurrentDay].Add(WerewolfGame.TOD.Day, new List<ClueObject>());
                SortedByDayAndPhaseClues[Service.Game.CurrentDay].Add(WerewolfGame.TOD.Night, new List<ClueObject>());
            }

            SortedByDayAndPhaseClues[Service.Game.CurrentDay][Service.Game.CurrentTimeOfDay].Add(givenClue);
        }
    }

    public bool CanGetClueFromCharacter(Character c)
    {
        if(Service.Game.CurrentState != WerewolfGame.GameState.PlayerInvestigateDay
            && Service.Game.CurrentState != WerewolfGame.GameState.PlayerInvestigateNight)
        {
            return false;
        }

        if(!c.CanTalkTo())
        {
            return false;
        }

        if (c.HasGivenAClueThisPhase)
        {
            return false;
        }

        return true;
    }

    public bool IsPlayerFinishedInCurrentPhase()
    {
        return !HasActionPointsLeft() && !IsTalkingToCharacter;
    }

    public bool HasActionPointsLeft()
    {
        return NumberOfActionsLeft > 0;
    }

    public bool TryConsumeActionPoint()
    {
        if (HasActionPointsLeft())
        {
            --NumberOfActionsLeft;

            VisuallyUpdateAmountOfUsedActionPoints();
            return true;
        }

        return false;
    }

    #region Actions UI

    public void VisuallyUpdateAmountOfUsedActionPoints()
    {
        int iActions = NumberOfActionsLeft;

        if (Service.Game.CurrentTimeOfDay == WerewolfGame.TOD.Day)
        {
            for(int i = DayTimeActionObjects.Count - 1; i >= 0; --i)
            {
                Image img = DayTimeActionObjects[i].GetComponent<Image>();
                if (img)
                {
                    img.color = iActions > 0 ? ActionAvailableColor : ActionSpentColor;
                }
                
                --iActions;
            }
        }   
        else if (Service.Game.CurrentTimeOfDay == WerewolfGame.TOD.Night)
        {
            for (int i = NightTimeActionObjects.Count - 1; i >= 0; --i)
            {
                Image img = NightTimeActionObjects[i].GetComponent<Image>();
                if (img)
                {
                    img.color = iActions > 0 ? ActionAvailableColor : ActionSpentColor;
                }

                --iActions;
            }
        }
    }

    public void ResetActions()
    {
        foreach(var go in DayTimeActionObjects)
        {
            Image img = go.GetComponent<Image>();
            if(img)
            {
                img.color = ActionAvailableColor;
            }
        }
        foreach (var go in NightTimeActionObjects)
        {
            Image img = go.GetComponent<Image>();
            if (img)
            {
                img.color = ActionAvailableColor;
            }
        }
    }

    public void SetDayActionsActive(bool bActive)
    {
        foreach(var go in DayTimeActionObjects)
        {
            go.SetActive(bActive);
        }
    }

    public void SetNightActionsActive(bool bActive)
    {
        foreach (var go in  NightTimeActionObjects)
        {
            go.SetActive(bActive);
        }
    }

    public void StartDay()
    {
        NumberOfActionsLeft = NumberOfTimesCanTalkToCharactersPerPhase;

        ResetActions();

        SetNightActionsActive(false);
        ActionsCanvasGroup.alpha = 0.0f;

        SetDayActionsActive(true);
        StartCoroutine(DoFadeIn());
    }

    public void EndDayOrNight()
    {
        StartCoroutine(DoFadeOut());
    }

    public void StartNight()
    {
        NumberOfActionsLeft = NumberOfTimesCanTalkToCharactersPerPhase;

        ResetActions();

        SetDayActionsActive(false);
        ActionsCanvasGroup.alpha = 0.0f;

        SetNightActionsActive(true);
        StartCoroutine(DoFadeIn());
    }

    IEnumerator DoFadeIn()
    {
        isFading = true;

        ActionsCanvasGroup.alpha = 0.0f;
        while (ActionsCanvasGroup.alpha < 1.0f)
        {
            ActionsCanvasGroup.alpha += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        ActionsCanvasGroup.alpha = 1.0f;
        isFading = false;
    }

    IEnumerator DoFadeOut()
    {
        isFading = true;

        ActionsCanvasGroup.alpha = 1.0f;
        while (ActionsCanvasGroup.alpha > 0.0f)
        {
            ActionsCanvasGroup.alpha -= Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        ActionsCanvasGroup.alpha = 0.0f;
        isFading = false;
    }

    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        Service.Player = this;

        SetDayActionsActive(false);
        SetNightActionsActive(false);
        ActionsCanvasGroup.alpha = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if(DebugGrabSomeClues)
        {
            if(Service.Game.CurrentState != WerewolfGame.GameState.PlayerInvestigateDay
                && Service.Game.CurrentState != WerewolfGame.GameState.PlayerInvestigateNight)
            {
                return;
            }

            DebugGrabSomeClues = false;

            if(Service.PhaseSolve.CurrentPhase != null)
            {
                List<int> characterIndices = Randomiser.GetRandomCharacterProcessingOrder();
                int iNumCluesToGrab = UnityEngine.Random.Range(3, 7);

                if (!SortedByDayAndPhaseClues.ContainsKey(Service.Game.CurrentDay))
                {
                    SortedByDayAndPhaseClues.Add(Service.Game.CurrentDay, new Dictionary<WerewolfGame.TOD, List<ClueObject>>());
                    SortedByDayAndPhaseClues[Service.Game.CurrentDay].Add(WerewolfGame.TOD.Day, new List<ClueObject>());
                    SortedByDayAndPhaseClues[Service.Game.CurrentDay].Add(WerewolfGame.TOD.Night, new List<ClueObject>());
                }

                foreach (var index in characterIndices)
                {
                    List<ClueObject> clues = Service.PhaseSolve.CurrentPhase.CharacterCluesToGive[Service.Population.ActiveCharacters[index]];
                    CollectedClues.Add(clues[UnityEngine.Random.Range(0, clues.Count)]);

                    SortedByDayAndPhaseClues[Service.Game.CurrentDay][Service.Game.CurrentTimeOfDay].Add(CollectedClues[CollectedClues.Count - 1]);

                    if (iNumCluesToGrab-- <= 0)
                    {
                        break;
                    }
                }
            }
        }
#endif

    }
}
