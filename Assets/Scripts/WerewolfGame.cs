using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WerewolfGame : MonoBehaviour
{
    public enum TOD
    {
        Day,
        Night
    }

    public enum GameState
    {
        GeneratePopulation,         // We generate all of the population, ready for the game
        IntroStorySegment,          // Little story segment about a previous (out of game) murder

        // \/ Start of game loop \/
        TransitionToDay,            // Graphic wheel transition to day
        VictimFoundAnnouncement,    // Game announces a found victim in the morning (if any)
        PhaseGenerationDay,         // We generate the story data for the next day time phase
        ClueGenerationDay,          // We generate the clue data from the day phase generated
        PlayerInvestigateDay,       // Player is walking around investigating the town (daytime)
        
        TransitionToNight,          // Graphic wheel transition to night
        PhaseGenerationNight,       // We generate the story data for the next night time phase
        ClueGenerationNight,        // We generate the clue data from the night phase generated
        PlayerInvestigateNight,     // Player is walking around investigating the town (nighttime)

        PlayerInformationReview,    // Corkboard scene where the player can organise their clues and thoughts
        // /\ End of game loop /\

        PlayerChoseToStake,         // Scene change to the player having chosen to stake a towns-person
        GameSummaryWon,             // End of game screen, with results on win/lose and statistics
        GameSummaryLoss,            // End of game screen, with results on win/lose and statistics
    }

    public static GameState InvalidState => (GameState)(-1);

    public enum SubState
    {
        Start,
        Update,
        Finish
    }

    // public vars

#if UNITY_EDITOR
    public bool DisplayDebug = true;
#endif

    [SerializeField]
    public SceneField WinScene;

    [SerializeField]
    public SceneField LoseScene;

    public GameState CurrentState = GameState.GeneratePopulation;
    private GameState NextState = InvalidState;

    public SubState CurrentSubState = SubState.Start;

    public TOD CurrentTimeOfDay = TOD.Day;
    public int CurrentDay = 0;
    public bool IsGamePaused = false;
    public bool HasTriggeredStakeAction = false;

    public int GetLastDay => ConfigManager.NumberOfDaysBeforeGameFailure;

    [SerializeField]
    public float TimeTransitionDuration = 5.0f;

    // Private vars

    private float fStateTimer = 0.0f;
    private bool canCurrentStateBeProgressed = false;

    public bool CanUpdatePopulation()
    {
        if(IsGamePaused)
        {
            return false;
        }

        if(CurrentState != GameState.PlayerInvestigateDay && CurrentState != GameState.PlayerInvestigateNight)
        {
            return false;
        }

        if(CurrentSubState != SubState.Update)
        {
            return false;
        }

        return true;
    }

    void Awake()
    {
        Service.Game = this;
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Update()
    {
        #region DEBUG
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F3))
        {
            Service.PhaseSolve.bShowDebug = false;
            Service.Population.bShowDebug = false;
            DisplayDebug = !DisplayDebug;
        }

        if (DisplayDebug && Input.GetKeyDown(KeyCode.F5))
        {
            ProgressGameFromExternal();
        }
        if (DisplayDebug && Input.GetKeyDown(KeyCode.F6))
        {
            IsGamePaused = !IsGamePaused;
        }
#endif
        #endregion

        if (IsGamePaused)
        {
            return;
        }

        fStateTimer += Time.deltaTime;

        switch (CurrentState)
        {
            case GameState.GeneratePopulation:
                ProcessStateGeneratePopulation();
                break;
            case GameState.IntroStorySegment:
                ProcessStateIntroStorySegment();
                break;

            case GameState.TransitionToDay:
            case GameState.TransitionToNight:
                ProcessTimeTransitionState();
                break;

            case GameState.VictimFoundAnnouncement:
                ProcessVictimFoundAnnouncement();
                break;

            case GameState.PhaseGenerationDay:
            case GameState.PhaseGenerationNight:
                ProcessStatePhaseGeneration();
                break;

            case GameState.ClueGenerationDay:
            case GameState.ClueGenerationNight:
                ProcessStateClueGeneration();
                break;

            case GameState.PlayerInvestigateDay:
            case GameState.PlayerInvestigateNight:
                ProcessStatePlayerInvestigate();
                break;
            
            case GameState.PlayerInformationReview:
                ProcessStatePlayerInformationReview();
                break;

            case GameState.PlayerChoseToStake:
                ProcessStatePlayerChoseToStake();
                break;

            case GameState.GameSummaryWon:
            case GameState.GameSummaryLoss:
                ProcessStateGameSummary();
                break;
        }

        if(CurrentSubState == SubState.Finish)
        {
            // This means we're naturally progressing, rather than something external pushing us through
            if (NextState == InvalidState)
            {
                ProgressGame(true);
            }
            else
            {
                CurrentSubState = SubState.Start;
            }

            canCurrentStateBeProgressed = false;
            fStateTimer = 0.0f;

            CurrentState = NextState;
            NextState = InvalidState;
        }
    }

    #region State processing

    void ProcessStateGeneratePopulation()
    {
        switch (CurrentSubState)
        {
            case SubState.Start:
                {
                    Service.Population.Init();
                    CurrentSubState++;
                    break;
                }
            case SubState.Update:
                {
                    if(Service.Population.CharacterCreationDone)
                    {
                        CurrentSubState++;
                    }
                    break;
                }
        }
    }
    void ProcessStateIntroStorySegment()
    {
        switch (CurrentSubState)
        {
            case SubState.Start:
                {
                    CurrentSubState = SubState.Finish;
                    break;
                }
            case SubState.Update:
                {

                    break;
                }
            case SubState.Finish:
                {

                    break;
                }
        }
    }
    void ProcessTimeTransitionState()
    {
        switch (CurrentSubState)
        {
            case SubState.Start:
                {
                    // If there is a victim to kill, try and do so
                    Service.PhaseSolve.TryKillOffCurrentVictim();

                    CurrentTimeOfDay = CurrentState == GameState.TransitionToDay ? TOD.Day : TOD.Night;

                    // (Night -> Day, increment the day counter)
                    if(CurrentState == GameState.TransitionToDay)
                    {
                        CurrentDay++;
                        Service.Audio.StartDay();
                    }
                    else
                    {
                        Service.Audio.StartNight();
                    }

                    CurrentSubState++;
                    break;
                }
            case SubState.Update:
                {
                    if(fStateTimer >= TimeTransitionDuration)
                    {
                        CurrentSubState++;

                        foreach (var c in Service.Population.ActiveCharacters)
                        {
                            c.OnTimeOfDayPhaseShift();
                        }
                    }
                    break;
                }
            case SubState.Finish:
                {

                    break;
                }
        }
    }
    void ProcessVictimFoundAnnouncement()
    {
        switch (CurrentSubState)
        {
            case SubState.Start:
                {
                    Character v = Service.PhaseSolve.GetVictimFromDay(CurrentDay - 1);
                    if (v != null)
                    {
                        // Setup announcement

                        CurrentSubState = SubState.Finish; // Don't actually finish when we have a way to announce
                    }
                    else
                    {
                        CurrentSubState = SubState.Finish;
                    }

                    break;
                }
            case SubState.Update:
                {

                    break;
                }
            case SubState.Finish:
                {

                    break;
                }
        }
    }
    void ProcessStatePhaseGeneration()
    {
        switch (CurrentSubState)
        {
            case SubState.Start:
                {
                    if (Service.PhaseSolve.CanGeneratePhase())
                    {
                        Service.PhaseSolve.StartPhaseGeneration();
                        CurrentSubState++;
                    }
                    else
                    {
                        // TODO: Go to fail screen?
                    }
                    break;
                }
            case SubState.Update:
                {
                    if(!Service.PhaseSolve.IsGeneratingAPhase)
                    {
                        CurrentSubState++;
                    }
                    break;
                }
        }
    }
    void ProcessStateClueGeneration()
    {
        switch (CurrentSubState)
        {
            case SubState.Start:
                {
                    Service.Clue.GenerateCluesForCurrentPhase();
                    CurrentSubState++;
                    break;
                }
            case SubState.Update:
                {
                    if(!Service.Clue.IsGeneratingClues)
                    {
                        CurrentSubState++;
                    }
                    break;
                }
            case SubState.Finish:
                {

                    break;
                }
        }
    }
    void ProcessStatePlayerInvestigate()
    {
        switch (CurrentSubState)
        {
            case SubState.Start:
                {
                    canCurrentStateBeProgressed = true;
                    CurrentSubState++;
                    break;
                }
            case SubState.Update:
                {
                    if(HasTriggeredStakeAction)
                    {
                        NextState = GameState.PlayerChoseToStake;
                        CurrentSubState++;
                    }
                    break;
                }
            case SubState.Finish:
                {

                    break;
                }
        }
    }
    void ProcessStatePlayerInformationReview()
    {
        switch (CurrentSubState)
        {
            case SubState.Start:
                {
                    CurrentSubState = SubState.Finish;
                    break;
                }
            case SubState.Update:
                {

                    break;
                }
            case SubState.Finish:
                {

                    break;
                }
        }
    }
    void ProcessStatePlayerChoseToStake()
    {
        switch (CurrentSubState)
        {
            case SubState.Start:
                {
                    // Perhaps start a character animation?
                    // Scene change?

                    foreach(var c in Service.Population.ActiveCharacters)
                    {
                        if(c.ChosenForStakeTarget)
                        {
                            if(c.IsWerewolf)
                            {
                                Debug.Log("Successfully staked the werewolf.");
                                CurrentState = GameState.GameSummaryWon;
                                SceneManager.LoadScene(WinScene.SceneName);
                                //
                            }
                            else
                            {
                                Debug.Log("Accidently staked a civilian.");
                                CurrentState = GameState.GameSummaryLoss;
                                SceneManager.LoadScene(LoseScene.SceneName);
                            }
                        }
                    }

                    break;
                }
            case SubState.Update:
                {

                    break;
                }
            case SubState.Finish:
                {

                    break;
                }
        }
    }
    void ProcessStateGameSummary()
    {
        switch (CurrentSubState)
        {
            case SubState.Start:
                {

                    break;
                }
            case SubState.Update:
                {

                    break;
                }
            case SubState.Finish:
                {

                    break;
                }
        }

    }

    #endregion

    public void ProgressGameFromExternal()
    {
        ProgressGame();
    }
    void ProgressGame(bool bProgressFromFinish = false)
    {
        if (!bProgressFromFinish)
        {
            if (!canCurrentStateBeProgressed)
            {
                return;
            }
        }

        if (bProgressFromFinish)
        {
            switch (CurrentState)
            {
                case GameState.GeneratePopulation:
                case GameState.IntroStorySegment:
                case GameState.TransitionToDay:
                case GameState.VictimFoundAnnouncement:
                case GameState.PhaseGenerationDay:
                case GameState.ClueGenerationDay:
                case GameState.PlayerInvestigateDay:
                case GameState.TransitionToNight:
                case GameState.PhaseGenerationNight:
                case GameState.ClueGenerationNight:
                case GameState.PlayerInvestigateNight:
                    NextState = CurrentState + 1;
                    break;

                case GameState.PlayerInformationReview:
                    NextState = GameState.TransitionToDay;
                    break;

                case GameState.PlayerChoseToStake:
                    break;

                case GameState.GameSummaryWon:
                case GameState.GameSummaryLoss:
                    break;
            }
        }

        CurrentSubState = bProgressFromFinish ? SubState.Start : SubState.Finish;
    }

    
    public void ProcessPlayerActionStakeCharacter(Character c)
    {
        if(CurrentState != GameState.PlayerChoseToStake)
        {
            c.ChosenForStakeTarget = true;
            HasTriggeredStakeAction = true;
        }
    }


    #region DEBUG
#if UNITY_EDITOR

    Vector2 vStakeSelectionScrollPosition = new Vector2();
    bool chosenToStake = false;

    private void OnGUI()
    {
        if (!DisplayDebug && !Service.PhaseSolve.bShowDebug && !Service.Population.bShowDebug)
        {
            GUI.Label(new Rect(5, 5, 200, 24), "F1 - Population Debug");
            GUI.Label(new Rect(5, 22, 200, 24), "F2 - PhaseSolver Debug");
            GUI.Label(new Rect(5, 39, 200, 24), "F3 - GameManager Debug");
        }

        if (!DisplayDebug)
        {
            return;
        }

        GUI.Label(new Rect(6, 5, 200, 24), "F5 - Step game forward.");
        GUI.Label(new Rect(6, 22, 300, 24), string.Format("F6 - Pause/Unpause{0}", IsGamePaused ? " (Currently Paused)" : ""));
        GUI.Label(new Rect(410, 5, 400, 24), string.Format("{0} ({1}) : {2} - Day {3}, {4}", 
            CurrentState.ToString(),
            fStateTimer.ToString("0.0"),
            CurrentSubState.ToString(), 
            CurrentDay, 
            CurrentTimeOfDay.ToString()));

        float fHeight = Screen.height - 80;

        GUI.Box(new Rect(10, 40, 600, fHeight), "");

        // Choose to stake a character
        vStakeSelectionScrollPosition = GUI.BeginScrollView(new Rect(15, 45, 200, fHeight - 10), vStakeSelectionScrollPosition, new Rect(0, 0, 190, 1000));
        {
            if (!chosenToStake)
            {
                Vector2 vPosition = new Vector2(5, 5);
                GUI.Label(new Rect(vPosition.x, vPosition.y, 200, 24), "Select a character to stake");
                vPosition.y += 16;

                foreach (var c in Service.Population.ActiveCharacters)
                {
                    if (!c.IsAlive)
                    {
                        continue;
                    }

                    if (GUI.Button(new Rect(vPosition.x, vPosition.y, 140, 24), string.Format("[{0}] {1}", c.Index, c.Name)))
                    {
                        ProcessPlayerActionStakeCharacter(c);
                        chosenToStake = true;
                    }
                    vPosition.y += 28;
                }
            }
        }
        GUI.EndScrollView();
    }
#endif
    #endregion
}
