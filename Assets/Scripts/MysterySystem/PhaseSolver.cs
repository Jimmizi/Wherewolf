using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseSolver : MonoBehaviour
{
    public SortedDictionary<int, List<Phase>> PhaseHistory = new SortedDictionary<int, List<Phase>>();

    public Phase CurrentPhase;
    public bool IsGeneratingAPhase;

#if UNITY_EDITOR
    public bool bShowDebug = false;
    bool firstTimeGeneratingPhase = true;
#endif

    // Start is called before the first frame update
    void Awake()
    {
        Service.PhaseSolve = this;
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F2))
        {
            Service.Population.bShowDebug = false;
            bShowDebug = !bShowDebug;
        }
        if (Input.GetKeyDown(KeyCode.F5) && bShowDebug && !IsGeneratingAPhase)
        {
            if (Service.Population.CharacterCreationDone)
            {
                bool incrementDay = lastDebugGeneratedTod == WerewolfGame.TOD.Night && !firstTimeGeneratingPhase;
                firstTimeGeneratingPhase = false;

                lastDebugGeneratedTod = lastDebugGeneratedTod == WerewolfGame.TOD.Day ? WerewolfGame.TOD.Night : WerewolfGame.TOD.Day;
                StartCoroutine(DebugGeneratePhase(lastDebugGeneratedTod, incrementDay));
            }
        }
#endif
    }

    // Public Access

    public bool CanGeneratePhase() => Service.Game.CurrentDay < 20;


    // Private functions

    IEnumerator GeneratePhase(WerewolfGame.TOD eTod)
    {
        IsGeneratingAPhase = true;

        Phase nextPhase = new Phase(eTod);

        // 1) Werewolf finds a character to kill
        if (ShouldGenerateVictim())
        {
            Character victim = GetBestCharacterForVictim(eTod);
            int iLocation = GetBestLocationForVictimDeath(eTod, victim);

            victim.IsVictim = true;
            nextPhase.GeneratedAVictim = true;

            GenerateWerewolfAndVictimTasks(eTod, victim, iLocation);
        }
        else
        {
            GenerateWerewolfAndVictimTasks(eTod);
        }

        // Yield for about a frame (a frame at 60fps is 0.016666)
        yield return new WaitForSeconds(0.02f);

        // 2) Next up, we randomise locations for characters doing idles and wanders

        foreach(var c in Service.Population.ActiveCharacters)
        {
            if(!c.IsAlive || c.IsWerewolf || c.IsVictim)
            {
                continue;
            }

            RandomiseCharacterTaskLocations(eTod, c);

            yield return new WaitForSeconds(0.02f);
        }

        // 3) Then we calculate what characters everyone would have seen throughout the day

        foreach(var c in Service.Population.ActiveCharacters)
        {
            if (!c.IsAlive)
            {
                continue;
            }

            nextPhase.CharacterSeenMap.Add(c, CalculateSeenCharactersDuringPhase(eTod, c));
            nextPhase.CharacterTasks.Add(c, eTod == WerewolfGame.TOD.Day ? c.TaskSchedule.DayTasks : c.TaskSchedule.NightTasks);

            yield return new WaitForSeconds(0.02f);
        }

        // 4) Done

        if (CurrentPhase != null)
        {
            if (!PhaseHistory.ContainsKey(Service.Game.CurrentDay))
            {
                PhaseHistory.Add(Service.Game.CurrentDay, new List<Phase>());
            }

            PhaseHistory[Service.Game.CurrentDay].Add(new Phase(CurrentPhase));
        }

        CurrentPhase = nextPhase;

        IsGeneratingAPhase = false;
        
        yield return null;
    }

    bool ShouldGenerateVictim()
    {
        switch(Service.Game.CurrentDay)
        {
            // The days we're allowed to generate a victim (every 2 days atm)
            case 1: // 1 in this case is the first day (not 0)
            case 3:
            case 5:
            case 7:
            case 9:
            case 11:
            case 13:
            case 15:
            case 17:
            case 19:
            {
                break;
            }
            default:
                return false;
        }

        // If we have a current phase (meaning the last phase)
        if(CurrentPhase != null)
        {
            // If the last phase was daytime and it already generated a victim, then don't
            if(CurrentPhase.GeneratedAVictim && CurrentPhase.TimeOfDay == WerewolfGame.TOD.Day)
            {
                return false;
            }
        }

        return true;
    }

    Character GetBestCharacterForVictim(WerewolfGame.TOD eTod)
    {
        List<Character> applicableCharacters = new List<Character>();

        foreach (var c in Service.Population.ActiveCharacters)
        {
            // Must be alive and not werewolf to be a victim
            if(!c.CanBeKilledByWerewolf(eTod))
            {
                continue;
            }

            applicableCharacters.Add(c);
        }

        Debug.Assert(applicableCharacters.Count > 0, "GetBestCharacterForVictim: Did not manage to find any killable characters. What's up with that?");
        if(applicableCharacters.Count == 0)
        {
            // In an emergency, run this again but don't care if people are sleeping
            foreach (var c in Service.Population.ActiveCharacters)
            {
                // Must be alive and not werewolf to be a victim
                if (!c.CanBeKilledByWerewolf(eTod, false))
                {
                    continue;
                }

                applicableCharacters.Add(c);
            }
        }

        if(applicableCharacters.Count == 0)
        {
            Debug.LogError(string.Format("Wasn't able to find a victim for day {0}... Something has gone horribly wrong!", Service.Game.CurrentDay));
        }

        return applicableCharacters[UnityEngine.Random.Range(0, applicableCharacters.Count)];
    }
    int GetBestLocationForVictimDeath(WerewolfGame.TOD eTod, Character victim)
    {
        int GetRandomLocationFromSchedule()
        {
            List<Task> taskList = eTod == WerewolfGame.TOD.Day ? victim.TaskSchedule.DayTasks : victim.TaskSchedule.NightTasks;
            List<int> availableLocations = new List<int>();

            foreach(var t in taskList)
            {
                if(t.Type != Task.TaskType.Sleep)
                {
                    availableLocations.Add(t.Location);
                }
            }

            return availableLocations.Count > 0
                ? availableLocations[UnityEngine.Random.Range(0, availableLocations.Count)]
                : -1;
        }

        // If the character would have been around certain locations during this time of day, try to get those before 
        //  randomly finding a position, as this will be more consistent to where they would have been
        int iLocationFromTasks = GetRandomLocationFromSchedule();

        // They didn't have any task locations, so just make a random one (should be pretty unlikely for this to happen)
        if(iLocationFromTasks == -1)
        {
            iLocationFromTasks = UnityEngine.Random.Range(Emote.LocationMin, Emote.LocationMax + 1);
        }

        return iLocationFromTasks;
    }
    void GenerateWerewolfAndVictimTasks(WerewolfGame.TOD eTod, Character victim = null, int iLocation = -1)
    {
        // When victim is null, we just get the werewolf to wander about
        // otherwise we have a victim and can task them both up together, for the werewolf to essentially follow the character

        Character ww = Service.Population.GetWerewolf();
        ww.TaskSchedule.Clear();
        victim?.TaskSchedule.Clear();

        void AddTask(Task.TaskType eType, int iLoc)
        {
            if (eTod == WerewolfGame.TOD.Day)
            {
                ww.TaskSchedule.DayTasks.Add(new Task(ww, eType, iLoc));
                victim?.TaskSchedule.DayTasks.Add(new Task(victim, eType, iLoc));
            }
            else if (eTod == WerewolfGame.TOD.Night)
            {
                ww.TaskSchedule.NightTasks.Add(new Task(ww, eType, iLoc));
                victim?.TaskSchedule.DayTasks.Add(new Task(victim, eType, iLoc));
            }
        }

        Task.TaskType eFirstTaskType = UnityEngine.Random.Range(0.0f, 100.0f) < 50.0f ? Task.TaskType.WanderArea :Task.TaskType.Idle;
        Task.TaskType eSecondTaskType = eFirstTaskType == Task.TaskType.WanderArea ? Task.TaskType.Idle : Task.TaskType.WanderArea;
        
        if (victim == null)
        {
            Vector3 vDummyPosition;
            int iDummyLocation;

            Service.Population.GetWorkPositionAndLocation(ww, out vDummyPosition, out iDummyLocation);

            // If the werewolf has an occupation, try a 50% chance to go to work
            eSecondTaskType = iDummyLocation >= 0
                ? (UnityEngine.Random.Range(0.0f, 100.0f) < 50.0f ? Task.TaskType.Work : eSecondTaskType)
                : eSecondTaskType;
        }

        AddTask(eFirstTaskType, iLocation > -1 ? Service.Population.GetRandomAdjacentLocation(iLocation) : -1);
        AddTask(eSecondTaskType, iLocation);
    }

    void RandomiseCharacterTaskLocations(WerewolfGame.TOD eTod, Character c)
    {
        void RandomiseTaskLocations(List<Task> tasks)
        {
            foreach(var t in tasks)
            {
                // Generally (75% chance) to not randomise a task (create a sense of characters sticking to areas)
                if(UnityEngine.Random.Range(0.0f, 100.0f) < 75.0f)
                {
                    continue;
                }

                if(t.Type == Task.TaskType.WanderArea || t.Type == Task.TaskType.Idle)
                {
                    t.Location = Task.GetRandomLocation();
                    t.UpdatePosition();
                }
            }
        }

        RandomiseTaskLocations(eTod == WerewolfGame.TOD.Day ? c.TaskSchedule.DayTasks : c.TaskSchedule.NightTasks);
    }

    List<Character> CalculateSeenCharactersDuringPhase(WerewolfGame.TOD eTod, Character character)
    {
        List<Character> otherCharactersSawDuringThisPhase = new List<Character>();

        List<int> iLocationsIn = new List<int>();

        foreach(var t in (eTod == WerewolfGame.TOD.Day? character.TaskSchedule.DayTasks : character.TaskSchedule.NightTasks))
        {
            // Wouldn't be able to see other people when sleeping
            if (t.Type != Task.TaskType.Sleep)
            {
                iLocationsIn.Add(t.Location);
            }
        }

        // Having zero is valid if it's night and the character only has a sleep task
        if (iLocationsIn.Count > 0)
        {
            foreach (var c in Service.Population.ActiveCharacters)
            {
                // Can't see self - well you can but you know what I mean
                if (c == character)
                {
                    continue;
                }

                foreach (var t in (eTod == WerewolfGame.TOD.Day ? c.TaskSchedule.DayTasks : c.TaskSchedule.NightTasks))
                {
                    if(t.Type != Task.TaskType.Sleep && iLocationsIn.Contains(t.Location))
                    {
                        otherCharactersSawDuringThisPhase.Add(c);
                        break;
                    }
                }
            }
        }

        return otherCharactersSawDuringThisPhase;
    }


    #region DEBUG
#if UNITY_EDITOR

    IEnumerator DebugGeneratePhase(WerewolfGame.TOD eTod, bool incrementDay)
    {
        yield return GeneratePhase(eTod);

        if (incrementDay)
        {
            Service.Game.CurrentDay++;
        }
    }

    const int iTextHeight = 16;
    const int iButtonHeight = 28;
    const int iTextBoxHeight = 24;
    Phase currentPhaseDebugging;

    WerewolfGame.TOD lastDebugGeneratedTod = WerewolfGame.TOD.Night;

    Vector2 vScrollPositionHistory = new Vector2();
    Vector2 vCharacterPickerPosition = new Vector2();
    Vector2 vPickedCharacterDetailsPosition = new Vector2();
    Character CurrentCharacterSeenListSelected;
    GUIStyle selectedStyle = null;

    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i)
        {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }

    void OnGUI()
    {
        if(!bShowDebug)
        {
            return;
        }

        if (selectedStyle == null)
        {
            selectedStyle = new GUIStyle(GUI.skin.box);
            selectedStyle.normal.background = MakeTex(2, 2, new Color(1f, 1f, 0f, 0.5f));
        }

        GUI.Box(new Rect(30, 30, 1050, 700), "");
        GUI.Label(new Rect(6, 0, 500, 24), "Phase Debug");
        GUI.Label(new Rect(206, 0, 800, 24), string.Format("F5 - Debug generate phase{0}", IsGeneratingAPhase ? " (Currently generating)" : ""));

        // List of history + current at top
        // Panel for general info (werewolf and victim)
        // Panel for who saw who

        // Phase history picker
        vScrollPositionHistory = GUI.BeginScrollView(new Rect(35, 35, 190, 690), vScrollPositionHistory, new Rect(0, 0, 170, 1500));
        {
            Vector2 vPosition = new Vector2(5, 5);

            // Selecting the current phase
            if (currentPhaseDebugging != null && currentPhaseDebugging == CurrentPhase)
            {
                GUI.Box(new Rect(vPosition.x + 50 - 2, vPosition.y - 2, 104, iTextBoxHeight + 4), "", selectedStyle);
            }
            if (CurrentPhase != null)
            {
                GUI.Label(new Rect(vPosition.x, vPosition.y, 50, iTextBoxHeight), "Curr:");
                if (GUI.Button(new Rect(vPosition.x + 50, vPosition.y, 100, iTextBoxHeight), string.Format("Day {0}:{1}", Service.Game.CurrentDay, CurrentPhase?.TimeOfDay.ToString() ?? "")))
                {
                    currentPhaseDebugging = CurrentPhase;

                    if (CurrentCharacterSeenListSelected != null && !currentPhaseDebugging.CharacterSeenMap.ContainsKey(CurrentCharacterSeenListSelected))
                    {
                        CurrentCharacterSeenListSelected = null;
                    }
                }
            }
            vPosition.y += iButtonHeight;

            vPosition.y += (PhaseHistory.Count * iButtonHeight);

            foreach(var entry in PhaseHistory)
            {
                GUI.Label(new Rect(vPosition.x, vPosition.y, 50, iTextBoxHeight), string.Format("Day {0}", entry.Key));

                int iAdditionalX = 50;
                foreach(var p in entry.Value)
                {
                    if (currentPhaseDebugging != null && currentPhaseDebugging == p)
                    {
                        GUI.Box(new Rect(vPosition.x + iAdditionalX - 2, vPosition.y - 2, 54, iTextBoxHeight + 4), "", selectedStyle);
                    }
                    if (GUI.Button(new Rect(vPosition.x + iAdditionalX, vPosition.y, 50, iTextBoxHeight), p.TimeOfDay.ToString()))
                    {
                        currentPhaseDebugging = p;
                        if (CurrentCharacterSeenListSelected != null && !currentPhaseDebugging.CharacterSeenMap.ContainsKey(CurrentCharacterSeenListSelected))
                        {
                            CurrentCharacterSeenListSelected = null;
                        }
                    }

                    iAdditionalX += 54;
                }

                vPosition.y -= iButtonHeight;
            }
        }
        GUI.EndScrollView();

        if (currentPhaseDebugging == null)
        {
            GUI.Label(new Rect(230, 40, 500, 32), "No Phase Selected.");
        }
        else
        {
            // Selected phase details
            vCharacterPickerPosition = GUI.BeginScrollView(new Rect(225, 35, 160, 690), vCharacterPickerPosition, new Rect(0, 0, 150, 1000));
            {
                Vector2 vPosition = new Vector2(5, 5);

                foreach(var c in currentPhaseDebugging.CharacterSeenMap)
                {
                    if (CurrentCharacterSeenListSelected != null && CurrentCharacterSeenListSelected == c.Key)
                    {
                        GUI.Box(new Rect(vPosition.x - 2, vPosition.y - 2, 144, iTextBoxHeight + 4), "", selectedStyle);
                    }
                    if (GUI.Button(new Rect(vPosition.x, vPosition.y, 140, iTextBoxHeight), string.Format("[{0}] {1}", c.Key.Index, c.Key.Name)))
                    {
                        CurrentCharacterSeenListSelected = c.Key;
                    }
                    vPosition.y += iButtonHeight;
                }
            }
            GUI.EndScrollView();

            vPickedCharacterDetailsPosition = GUI.BeginScrollView(new Rect(425, 35, 200, 690), vPickedCharacterDetailsPosition, new Rect(0, 0, 200, 1500));
            {
                if (CurrentCharacterSeenListSelected != null)
                {
                    Vector2 vPosition = new Vector2(5, 5);

                    GUI.Label(new Rect(vPosition.x, vPosition.y, 200, iTextBoxHeight), string.Format("This phase, {0} saw:", CurrentCharacterSeenListSelected.Name));
                    vPosition.y += iTextHeight;

                    foreach (var c in currentPhaseDebugging.CharacterSeenMap[CurrentCharacterSeenListSelected])
                    {
                        if (c.IsWerewolf)
                        {
                            GUI.contentColor = new Color(1.0f, 0.5f, 0.5f);
                        }

                        GUI.Label(new Rect(vPosition.x, vPosition.y, 200, iTextBoxHeight), string.Format("[{0}] {1}", c.Index, c.Name));
                        vPosition.y += iTextHeight;
                        GUI.contentColor = Color.white;
                    }

                    vPosition.y += iTextHeight * 2;
                    GUI.Label(new Rect(vPosition.x, vPosition.y, 200, iTextBoxHeight), string.Format("This phase, {0} did:", CurrentCharacterSeenListSelected.Name));
                    vPosition.y += iTextHeight;

                    if (currentPhaseDebugging.CharacterTasks.ContainsKey(CurrentCharacterSeenListSelected))
                    {
                        foreach (var t in currentPhaseDebugging.CharacterTasks[CurrentCharacterSeenListSelected])
                        {
                            GUI.Label(new Rect(vPosition.x, vPosition.y, 200, iTextBoxHeight), string.Format("{0} at Loc {1}", t.Type.ToString(), t.Location));
                            vPosition.y += iTextHeight;
                        }
                    }
                }
            }
            GUI.EndScrollView();

            // General info
            {
                Vector2 vPosition = new Vector2(640, 40);
                Character ww = Service.Population.GetWerewolf();

                GUI.Label(new Rect(vPosition.x, vPosition.y, 200, iTextBoxHeight), string.Format("Who saw {0}?", ww.Name));
                vPosition.y += iTextHeight;

                foreach(var c in currentPhaseDebugging.CharacterSeenMap)
                {
                    if(c.Value.Contains(ww))
                    {
                        GUI.Label(new Rect(vPosition.x, vPosition.y, 200, iTextBoxHeight), string.Format("[{0}] {1}", c.Key.Index, c.Key.Name));
                        vPosition.y += iTextHeight;
                    }
                }
            }
        }
    }

    void RenderPhase(Phase p)
    {

    }
#endif
#endregion
}
