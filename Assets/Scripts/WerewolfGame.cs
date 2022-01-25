using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WerewolfGame : MonoBehaviour
{
    public enum TOD
    {
        Day,
        Night,
        EndOfNight
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
        GameSummary,                // End of game screen, with results on win/lose and statistics
    }

#if UNITY_EDITOR
    public bool DebugGoToNextPhase;
    public bool DebugStartNextPhase;
#endif

    public TOD CurrentTimeOfDay = TOD.EndOfNight;
    public int CurrentDay = 1;
    public bool IsGamePaused = true;

    public bool CanUpdate => !IsGamePaused;

    void Awake()
    {
        Service.Game = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        

#if UNITY_EDITOR
        if(DebugGoToNextPhase)
        {
            DebugGoToNextPhase = false;

            GoToNextPhase();
        }
        if(DebugStartNextPhase)
        {
            IsGamePaused = false;
        }
#endif
    }

    void GoToNextPhase()
    {
        if (CurrentTimeOfDay == TOD.Day)
        {
            CurrentTimeOfDay = TOD.Night;
            ShowPhaseShiftAnimation(TOD.Day, TOD.Night);
        }
        else if (CurrentTimeOfDay == TOD.Night)
        {
            CurrentTimeOfDay = TOD.EndOfNight;
        }
        else if (CurrentTimeOfDay == TOD.EndOfNight)
        {
            CurrentTimeOfDay = TOD.Day;
            ShowPhaseShiftAnimation(TOD.Night, TOD.Day);
        }

        IsGamePaused = true;

        if (CurrentTimeOfDay != TOD.EndOfNight)
        { 
            ProcessPhaseShiftOnAllCharacters(); 
        }
    }

    void ShowPhaseShiftAnimation(TOD eFrom, TOD eTo)
    {

    }

    void BeginPhase()
    {

    }

    void ProcessPhaseShiftOnAllCharacters()
    {
        foreach(var c in Service.Population.ActiveCharacters)
        {
            c.OnTimeOfDayPhaseShift();
        }
    }

}
