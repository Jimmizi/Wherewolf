using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
        //PhaseGenerationDay,         // We generate the story data for the next day time phase
        VictimFoundAnnouncement,    // Game announces a found victim in the morning (if any)
        //ClueGenerationDay,          // We generate the clue data from the day phase generated
        PlayerInvestigateDay,       // Player is walking around investigating the town (daytime)
        
        TransitionToNight,          // Graphic wheel transition to night
        //PhaseGenerationNight,       // We generate the story data for the next night time phase
        //ClueGenerationNight,        // We generate the clue data from the night phase generated
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
    public SceneField TitleScene;

    [SerializeField]
    public SceneField WinScene;

    [SerializeField]
    public SceneField LoseScene;

    [SerializeField]
    public Text DayCounterText;

    [SerializeField]
    public Text SummaryScreenReason;

    public GameState CurrentState = GameState.GeneratePopulation;
    private GameState NextState = InvalidState;

    public SubState CurrentSubState = SubState.Start;

    // Reason for winning or losing the game
    public string GameOverReason;

    public TOD CurrentTimeOfDay = TOD.Day;
    public int CurrentDay = 0;
    public bool IsGamePaused = false;
    public bool HasTriggeredStakeAction = false;

    public bool FirstTimeHasShownTutorial = false;

    private Character characterStaked = null;

    public int GetLastDay => ConfigManager.NumberOfDaysBeforeGameFailure;

    [SerializeField]
    public float TimeTransitionDuration = 5.0f;

    // Private vars

    private float fStateTimer = 0.0f;
    private bool canCurrentStateBeProgressed = false;
    private bool startedTransitionFadeOut = false;
    private bool startedFailLoadSceneAfterDayExpiry = false;

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

    [SerializeField]
    public GameObject SummaryScreenGameObject;
    public bool GoBackToTitleWhenAble = false;
    public void GoBackToTitleScreen()
    {
        Service.Audio.PlayUIClick();
        Debug.Log("Going back to title pressed");
        if (!GoBackToTitleWhenAble)
        {
            Service.Audio.PlayUIClick();
            GoBackToTitleWhenAble = true;
            Service.Audio.PlayBackToTitle();
            Service.Transition.BlendIn();
        }
    }
    public void SetSummaryScreenOpen()
    {
        SummaryScreenGameObject?.SetActive(true);
    }

    void Awake()
    {
        Service.Game = this;
        DontDestroyOnLoad(this.gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
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

        SubState previousSubState = CurrentSubState;

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

            //case GameState.PhaseGenerationDay:
            //case GameState.PhaseGenerationNight:
            //    ProcessStatePhaseGeneration();
            //    break;

            case GameState.VictimFoundAnnouncement:
                ProcessVictimFoundAnnouncement();
                break;

            //case GameState.ClueGenerationDay:
            //case GameState.ClueGenerationNight:
            //    ProcessStateClueGeneration();
            //    break;

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

        // Check against previously to allow processing of Finish states for a frame
        if(CurrentSubState == SubState.Finish && previousSubState == CurrentSubState)
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


    const int TransitionState_GeneratePhase = 0;
    const int TransitionState_WaitForPhaseComplete = 1;
    const int TransitionState_GenerateClues = 2;
    const int TransitionState_WaitForCluesComplete = 3;
    const int TransitionState_DONE = 4;

    private int transitionGenerationState = 0;
    void ProcessTimeTransitionState()
    {
        switch (CurrentSubState)
        {
            case SubState.Start:
                {
                    // Play wolf howl if someone was murdered this day
                    if (CurrentTimeOfDay == TOD.Night)
                    {
                        if(Service.PhaseSolve.GetVictimFromDay(CurrentDay) != null)
                        {
                            Service.Audio.PlayWolfHowl();
                        }
                    }

                    bool bFailed = CurrentDay + 1 >= ConfigManager.NumberOfDaysBeforeGameFailure
                        && CurrentState == GameState.TransitionToDay;

                    if (bFailed)
                    {
                        GameOverReason = "Took too long. The werewolf, sensing you were closing in, fled the town.";
                        startedFailLoadSceneAfterDayExpiry = false;
                    }

                    // Bring in the transition blend
                    Service.Transition.BlendIn();

                    if (!bFailed)
                    {
                        // If there is a victim to kill, try and do so
                        Service.PhaseSolve.TryKillOffCurrentVictim();
                    }

                    // Switch phase
                    CurrentTimeOfDay = CurrentState == GameState.TransitionToDay ? TOD.Day : TOD.Night;

                    // (Night -> Day, increment the day counter)
                    if(CurrentState == GameState.TransitionToDay)
                    {
                        CurrentDay++;

                        if (!bFailed)
                        { Service.Audio.StartDay(); }
                    }
                    else if (!bFailed)
                    {
                        Service.Audio.StartNight();
                    }

                    if (!bFailed)
                    {
                        if (CurrentTimeOfDay == TOD.Night)
                        {
                            // If we're now night, set the wheel as being in day time so when we transition it, it will move to night
                            Service.TransitionScreen.SetIsDay();
                        }
                        else
                        {
                            // vice versa
                            Service.TransitionScreen.SetIsNight();
                        }

                        Service.TransitionScreen.ShowPanel(0.5f);
                        Service.TransitionScreen.PerformTransition(TimeTransitionDuration * 0.7f);
                        startedTransitionFadeOut = false;
                    }

                    transitionGenerationState = TransitionState_GeneratePhase;

                    CurrentSubState++;
                    break;
                }
            case SubState.Update:
                {
                    bool bFailed = CurrentDay >= ConfigManager.NumberOfDaysBeforeGameFailure;

                    if (!bFailed)
                    {
                        // Process generation inside the daytime transition rather than as separate states
                        switch(transitionGenerationState)
                        {
                            case TransitionState_GeneratePhase:
                                {
                                    if (Service.PhaseSolve.CanGeneratePhase())
                                    {
                                        Service.PhaseSolve.StartPhaseGeneration();
                                        transitionGenerationState = TransitionState_WaitForPhaseComplete;
                                    }
                                    else
                                    {
                                        Debug.LogError("Was unable to generate a phase.");
                                    }
                                    break;
                                }
                            case TransitionState_WaitForPhaseComplete:
                                {
                                    if (!Service.PhaseSolve.IsGeneratingAPhase)
                                    {
                                        transitionGenerationState = TransitionState_GenerateClues;
                                    }
                                    break;
                                }
                            case TransitionState_GenerateClues:
                                {
                                    Service.Clue.GenerateCluesForCurrentPhase();
                                    transitionGenerationState = TransitionState_WaitForCluesComplete;
                                    break;
                                }
                            case TransitionState_WaitForCluesComplete:
                                {
                                    if (!Service.Clue.IsGeneratingClues)
                                    {
                                        transitionGenerationState = TransitionState_DONE;
                                    }
                                    break;
                                }
                        }

                        if (fStateTimer >= TimeTransitionDuration && transitionGenerationState == TransitionState_DONE)
                        {
                            CurrentSubState++;

                            foreach (var c in Service.Population.ActiveCharacters)
                            {
                                Service.Population.PhysicalCharacterMap[c].gameObject.SetActive(true);

                                c.OnTimeOfDayPhaseShift();
                                c.Update();

                                // half and half, put people already in place, and have the other half travel, so
                                //  things aren't super idle when starting a phase
                                if (UnityEngine.Random.Range(0.0f, 100.0f) < 50.0f)
                                {
                                    c.TryWarpToTaskLocation();
                                }
                                else
                                {
                                    Service.Population.PhysicalCharacterMap[c].gameObject.transform.position = 
                                        Service.Location.GetRandomNavmeshPositionInLocation(Task.GetRandomLocation());
                                }

                                if(c.IsAlive && c.IsSleeping())
                                {
                                    Service.Population.PhysicalCharacterMap[c].gameObject.SetActive(false);
                                }
                                // Hide ghosts once they've given their clue last phase
                                else if(!c.IsAlive)
                                {
                                    if (c.HasGhostGivenClue || CurrentTimeOfDay != TOD.Night)
                                    { 
                                        Service.Population.PhysicalCharacterMap[c].gameObject.SetActive(false);
                                    }
                                }
                            }

                            // Will fade in the action icons
                            if (CurrentTimeOfDay == TOD.Day)
                            {
                                Service.Player.StartDay();
                                Service.Lighting.SetDay();
                            }
                            else
                            {
                                Service.Player.StartNight();
                                Service.Lighting.SetNight();
                            }
                        }

                        // Time - 1, it takes 1 second to fade out
                        if (!startedTransitionFadeOut && fStateTimer >= TimeTransitionDuration - 1)
                        {
                            startedTransitionFadeOut = true;
                            Service.TransitionScreen.HidePanel();
                        }
                    }
                    else
                    {
                        if (!startedFailLoadSceneAfterDayExpiry && Service.Transition.IsBlendedIn())
                        {
                            startedFailLoadSceneAfterDayExpiry = true;

                            // Hide action icons
                            Service.Player.EndDayOrNight();
                            Debug.Log("Loading in lose scene");
                            SceneManager.LoadScene(LoseScene.SceneName);
                        }
                    }

                    break;
                }
            case SubState.Finish:
                {
                    if (CurrentTimeOfDay == TOD.Day)
                    {
                        Service.Audio.PlayRoosterCrow();
                    }
                    DayCounterText.text = string.Format("Day {0}", CurrentDay);
                    Service.Transition.BlendOut();
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
                        Debug.LogError("Was unable to generate a phase.");
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
                    if(!FirstTimeHasShownTutorial && Service.Transition.IsBlendedOut())
                    {
                        if (!FirstTimeHasShownTutorial)
                        {
                            FirstTimeHasShownTutorial = true;

                            if (Service.Options != null && !Service.Options.PlayerHasSeenTutorial)
                            {
                                Service.Audio.PlayDiscoveryDay();
                                Service.Options.ShowTutorial();
                            }
                        }
                    }

                    if(HasTriggeredStakeAction)
                    {
                        Service.Transition.BlendIn();

                        NextState = GameState.PlayerChoseToStake;
                        CurrentSubState++;
                    }
                    else if(Service.Player.IsPlayerFinishedInCurrentPhase())
                    {
                        ProgressGameFromExternal();
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
                    if (Service.Transition.IsBlendedIn())
                    {
                        // Hide action icons
                        Service.Player.EndDayOrNight();

                        Debug.Assert(characterStaked != null);
                        if (characterStaked.IsWerewolf)
                        {
                            Debug.Log("Successfully staked the werewolf.");
                            GameOverReason = string.Format("Successfully staked the werewolf! It was {0} all along!", characterStaked.Name);
                            SceneManager.LoadScene(WinScene.SceneName);
                        }
                        else
                        {
                            Debug.Log("Accidently staked a civilian.");
                            GameOverReason = "You staked a civilian, the werewolf managed to get away.";
                            SceneManager.LoadScene(LoseScene.SceneName);
                        }

                        CurrentSubState++;
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
                    SetSummaryScreenOpen();
                    CurrentSubState++;
                    break;
                }
            case SubState.Update:
                {
                    if(GoBackToTitleWhenAble)
                    {
                        Debug.Log("Waiting for transition to blend in....");
                        if (Service.Transition.IsBlendedIn())
                        {
                            GoBackToTitleWhenAble = false;
                            SceneManager.LoadScene(TitleScene.SceneName);
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

    #endregion

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(CurrentState == GameState.PlayerChoseToStake)
        {
            if (characterStaked.IsWerewolf)
            {
                Debug.Log("Loaded win scene.");
                NextState = GameState.GameSummaryWon;
                CurrentSubState = SubState.Finish;

                if (CurrentTimeOfDay == TOD.Day)
                {
                    Service.Audio.PlayWinDay();
                }
                else
                {
                    Service.Audio.PlayWinNight();
                }
            }
            else
            {
                Debug.Log("Loaded lose scene.");
                NextState = GameState.GameSummaryLoss;
                CurrentSubState = SubState.Finish;

                if (CurrentTimeOfDay == TOD.Day)
                {
                    Service.Audio.PlayGameOverDay();
                }
                else
                {
                    Service.Audio.PlayGameOverNight();
                }
            }

            if (SummaryScreenReason)
            {
                SummaryScreenReason.text = GameOverReason;
            }

            Service.Transition.BlendOut();
        }

        if(CurrentState == GameState.TransitionToDay
            || CurrentState == GameState.TransitionToDay)
        {
            Debug.Log("Loaded lose scene.");
            NextState = GameState.GameSummaryLoss;
            CurrentSubState = SubState.Finish;

            // Play game over day as we lose at the end of the last night
            Service.Audio.PlayGameOverNight();
            Service.Transition.BlendOut();

            if(SummaryScreenReason)
            {
                SummaryScreenReason.text = GameOverReason;
            }
        }

        if (CurrentState == GameState.GameSummaryWon
            || CurrentState == GameState.GameSummaryLoss)
        {
            Debug.Log("Destroying Manager object at transition back to title");
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Object.Destroy(gameObject);
        }
    }

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
                //case GameState.PhaseGenerationDay:
                //case GameState.ClueGenerationDay:
                case GameState.PlayerInvestigateDay:
                case GameState.TransitionToNight:
                //case GameState.PhaseGenerationNight:
                //case GameState.ClueGenerationNight:
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
            characterStaked = c;
            HasTriggeredStakeAction = true;
        }
    }


    #region DEBUG
#if UNITY_EDITOR

    Vector2 vStakeSelectionScrollPosition = new Vector2();
    Vector2 vClueSelectionScrollPosition = new Vector2();
    Vector2 vPLayerCluesScrollPosition = new Vector2();
    bool DebugChosenToStake = false;

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

        if (CurrentState == GameState.TransitionToDay
            || CurrentState == GameState.TransitionToNight)
        {
            string sTransitionState = "";
            switch(transitionGenerationState)
            {
                case TransitionState_GeneratePhase: sTransitionState = "GeneratePhase"; break;
                case TransitionState_WaitForPhaseComplete: sTransitionState = "WaitForPhaseComplete"; break;
                case TransitionState_GenerateClues: sTransitionState = "GenerateClues"; break;
                case TransitionState_WaitForCluesComplete: sTransitionState = "WaitForCluesComplete"; break;
                case TransitionState_DONE: sTransitionState = "DONE"; break;
            }

            GUI.Label(new Rect(415, 30, 400, 24), sTransitionState);
        }

        float fHeight = Screen.height - 80;

        GUI.Box(new Rect(10, 40, 900, fHeight), "");

        // Choose to stake a character
        vStakeSelectionScrollPosition = GUI.BeginScrollView(new Rect(15, 45, 200, fHeight - 10), vStakeSelectionScrollPosition, new Rect(0, 0, 190, 1000));
        {
            if (!DebugChosenToStake)
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
                        DebugChosenToStake = true;
                    }
                    vPosition.y += 28;
                }
            }
        }
        GUI.EndScrollView();

        // Choose to talk to character
        vClueSelectionScrollPosition = GUI.BeginScrollView(new Rect(220, 45, 200, fHeight - 10), vClueSelectionScrollPosition, new Rect(0, 0, 190, 1000));
        {
            if (!DebugChosenToStake)
            {
                Vector2 vPosition = new Vector2(5, 5);
                GUI.Label(new Rect(vPosition.x, vPosition.y, 200, 24), "Select a character to talk to");
                vPosition.y += 16;

                foreach (var c in Service.Population.ActiveCharacters)
                {
                    if (!c.IsAlive)
                    {
                        continue;
                    }

                    if(c.HasGivenAClueThisPhase)
                    {
                        GUI.Label(new Rect(vPosition.x, vPosition.y, 200, 24), string.Format("[{0}] {1} Already given a clue.", c.Index, c.Name));
                        vPosition.y += 28;
                        continue;
                    }

                    if (GUI.Button(new Rect(vPosition.x, vPosition.y, 140, 24), string.Format("[{0}] {1}", c.Index, c.Name)))
                    {
                        Service.Player.TryGetClueFromCharacter(c);
                    }
                    vPosition.y += 28;
                }
            }
        }
        GUI.EndScrollView();

        vPLayerCluesScrollPosition = GUI.BeginScrollView(new Rect(425, 45, 450, fHeight - 10), vPLayerCluesScrollPosition, new Rect(0, 0, 440, 5000));
        {
            Vector2 vPosition = new Vector2(5, 5);

            foreach (var clueMap in Service.Player.SortedByDayAndPhaseClues)
            {
                GUI.Label(new Rect(vPosition.x, vPosition.y, 200, 24), string.Format("Day {0} clues:", clueMap.Key));
                vPosition.y += 16;
                foreach(var cluePhase in clueMap.Value)
                {
                    GUI.Label(new Rect(vPosition.x + 7, vPosition.y, 200, 24), string.Format("{0}:", cluePhase.Key.ToString()));
                    vPosition.y += 16;
                    foreach(var clue in cluePhase.Value)
                    {
                        // Type
                        GUI.contentColor = Color.yellow;
                        GUI.Label(new Rect(vPosition.x, vPosition.y, 150, 24), clue.Type.ToString());

                        // True/false
                        GUI.contentColor = clue.IsTruth ? new Color(0.1f, 0.8f, 0.1f) : new Color(0.8f, 0.1f, 0.1f);
                        GUI.Label(new Rect(vPosition.x + 200, vPosition.y, 150, 24),
                            string.Format("({0})", clue.IsTruth ? "Is the truth" : "Is a lie"));

                        vPosition.y += 16;
                        GUI.contentColor = Color.white;

                        // Extra info
                        if (clue.RelatesToCharacter.IsWerewolf)
                        {
                            GUI.contentColor = new Color(1.0f, 0.5f, 0.5f);
                        }
                        GUI.Label(new Rect(vPosition.x, vPosition.y, 200, 24),
                            string.Format("Subject: [{0}] {1}", clue.RelatesToCharacter.Index, clue.RelatesToCharacter.Name));
                        GUI.contentColor = Color.white;

                        GUI.Label(new Rect(vPosition.x + 200, vPosition.y, 200, 24),
                             string.Format("LocSeenIn: {0}", clue.LocationSeenIn));

                        vPosition.y += 16;

                        if (clue.Type == ClueObject.ClueType.VisualFromGhost)
                        {
                            GUI.Label(new Rect(vPosition.x, vPosition.y, 350, 24),
                                string.Format("Ghost Descriptor Type: {0}", clue.GhostGivenClueType.ToString()));
                            vPosition.y += 16;
                        }

                        if (clue.Emotes.Count > 0)
                        {
                            GUI.Label(new Rect(vPosition.x, vPosition.y, 200, 24), "Emotes string:");
                            vPosition.y += 16;

                            GUI.contentColor = new Color(0.2f, 0.7f, 1.0f);
                            foreach (var emote in clue.Emotes)
                            {
                                GUI.Label(new Rect(vPosition.x, vPosition.y, 300, 24), emote.SubType.ToString());
                                vPosition.y += 16;
                            }
                            GUI.contentColor = Color.white;
                        }

                        vPosition.y += 16;
                    }

                    vPosition.y += 16;
                }

                vPosition.y += 16;
            }
        }
        GUI.EndScrollView();
    }
#endif
    #endregion
}
