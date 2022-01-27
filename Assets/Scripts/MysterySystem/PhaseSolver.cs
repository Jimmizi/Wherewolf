using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseSolver : MonoBehaviour
{
    public SortedDictionary<int, List<Phase>> PhaseHistory = new SortedDictionary<int, List<Phase>>();

    public Phase CurrentPhase;
    public bool IsGeneratingAPhase;

    private int iLastPlaceSomeoneWasMurdered = -1;

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
            Service.Game.DisplayDebug = false;
            bShowDebug = !bShowDebug;
        }
        if(bShowDebug)
        {
            if (CurrentCharacterSelected != null)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    Character lastLoopCharacter = null;
                    foreach (var c in currentPhaseDebugging.CharacterSeenMap)
                    {
                        if (CurrentCharacterSelected == c.Key)
                        {
                            if (lastLoopCharacter != null)
                            {
                                CurrentCharacterSelected = lastLoopCharacter;
                            }
                            break;
                        }

                        lastLoopCharacter = c.Key;
                    }
                }
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    bool bNextLoopAssigns = false;
                    foreach (var c in currentPhaseDebugging.CharacterSeenMap)
                    {
                        if (bNextLoopAssigns)
                        {
                            CurrentCharacterSelected = c.Key;
                            break;
                        }

                        if (CurrentCharacterSelected == c.Key)
                        {
                            bNextLoopAssigns = true;
                        }
                    }
                }
            }
        }
        //if (Input.GetKeyDown(KeyCode.F5) && bShowDebug && !IsGeneratingAPhase)
        //{
        //    if (Service.Population.CharacterCreationDone)
        //    {
        //        bool incrementDay = lastDebugGeneratedTod == WerewolfGame.TOD.Night && !firstTimeGeneratingPhase;
        //        firstTimeGeneratingPhase = false;

        //        if (incrementDay)
        //        {
        //            Service.Game.CurrentDay++;
        //        }

        //        lastDebugGeneratedTod = lastDebugGeneratedTod == WerewolfGame.TOD.Day ? WerewolfGame.TOD.Night : WerewolfGame.TOD.Day;
        //        StartCoroutine(GeneratePhase(lastDebugGeneratedTod));
        //    }
        //}
#endif
    }

    // Public Access

    public bool CanGeneratePhase() => Service.Game.CurrentDay < 20 && !IsGeneratingAPhase;
    public void StartPhaseGeneration() => StartCoroutine(GeneratePhase(Service.Game.CurrentTimeOfDay));

    public Character GetVictimFromDay(int iDay)
    {
        if(!PhaseHistory.ContainsKey(iDay))
        {
            return null;
        }

        foreach(var p in PhaseHistory[iDay])
        {
            if(p.Victim == null)
            {
                continue;
            }

            return p.Victim;
        }

        return null;
    }

    public void TryKillOffCurrentVictim()
    {
        if(CurrentPhase != null && CurrentPhase.Victim != null)
        {
            CurrentPhase.Victim.IsAlive = false;
            CurrentPhase.Victim.IsVictim = false;
            CurrentPhase.Victim.DeathAnnounced = false;

            Service.Population.iNumberOfCharactersDead++;
        }
    }

    // Private functions

    IEnumerator GeneratePhase(WerewolfGame.TOD eTod)
    {
        IsGeneratingAPhase = true;

        if (!PhaseHistory.ContainsKey(Service.Game.CurrentDay))
        {
            PhaseHistory.Add(Service.Game.CurrentDay, new List<Phase>());
        }

        bool bJustMurderedNpc = CurrentPhase != null && CurrentPhase.Victim != null;
        bool bShouldGenerateVictim = !bJustMurderedNpc && ShouldGenerateVictim();
        Character ww = Service.Population.GetWerewolf();

        PhaseHistory[Service.Game.CurrentDay].Add(new Phase(eTod));
        CurrentPhase = PhaseHistory[Service.Game.CurrentDay][PhaseHistory[Service.Game.CurrentDay].Count - 1];

        bool bWerewolfReemergeAfterHiding = false;
        if(eTod == WerewolfGame.TOD.Day)
        {
            // Since it's now day, check last nights tasks
            if(ww.TaskSchedule.NightTasks.Count == 1 && ww.TaskSchedule.NightTasks[0].Type == Task.TaskType.Sleep)
            {
                bWerewolfReemergeAfterHiding = true;
            }
        }
        else if (eTod == WerewolfGame.TOD.Night)
        {
            // Since it's now night, check last days tasks
            if (ww.TaskSchedule.DayTasks.Count == 1 && ww.TaskSchedule.DayTasks[0].Type == Task.TaskType.Sleep)
            {
                bWerewolfReemergeAfterHiding = true;
            }
        }

        // 1) Werewolf finds a character to kill
        if (bShouldGenerateVictim)
        {
            Character victim = GetBestCharacterForVictim(eTod);
            int iLocation = GetBestLocationForVictimDeath(eTod, victim);

            victim.IsVictim = true;
            victim.DeathTimeOfDay = eTod;
            victim.DeathDay = Service.Game.CurrentDay;
            victim.DeathLocation = iLocation;

            CurrentPhase.Victim = victim;

            GenerateWerewolfAndVictimTasks(eTod, victim, iLocation);
            iLastPlaceSomeoneWasMurdered = iLocation;
        }
        else if(bJustMurderedNpc && Service.Config.WerewolfDisappearsAfterMurder)
        {
            ww.TaskSchedule.Clear();

            if(eTod == WerewolfGame.TOD.Day)
            {
                ww.TaskSchedule.DayTasks.Add(new Task(ww, Task.TaskType.Sleep));
            }
            else if (eTod == WerewolfGame.TOD.Night)
            {
                ww.TaskSchedule.NightTasks.Add(new Task(ww, Task.TaskType.Sleep));
            }
        }
        else
        {
            GenerateWerewolfAndVictimTasks(eTod, 
                iWerewolfLastMurderLoc: bWerewolfReemergeAfterHiding ? iLastPlaceSomeoneWasMurdered : -1);
        }

        // Yield for about a frame (a frame at 60fps is 0.016666)
        yield return new WaitForSeconds(0.02f);

        // 2) Next up, we randomise locations for characters doing idles and wanders
        //  By randomise we either give them a completely random location if they don't already have one
        //  but if they do have one, we just alter it to move to an adjacent tile

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

            CurrentPhase.CharacterSeenMap.Add(c, CalculateSeenCharactersDuringPhase(eTod, c));
            
            CurrentPhase.CharacterTasks.Add(c, new List<Task>());
            CurrentPhase.CharacterTasks[c].AddRange(eTod == WerewolfGame.TOD.Day ? c.TaskSchedule.DayTasks : c.TaskSchedule.NightTasks);

            yield return new WaitForSeconds(0.02f);
        }

        // 4) Calculate what characters they saw passing by (only if that character isn't present in the character seen map

        foreach (var c in Service.Population.ActiveCharacters)
        {
            if (!c.IsAlive)
            {
                continue;
            }

            CurrentPhase.CharacterSawPassingByMap.Add(c, CalculateSawCharactersPassingByDuringPhase(eTod, c));

            yield return new WaitForSeconds(0.02f);
        }

        // 5) Done

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

        // If the last phase was daytime (this is now going to be the night time)
        if (CurrentPhase != null && CurrentPhase.TimeOfDay == WerewolfGame.TOD.Day)
        {
            // If the day time didn't produce a victim, do it now
            return CurrentPhase.Victim == null;
        }

        // Otherwise it's either the first day time, or a new day in which case we have a random chance of striking in the day time
        //  If it fails here, we will generate during the night above
        return UnityEngine.Random.Range(0.0f, 100.0f) < 50.0f;
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
    void GenerateWerewolfAndVictimTasks(WerewolfGame.TOD eTod, Character victim = null, int iLocation = -1, int iWerewolfLastMurderLoc = -1)
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

        int iFirstTaskLocation = iLocation > -1 ? Service.Population.GetRandomAdjacentLocation(iLocation) : -1;

        // Have the werewolf re-emerge the phase after hiding after a murder at the place where they did the murder
        if(victim == null && iWerewolfLastMurderLoc != -1)
        {
            iFirstTaskLocation = iWerewolfLastMurderLoc;
        }

        AddTask(eFirstTaskType, iFirstTaskLocation);
        AddTask(eSecondTaskType, iLocation);
    }

    void RandomiseCharacterTaskLocations(WerewolfGame.TOD eTod, Character c)
    {
        void RandomiseTaskLocations(List<Task> tasks)
        {
            foreach (var t in tasks)
            {
                // Generally do not randomise a task (create a sense of characters sticking to areas)
                if (UnityEngine.Random.Range(0.0f, 100.0f) < 60.0f)
                {
                    continue;
                }

                if(t.Type == Task.TaskType.WanderArea || t.Type == Task.TaskType.Idle)
                {
                    t.Location = Service.Population.GetRandomAdjacentLocation(t.Location, true);
                    t.UpdatePosition();
                }
            }
        }

        RandomiseTaskLocations(eTod == WerewolfGame.TOD.Day ? c.TaskSchedule.DayTasks : c.TaskSchedule.NightTasks);
    }

    List<Tuple<Character, int>> CalculateSeenCharactersDuringPhase(WerewolfGame.TOD eTod, Character character)
    {
        List<Tuple<Character, int>> otherCharactersSawDuringThisPhase = new List<Tuple<Character, int>>();
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

                if (!c.IsAlive)
                {
                    continue;
                }

                foreach (var t in (eTod == WerewolfGame.TOD.Day ? c.TaskSchedule.DayTasks : c.TaskSchedule.NightTasks))
                {
                    if(t.Type != Task.TaskType.Sleep && iLocationsIn.Contains(t.Location))
                    {
                        otherCharactersSawDuringThisPhase.Add(new Tuple<Character, int>(c, t.Location));
                        break;
                    }
                }
            }
        }

        return otherCharactersSawDuringThisPhase;
    }

    List<Tuple<Character, int>> CalculateSawCharactersPassingByDuringPhase(WerewolfGame.TOD eTod, Character character)
    {
        List<Tuple<Character, int>> otherCharactersSawPassByDuringPhase = new List<Tuple<Character, int>>();

        List<int> iLocationsIn = new List<int>();
        foreach (var t in (eTod == WerewolfGame.TOD.Day ? character.TaskSchedule.DayTasks : character.TaskSchedule.NightTasks))
        {
            // Wouldn't be able to see other people when sleeping
            if (t.Type != Task.TaskType.Sleep)
            {
                iLocationsIn.Add(t.Location);
            }
        }

        foreach (var c in Service.Population.ActiveCharacters)
        {
            if (c == character)
            {
                continue;
            }

            if (!c.IsAlive)
            {
                continue;
            }

            bool bAlreadySeen = false;
            foreach(var charSeen in CurrentPhase.CharacterSeenMap[character])
            {
                if(charSeen.Item1 == c)
                {
                    bAlreadySeen = true;
                    break;
                }
            }

            // If we've fully seen this character in a location already, no need to generate passing by records
            //  as knowing what location they're in is stronger evidence then only knowing they passed by
            if(bAlreadySeen)
            {
                continue;
            }

            int iLocSeenIn = 0;
            if (c.WillTravelThroughLocationDuringTasks(eTod, iLocationsIn, out iLocSeenIn))
            {
                otherCharactersSawPassByDuringPhase.Add(new Tuple<Character, int>(c, iLocSeenIn));
                break;
            }
        }

        return otherCharactersSawPassByDuringPhase;
    }


    #region DEBUG
#if UNITY_EDITOR

    const int iTextHeight = 16;
    const int iButtonHeight = 28;
    const int iTextBoxHeight = 24;
    Phase currentPhaseDebugging;
    Character CurrentCharacterSelected;
    int iCurrentDayDebugging;

    WerewolfGame.TOD lastDebugGeneratedTod = WerewolfGame.TOD.Night;

    Vector2 vScrollPositionHistory = new Vector2();
    Vector2 vCharacterPickerPosition = new Vector2();
    Vector2 vPickedCharacterDetailsPosition = new Vector2();
    
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

        float fDebugHeight = Screen.height - 60;

        GUI.Box(new Rect(30, 30, 1250, fDebugHeight + 10), "");
        GUI.Label(new Rect(6, 0, 500, 24), "Phase Debug");
        //GUI.Label(new Rect(206, 0, 800, 24), string.Format("F5 - Debug generate phase{0}", IsGeneratingAPhase ? " (Currently generating)" : ""));

        // List of history + current at top
        // Panel for general info (werewolf and victim)
        // Panel for who saw who

        // Phase history picker
        vScrollPositionHistory = GUI.BeginScrollView(new Rect(35, 35, 190, fDebugHeight), vScrollPositionHistory, new Rect(0, 0, 170, 1500));
        {
            Vector2 vPosition = new Vector2(5, 5);

            // Selecting the current phase
            //if (currentPhaseDebugging != null && currentPhaseDebugging == CurrentPhase)
            //{
            //    GUI.Box(new Rect(vPosition.x + 50 - 2, vPosition.y - 2, 104, iTextBoxHeight + 4), "", selectedStyle);
            //}
            //if (CurrentPhase != null)
            //{
            //    GUI.Label(new Rect(vPosition.x, vPosition.y, 50, iTextBoxHeight), "Curr:");
            //    if (GUI.Button(new Rect(vPosition.x + 50, vPosition.y, 100, iTextBoxHeight), string.Format("Day {0}:{1}", Service.Game.CurrentDay, CurrentPhase?.TimeOfDay.ToString() ?? "")))
            //    {
            //        currentPhaseDebugging = CurrentPhase;

            //        if (CurrentCharacterSeenListSelected != null && !currentPhaseDebugging.CharacterSeenMap.ContainsKey(CurrentCharacterSeenListSelected))
            //        {
            //            CurrentCharacterSeenListSelected = null;
            //        }
            //    }
            //}
            //vPosition.y += iButtonHeight;

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
                        iCurrentDayDebugging = entry.Key;
                        if (CurrentCharacterSelected != null && !currentPhaseDebugging.CharacterSeenMap.ContainsKey(CurrentCharacterSelected))
                        {
                            CurrentCharacterSelected = null;
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
            vCharacterPickerPosition = GUI.BeginScrollView(new Rect(225, 35, 160, fDebugHeight), vCharacterPickerPosition, new Rect(0, 0, 150, 1000));
            {
                Vector2 vPosition = new Vector2(5, 5);

                foreach(var c in currentPhaseDebugging.CharacterSeenMap)
                {
                    if (CurrentCharacterSelected != null && CurrentCharacterSelected == c.Key)
                    {
                        GUI.Box(new Rect(vPosition.x - 2, vPosition.y - 2, 144, iTextBoxHeight + 4), "", selectedStyle);
                    }
                    if (GUI.Button(new Rect(vPosition.x, vPosition.y, 140, iTextBoxHeight), string.Format("[{0}] {1}", c.Key.Index, c.Key.Name)))
                    {
                        CurrentCharacterSelected = c.Key;
                    }
                    vPosition.y += iButtonHeight;
                }
            }
            GUI.EndScrollView();

            vPickedCharacterDetailsPosition = GUI.BeginScrollView(new Rect(425, 35, 400, fDebugHeight), vPickedCharacterDetailsPosition, new Rect(0, 0, 500, 2500));
            {
                if (CurrentCharacterSelected != null)
                {
                    Vector2 vPosition = new Vector2(5, 5);

                    GUI.Label(new Rect(vPosition.x, vPosition.y, 200, iTextBoxHeight), string.Format("This phase, {0} saw:", CurrentCharacterSelected.Name));
                    vPosition.y += iTextHeight;

                    foreach (var c in currentPhaseDebugging.CharacterSeenMap[CurrentCharacterSelected])
                    {
                        if (c.Item1.IsWerewolf)
                        {
                            GUI.contentColor = new Color(1.0f, 0.5f, 0.5f);
                        }

                        GUI.Label(new Rect(vPosition.x, vPosition.y, 200, iTextBoxHeight), string.Format("[{0}] {1} in Loc {2}", c.Item1.Index, c.Item1.Name, c.Item2));
                        vPosition.y += iTextHeight;
                        GUI.contentColor = Color.white;
                    }

                    vPosition.y += iTextHeight;
                    GUI.Label(new Rect(vPosition.x, vPosition.y, 200, iTextBoxHeight), "and saw these passing by:");
                    vPosition.y += iTextHeight;

                    foreach (var c in currentPhaseDebugging.CharacterSawPassingByMap[CurrentCharacterSelected])
                    {
                        if (c.Item1.IsWerewolf)
                        {
                            GUI.contentColor = new Color(1.0f, 0.5f, 0.5f);
                        }

                        GUI.Label(new Rect(vPosition.x, vPosition.y, 200, iTextBoxHeight), string.Format("[{0}] {1} in Loc {2}", c.Item1.Index, c.Item1.Name, c.Item2));
                        vPosition.y += iTextHeight;
                        GUI.contentColor = Color.white;
                    }

                    vPosition.y += iTextHeight * 2;
                    GUI.Label(new Rect(vPosition.x, vPosition.y, 200, iTextBoxHeight), string.Format("This phase, {0} did:", CurrentCharacterSelected.Name));
                    vPosition.y += iTextHeight;

                    if (currentPhaseDebugging.CharacterTasks.ContainsKey(CurrentCharacterSelected))
                    {
                        foreach (var t in currentPhaseDebugging.CharacterTasks[CurrentCharacterSelected])
                        {
                            GUI.Label(new Rect(vPosition.x, vPosition.y, 200, iTextBoxHeight), string.Format("{0} at Loc {1}", t.Type.ToString(), t.Location));
                            vPosition.y += iTextHeight;
                        }
                    }

                    vPosition.y += iTextHeight;
                    GUI.Label(new Rect(vPosition.x, vPosition.y, 200, iTextBoxHeight), "---------------");
                    vPosition.y += iTextHeight * 2;

                    GUI.Label(new Rect(vPosition.x, vPosition.y, 200, iTextBoxHeight), "Can give these clues:");
                    vPosition.y += iTextHeight;

                    foreach(var clue in currentPhaseDebugging.CharacterCluesToGive[CurrentCharacterSelected])
                    {
                        // Type
                        GUI.contentColor = Color.yellow;
                        GUI.Label(new Rect(vPosition.x, vPosition.y, 150, iTextBoxHeight), clue.Type.ToString());

                        // True/false
                        GUI.contentColor = clue.IsTruth ? new Color(0.1f, 0.8f, 0.1f) : new Color(0.8f, 0.1f, 0.1f);
                        GUI.Label(new Rect(vPosition.x + 200, vPosition.y, 150, iTextBoxHeight), 
                            string.Format("({0})", clue.IsTruth ? "Is the truth" : "Is a lie"));

                        vPosition.y += iTextHeight;
                        GUI.contentColor = Color.white;

                        // Extra info
                        if(clue.RelatesToCharacter.IsWerewolf)
                        {
                            GUI.contentColor = new Color(1.0f, 0.5f, 0.5f);
                        }
                        GUI.Label(new Rect(vPosition.x, vPosition.y, 200, iTextBoxHeight), 
                            string.Format("Subject: [{0}] {1}", clue.RelatesToCharacter.Index, clue.RelatesToCharacter.Name));
                        GUI.contentColor = Color.white;

                        GUI.Label(new Rect(vPosition.x + 200, vPosition.y, 200, iTextBoxHeight),
                             string.Format("LocSeenIn: {0}", clue.LocationSeenIn));

                        vPosition.y += iTextHeight;

                        if(clue.Type == ClueObject.ClueType.VisualFromGhost)
                        {
                            GUI.Label(new Rect(vPosition.x, vPosition.y, 350, iTextBoxHeight),
                                string.Format("Ghost Descriptor Type: {0}", clue.GhostGivenClueType.ToString()));
                            vPosition.y += iTextHeight;
                        }

                        if (clue.Emotes.Count > 0)
                        {
                            GUI.Label(new Rect(vPosition.x, vPosition.y, 200, iTextBoxHeight), "Emotes string:");
                            vPosition.y += iTextHeight;

                            GUI.contentColor = new Color(0.2f, 0.7f, 1.0f);
                            foreach (var emote in clue.Emotes)
                            {
                                GUI.Label(new Rect(vPosition.x, vPosition.y, 300, iTextBoxHeight), emote.SubType.ToString());
                                vPosition.y += iTextHeight;
                            }
                            GUI.contentColor = Color.white;
                        }

                        vPosition.y += iTextHeight;
                    }

                }
            }
            GUI.EndScrollView();

            // General info
            {
                Vector2 vPosition = new Vector2(840, 40);
                Character ww = Service.Population.GetWerewolf();

                GUI.Label(new Rect(vPosition.x, vPosition.y, 200, iTextBoxHeight), string.Format("Who saw {0}?", ww.Name));
                vPosition.y += iTextHeight;

                foreach(var c in currentPhaseDebugging.CharacterSeenMap)
                {
                    foreach(var tup in c.Value)
                    {
                        if(tup.Item1 == ww)
                        {
                            GUI.Label(new Rect(vPosition.x, vPosition.y, 200, iTextBoxHeight), string.Format("[{0}] {1}", c.Key.Index, c.Key.Name));
                            vPosition.y += iTextHeight;
                        }
                    }
                }

                vPosition.y += iTextHeight;
                GUI.Label(new Rect(vPosition.x, vPosition.y, 200, iTextBoxHeight), "---------------");
                vPosition.y += iTextHeight * 2;

                Character charToKill = null;

                foreach (var c in Service.Population.ActiveCharacters)
                {
                    if (c.DeathTimeOfDay == currentPhaseDebugging.TimeOfDay
                        && c.DeathDay == iCurrentDayDebugging)
                    {
                        charToKill = c;
                        break;
                    }
                }

                GUI.Label(new Rect(vPosition.x, vPosition.y, 200, iTextBoxHeight), "This phase, werewolf did:");
                vPosition.y += iTextHeight;

                if (currentPhaseDebugging.CharacterTasks.ContainsKey(ww))
                {
                    foreach (var t in currentPhaseDebugging.CharacterTasks[ww])
                    {
                        GUI.Label(new Rect(vPosition.x, vPosition.y, 200, iTextBoxHeight), string.Format("{0} at Loc {1}", t.Type.ToString(), t.Location));
                        vPosition.y += iTextHeight;
                    }
                }

                vPosition.y += iTextHeight;

                if (charToKill != null)
                {
                    GUI.contentColor = new Color(1.0f, 0.5f, 0.5f);
                    GUI.Label(new Rect(vPosition.x, vPosition.y, 200, iTextBoxHeight), string.Format("[{0}] {1} is victim!", charToKill.Index, charToKill.Name));
                    vPosition.y += iTextHeight;

                    if (currentPhaseDebugging.CharacterTasks.ContainsKey(charToKill))
                    {
                        foreach (var t in currentPhaseDebugging.CharacterTasks[charToKill])
                        {
                            GUI.Label(new Rect(vPosition.x, vPosition.y, 200, iTextBoxHeight), string.Format("{0} at Loc {1}", t.Type.ToString(), t.Location));
                            vPosition.y += iTextHeight;
                        }
                    }

                    GUI.contentColor = Color.white;
                }
            }
            // General info 2
            {
                Vector2 vPosition = new Vector2(1050, 40);

                Character ww = Service.Population.GetWerewolf();
                List<Character> inactiveCharacters = new List<Character>();

                GUI.Label(new Rect(vPosition.x, vPosition.y, 200, iTextBoxHeight), "Active Characters:");
                vPosition.y += iTextHeight;
                foreach (var c in Service.Population.ActiveCharacters)
                {
                    if(!c.IsAlive)
                    {
                        inactiveCharacters.Add(c);
                        continue;
                    }

                    if (currentPhaseDebugging.TimeOfDay == WerewolfGame.TOD.Day)
                    {
                        if(c.CurrentTask != null && c.CurrentTask.Type == Task.TaskType.Sleep)
                        {
                            inactiveCharacters.Add(c);
                            continue;
                        }
                    }
                    else
                    {
                        if (c.TaskSchedule.NightTasks.Count > 0)
                        {
                            if(c.TaskSchedule.NightTasks[0].Type == Task.TaskType.Sleep)
                            {
                                inactiveCharacters.Add(c);
                                continue;
                            }
                        }
                    }

                    GUI.Label(new Rect(vPosition.x, vPosition.y, 200, iTextBoxHeight), string.Format("[{0}] {1}", c.Index, c.Name));
                    vPosition.y += iTextHeight;
                }

                vPosition.y += iTextHeight;
                GUI.Label(new Rect(vPosition.x, vPosition.y, 200, iTextBoxHeight), "Inactive Characters:");
                vPosition.y += iTextHeight;
                foreach (var c in inactiveCharacters)
                {
                    if(!c.IsAlive)
                    {
                        continue;
                    }

                    GUI.Label(new Rect(vPosition.x, vPosition.y, 200, iTextBoxHeight), string.Format("[{0}] {1}", c.Index, c.Name));
                    vPosition.y += iTextHeight;
                }

                vPosition.y += iTextHeight;
                GUI.Label(new Rect(vPosition.x, vPosition.y, 200, iTextBoxHeight), "Dead Characters:");
                vPosition.y += iTextHeight;
                foreach (var c in inactiveCharacters)
                {
                    if (c.IsAlive)
                    {
                        continue;
                    }

                    if(c.DeathDay > iCurrentDayDebugging)
                    {
                        continue;
                    }

                    if(c.DeathDay == iCurrentDayDebugging)
                    {
                        // If they died in the day, show them in the night phase as dead
                        if(c.DeathTimeOfDay == WerewolfGame.TOD.Day
                            && currentPhaseDebugging.TimeOfDay == WerewolfGame.TOD.Day)
                        {
                            continue;
                        }

                        // if they died at night, don't show until you select a further day
                        if (c.DeathTimeOfDay == WerewolfGame.TOD.Night)
                        {
                            continue;
                        }
                    }

                    GUI.Label(new Rect(vPosition.x, vPosition.y, 200, iTextBoxHeight), string.Format("[{0}] {1}", c.Index, c.Name));
                    vPosition.y += iTextHeight;
                }
            }
        }
    }

#endif
#endregion
}
