using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationManager : MonoBehaviour
{
    // Public vars
    public List<Character> ActiveCharacters = new List<Character>();

    // Private vars
    private int werewolfIndex = -1;
    private Vector2 vScrollPosition = new Vector2();
    private Vector2 vScrollPositionForCounts = new Vector2();
    private Vector2 vScrollPositionForInfo = new Vector2();
    private bool bShowDebug = false;
    private bool bDoneCharacterGeneration = false;

    // API

    // Get the character for the werewolf
    public Character GetWerewolf() => werewolfIndex > -1 ? ActiveCharacters[werewolfIndex] : null;


    // Internal Functions

    private void Awake()
    {
        Service.Population = this;
    }

    void Start()
    {
        InitialisePopulation();
    }

    void Update()
    {
#if UNITY_EDITOR
        if(Input.GetKeyDown(KeyCode.F1))
        {
            bShowDebug = !bShowDebug;
        }

        if(bShowDebug && Input.GetKeyDown(KeyCode.F5))
        {
            foreach(var c in ActiveCharacters)
            {
                c.Home.Owner = null;
                c.Home = null;
            }

            ActiveCharacters.Clear();
            subTypeNumberCounts.Clear();
            werewolfIndex = -1;
            InitialisePopulation();
        }
#endif
    }

#if UNITY_EDITOR

    const int iTextHeight = 16;
    const int iTextBoxWidth = 250;
    const int iTextBoxHeight = 24;
    const int iPaddingBetweenCharacters = 24;
    const int iColumnWidth = iTextBoxWidth + iPaddingBetweenCharacters;
    SortedDictionary<Emote.EmoteType, SortedDictionary<Emote.EmoteSubType, int>> subTypeNumberCounts = new SortedDictionary<Emote.EmoteType, SortedDictionary<Emote.EmoteSubType, int>>();

    private void OnGUI()
    {
        if (!bShowDebug) 
        { 
            return; 
        }

        GUI.Box(new Rect(15, 15, 1050, 700), "");
        GUI.Label(new Rect(6, 0, 200, 24), "Population Debug");

        GUI.Label(new Rect(206, 0, 400, 24), "F5 - Reroll population");

        if (ActiveCharacters.Count > 0)
        {
            vScrollPosition = GUI.BeginScrollView(new Rect(20, 20, 590, 690), vScrollPosition, new Rect(0, 0, 590, 2500));
            {
                Vector2 vPosition = new Vector2(5, 5);
                RenderCharacterDebug(ref vPosition, GetWerewolf(), werewolfIndex);

                for(int i = 0; i < ActiveCharacters.Count; ++i)
                {
                    if (i == werewolfIndex)
                    {
                        continue;
                    }

                    if(i == 10)
                    {
                        vPosition = new Vector2(5 + iColumnWidth, 5);
                    }

                    RenderCharacterDebug(ref vPosition, ActiveCharacters[i], i);
                }
            }
            GUI.EndScrollView();

            // First time tally up the numbers
            if (subTypeNumberCounts.Count == 0 && bDoneCharacterGeneration)
            {
                foreach (var c in ActiveCharacters)
                {
                    foreach (var emoteList in c.Descriptors.Values)
                    {
                        foreach (var emote in emoteList)
                        {
                            if(!subTypeNumberCounts.ContainsKey(emote.Type))
                            {
                                subTypeNumberCounts.Add(emote.Type, new SortedDictionary<Emote.EmoteSubType, int>());
                            }

                            if (!subTypeNumberCounts[emote.Type].ContainsKey(emote.SubType))
                            {
                                subTypeNumberCounts[emote.Type].Add(emote.SubType, 0);
                            }

                            subTypeNumberCounts[emote.Type][emote.SubType]++;
                        }
                    }
                }
            }

            vScrollPositionForCounts = GUI.BeginScrollView(new Rect(610, 20, 250, 690), vScrollPositionForCounts, new Rect(0, 0, 200, 1200));
            {
                Vector2 vPosition = new Vector2(5, 5);
                Character ww = GetWerewolf();

                float fHeaderPosition = vPosition.y;
                vPosition.y += iTextHeight;

                int iNumMatches = 0;

                // Show which elements of other characters match against the werewolf
                for (int i = 0; i < ActiveCharacters.Count; ++i)
                {
                    if(i == werewolfIndex)
                    {
                        continue;
                    }

                    Character c = ActiveCharacters[i];

                    if(!c.IsAlive)
                    {
                        continue;
                    }

                    float fNamePosition = vPosition.y;
                    vPosition.y += iTextHeight;

                    bool bAnyMatch = false;

                    for (int t = 0; t < Character.DescriptorMax; ++t)
                    {
                        var desc = (Character.Descriptor)t;

                        if (c.DoDescriptorsMatch(ref ww, desc))
                        {
                            iNumMatches++;
                            bAnyMatch = true;

                            GUI.Label(new Rect(vPosition.x + 15, vPosition.y, 225, iTextBoxHeight), string.Format("Matching {0}", desc.ToString()));
                            vPosition.y += iTextHeight;
                        }
                    }

                    if (bAnyMatch)
                    {
                        GUI.Label(new Rect(vPosition.x, fNamePosition, 240, iTextBoxHeight), string.Format("[{0}] {1}", i, c.Name));
                        vPosition.y += iTextHeight;
                    }
                    else
                    {
                        vPosition.y = fNamePosition;
                    }
                }

                GUI.Label(new Rect(vPosition.x, fHeaderPosition, 250, iTextBoxHeight), string.Format("(Alive) {0} Matches to Werewolf", iNumMatches));


                foreach (var map in subTypeNumberCounts)
                {
                    GUI.Label(new Rect(vPosition.x, vPosition.y, 150, iTextBoxHeight), map.Key.ToString());
                    vPosition.y += iTextHeight;

                    foreach (var subType in map.Value)
                    {
                        string sKeyName = subType.Key.ToString();
                        string sSubTypeName = sKeyName.Substring(sKeyName.IndexOf("_") + 1);

                        GUI.Label(new Rect(vPosition.x + 15, vPosition.y, 250, iTextBoxHeight), string.Format("{0}: {1}", sSubTypeName, subType.Value));
                        vPosition.y += iTextHeight;
                    }

                    vPosition.y += iPaddingBetweenCharacters;
                }
            }
            GUI.EndScrollView();

            vScrollPositionForInfo = GUI.BeginScrollView(new Rect(860, 20, 190, 690), vScrollPositionForInfo, new Rect(0, 0, 200, 1200));
            {
                Vector2 vPosition = new Vector2(5, 5);

                int iNumAlive = 0;
                foreach(var c in ActiveCharacters)
                {
                    if(c.IsAlive && !c.IsWerewolf)
                    {
                        iNumAlive++;
                    }
                }

                GUI.Label(new Rect(vPosition.x, vPosition.y, 150, iTextBoxHeight), string.Format("Num Alive: {0}", iNumAlive));
            }
            GUI.EndScrollView();
        }
    }

    private void RenderCharacterDebug(ref Vector2 vPos, Character c, int iIndex)
    {
        string MakeStringFromDescriptor(string sLabel, Character.Descriptor eType)
        {
            string str = sLabel + ": ";
            foreach (var emote in c.Descriptors[eType])
            {
                str += emote.Name + " ";
            }
            return str;
        }

        GUI.Label(new Rect(vPos.x, vPos.y, 100, iTextBoxHeight), string.Format("[{0}] {1}", iIndex, c.Name));
        if (c.IsWerewolf)
        {
            GUI.Label(new Rect(vPos.x + 100, vPos.y, 75, iTextBoxHeight), "(Werewolf!)");
        }
        else
        {
            GUI.Label(new Rect(vPos.x + 100, vPos.y, 75, iTextBoxHeight), string.Format("(Alive: {0})", c.IsAlive ? "Y" : "N"));
        }
        vPos.y += iTextHeight;

        GUI.Label(new Rect(vPos.x + 16, vPos.y, iTextBoxWidth, iTextBoxHeight), MakeStringFromDescriptor("Hair", Character.Descriptor.Hair));
        vPos.y += iTextHeight;

        GUI.Label(new Rect(vPos.x + 16, vPos.y, iTextBoxWidth, iTextBoxHeight), MakeStringFromDescriptor("Facial", Character.Descriptor.Facial));
        vPos.y += iTextHeight;

        GUI.Label(new Rect(vPos.x + 16, vPos.y, iTextBoxWidth, iTextBoxHeight), MakeStringFromDescriptor("Occupation", Character.Descriptor.Occupation));
        vPos.y += iTextHeight;

        GUI.Label(new Rect(vPos.x + 16, vPos.y, iTextBoxWidth, iTextBoxHeight), MakeStringFromDescriptor("Clothing", Character.Descriptor.Clothing));
        vPos.y += iTextHeight;

        GUI.Label(new Rect(vPos.x + 16, vPos.y, iTextBoxWidth, iTextBoxHeight), string.Format("Home: {0}", c.Home ? c.Home.name : "NULL"));
        vPos.y += iTextHeight;

        if (!c.IsWerewolf)
        {
            GUI.contentColor = new Color(1.0f, 0.5f, 0.5f);
            GUI.Label(new Rect(vPos.x + 16, vPos.y, iColumnWidth / 2, iTextBoxHeight), "Day Tasks");
            GUI.Label(new Rect(vPos.x + 16 + iColumnWidth / 2, vPos.y, iColumnWidth / 2, iTextBoxHeight), "Night Tasks");

            GUI.contentColor = new Color(0.75f, 0.5f, 0.5f);

            vPos.y += iTextHeight;

            for (int i = 0; i < Math.Max(c.TaskSchedule.DayTasks.Count, c.TaskSchedule.NightTasks.Count); ++i)
            {
                if (i < c.TaskSchedule.DayTasks.Count)
                {
                    bool bIsActive = c.TaskSchedule.DayTasks[i] == c.CurrentTask;

                    GUI.Label(new Rect(vPos.x + 16, vPos.y, iColumnWidth / 2, iTextBoxHeight),
                        string.Format("{0}{1}{2}", bIsActive ? "> " : "", c.TaskSchedule.DayTasks[i].Type.ToString(), bIsActive ? " <" : ""));
                }

                if (i < c.TaskSchedule.NightTasks.Count)
                {
                    bool bIsActive = c.TaskSchedule.NightTasks[i] == c.CurrentTask;

                    GUI.Label(new Rect(vPos.x + 16 + iColumnWidth / 2, vPos.y, iColumnWidth / 2, iTextBoxHeight),
                        string.Format("{0}{1}{2}", bIsActive ? "> " : "", c.TaskSchedule.NightTasks[i].Type.ToString(), bIsActive ? " <" : ""));
                }

                vPos.y += iTextHeight;
            }

            GUI.Label(new Rect(vPos.x + 16, vPos.y, iColumnWidth / 2, iTextBoxHeight), 
                string.Format("TaskTimer: {0}/{1}", (c.CurrentTask != null ? c.CurrentTask.Timer.ToString() : "-"), 
                (c.CurrentTask != null ? c.CurrentTask.Duration.ToString() : "-")));
            vPos.y += iTextHeight;

            GUI.contentColor = Color.white;
        }

        vPos.y += iPaddingBetweenCharacters;
    }
#endif

    IEnumerator InitialisePopulationStaggered()
    {
        bDoneCharacterGeneration = false;

        // First generate the werewolf
        AddCharacter(null, isWerewolf: true);
        yield return new WaitForSeconds(0.04f);

        Character ww = GetWerewolf();
        var descriptorsToMatch = ww.GetRandomDescriptors(2);
        int iNumberOfMatches = 0;

        // Then generate the remainder of NPCs
        for (int i = 0; i < 19; ++i)
        {
            Character characterAdded = AddCharacter(descriptorsToMatch);
            yield return new WaitForSeconds(0.04f);

            // First character made copies two, and then passes one onto the next character made
            if(i == 0)
            {
                var werewolfDescriptors = descriptorsToMatch;

                // Some weird stuff to remove 
                int indexToRemove = UnityEngine.Random.Range(0, werewolfDescriptors.Keys.Count);
                var vDescriptors = new List<Character.Descriptor>();
                foreach(var a in werewolfDescriptors.Keys)
                {
                    vDescriptors.Add(a);
                }

                // For the first character, randomly remove one of the werewolf's descriptors so that another character can add the one remaining
                var descToRemove = vDescriptors[indexToRemove];
                werewolfDescriptors.Remove(descToRemove);
                vDescriptors.RemoveAt(indexToRemove);

                // Set to the remaining descriptor to begin with, so we can know when we've found one that isn't the ones we've copied from the werewolf
                Character.Descriptor descToGrabFromFirstCharacter = vDescriptors[0];
                while (descToGrabFromFirstCharacter == descToRemove || descToGrabFromFirstCharacter == vDescriptors[0])
                {
                    // Keep grabbing random descriptors until we've grabbed one of the two we didn't use of the werewolf's
                    descToGrabFromFirstCharacter = (Character.Descriptor)UnityEngine.Random.Range(0, Character.DescriptorMax);
                }

                // Then add in the second descriptor from this first made character to copy to the next
                descriptorsToMatch[descToGrabFromFirstCharacter] = characterAdded.GetDescriptors(descToGrabFromFirstCharacter);

            }
            else if(i == 1)
            {
                // For the third character we break the chain of copying werewolf descriptors
                descriptorsToMatch.Clear();
            }
            // Only allow up to 7 manual descriptor matches with the werewolf, so that there aren't too many (making it more difficult)
            //  this number can go up characters manage to randomly pick some
            else if(iNumberOfMatches < 8)
            {
                // If we're late into the character creation, and there is a low number of matches, make sure some matches get made
                bool bEmergencyCopyDescriptors = (i > 13 && iNumberOfMatches < 7);

                // From here on, randomly copy 1 or 2 descriptors on a 33% chance to do so

                if (UnityEngine.Random.Range(0, 101) < 33 || bEmergencyCopyDescriptors)
                {
                    var randomDescriptorsAmount = UnityEngine.Random.Range(1, 3);
                    descriptorsToMatch = characterAdded.GetRandomDescriptors(randomDescriptorsAmount);
                }
                else
                {
                    descriptorsToMatch.Clear();
                }
            }
            else
            {
                descriptorsToMatch.Clear();
            }

            for (int de = 0; de < Character.DescriptorMax; ++de)
            {
                var desc = (Character.Descriptor)de;

                if (characterAdded.DoDescriptorsMatch(ref ww, desc))
                {
                    iNumberOfMatches++;
                }
            }
        }

        // Randomise up the order in which we give the homes out
        List<int> randomGiveHomeOrder = new List<int>();
        while(randomGiveHomeOrder.Count < 20)
        {
            int index = UnityEngine.Random.Range(0, 20);
            if(!randomGiveHomeOrder.Contains(index))
            {
                randomGiveHomeOrder.Add(index);
            }
        }

        // Give the homes out
        int iCurrentHomeGiveIndex = 0;
        var allBuildings = FindObjectsOfType<Building>();
        foreach(var building in allBuildings)
        {
            if(!building.IsHome)
            {
                continue;
            }

            // Shouldn't have any assigned home owners at this point.
            // Indices should never be able to become out of bounds
            Debug.Assert(building.Owner == null);
            Debug.Assert(iCurrentHomeGiveIndex < randomGiveHomeOrder.Count);
            Debug.Assert(randomGiveHomeOrder[iCurrentHomeGiveIndex] < ActiveCharacters.Count);

            Character c = ActiveCharacters[randomGiveHomeOrder[iCurrentHomeGiveIndex++]];
            c.Home = building;
            building.Owner = c;
        }

        Debug.Assert(iCurrentHomeGiveIndex == ActiveCharacters.Count, "Not all characters have a home, only " + iCurrentHomeGiveIndex + " characters do.");

        // Give the NPCs schedules to follow
        foreach(var c in ActiveCharacters)
        {
            if(!c.IsWerewolf)
            {
                // Give them a schedule for day and night

                c.TaskSchedule.DayTasks = GenerateSchedule(c, true);
                c.TaskSchedule.NightTasks = GenerateSchedule(c, false);
            }
        }

        bDoneCharacterGeneration = true;
    }

    void InitialisePopulation()
    {
        StartCoroutine(InitialisePopulationStaggered());
    }

    Character AddCharacter(SortedDictionary<Character.Descriptor, List<Emote>> descriptorsToGive, bool isWerewolf = false)
    {
        ActiveCharacters.Add(GenerateCharacter(descriptorsToGive, isWerewolf));
        return ActiveCharacters[ActiveCharacters.Count - 1];
    }

    Character GenerateCharacter(SortedDictionary<Character.Descriptor, List<Emote>> descriptorsToGive, bool isWerewolf = false)
    {
        Character c = new Character();

        c.IsWerewolf = isWerewolf;
        if(isWerewolf)
        {
            Debug.Assert(werewolfIndex == -1, "Already have a werewolf, and trying to generate another.");
            werewolfIndex = ActiveCharacters.Count;
        }

        // Grab a random name.
        c.Name = Service.InfoManager.GetRandomName();

        for(int i = 0; i < Character.DescriptorMax; ++i)
        {
            c.Descriptors.Add((Character.Descriptor)i, new List<Emote>());
        }
        
        // Give descriptors
        if(descriptorsToGive != null)
        {
            foreach(var d in descriptorsToGive)
            {
                c.Descriptors[d.Key] = d.Value;
            }
        }

        // Grab one of each type of descriptor

        if (descriptorsToGive == null || !descriptorsToGive.ContainsKey(Character.Descriptor.Hair))
        {
            // Hair Style
            var hs = Service.InfoManager.GetRandomEmoteOfType(Emote.EmoteType.HairStyle);

            if (hs.SubType != Emote.EmoteSubType.HairStyle_Bald)
            {
                c.Descriptors[Character.Descriptor.Hair].Add(Service.InfoManager.GetRandomEmoteOfType(Emote.EmoteType.Color));
            }

            c.Descriptors[Character.Descriptor.Hair].Add(hs);
        }

        if (descriptorsToGive == null || !descriptorsToGive.ContainsKey(Character.Descriptor.Facial))
        {
            // Facial Feature
            c.Descriptors[Character.Descriptor.Facial].Add(Service.InfoManager.GetRandomEmoteOfType(Emote.EmoteType.FacialFeature));
        }

        if (descriptorsToGive == null || !descriptorsToGive.ContainsKey(Character.Descriptor.Occupation))
        {
            if (UnityEngine.Random.Range(0, 101) < Service.Config.CharacterHasOccupationChance)
            {
                // Occupation
                c.Descriptors[Character.Descriptor.Occupation].Add(Service.InfoManager.GetRandomEmoteOfType(Emote.EmoteType.Occupation));
            }
        }

        if (descriptorsToGive == null || !descriptorsToGive.ContainsKey(Character.Descriptor.Clothing))
        {
            // Clothing
            c.Descriptors[Character.Descriptor.Clothing].Add(Service.InfoManager.GetRandomEmoteOfType(Emote.EmoteType.Color));
            c.Descriptors[Character.Descriptor.Clothing].Add(Service.InfoManager.GetRandomEmoteOfType(Emote.EmoteType.Clothing));
        }

        return c;
    }

    List<Task> GenerateSchedule(Character c, bool bIsDaySchedule)
    {
        List<Task> schedule = new List<Task>();
        bool bHasOccupation = c.Descriptors[Character.Descriptor.Occupation].Count > 0;

        // Between 1 and 3 steps to a schedule. If they have a job, have at least 2 minimum
        int iScheduleSteps = UnityEngine.Random.Range(bHasOccupation ? 2 : 1, 4);

        bool bHasAddedJobTask = false;
        for(int i = 0; i < iScheduleSteps; ++i)
        {
            // In the day we can: Wander, Idle, Work
            // In the night we can: Wander, Idle, Sleep

            // If it's day, we've reached the end of the schedule and we haven't added a job task yet, do so
            if (bIsDaySchedule && bHasOccupation && i == iScheduleSteps - 1 && !bHasAddedJobTask)
            {
                schedule.Add(new Task(c, Task.TaskType.Work));
                bHasAddedJobTask = true;
            }
            else
            {
                var iRandTask = UnityEngine.Random.Range(0, 3);

                if (iRandTask == 2) // Work/Sleep
                {
                    if (bIsDaySchedule)
                    {
                        if (bHasOccupation)
                        {
                            schedule.Add(new Task(c, Task.TaskType.Work));
                            bHasAddedJobTask = true;
                        }
                    }
                    else
                    {
                        schedule.Add(new Task(c, Task.TaskType.Sleep));
                        
                        // If a sleep task is given, always end the schedule on this no matter how long the schedule should be
                        break;
                    }
                }
                else if (iRandTask == 1) // Idle
                {
                    schedule.Add(new Task(c, Task.TaskType.Idle));
                }
                else if (iRandTask == 0) // Wander
                {
                    schedule.Add(new Task(c, Task.TaskType.WanderArea));
                }
            }
        }

        return schedule;
    }

    public Vector3 GetHomePosition(Character c)
    {
        if(c.Home)
        {
            return c.Home.UseBuildingPosition;
        }

        Debug.LogError("Failed to find home for " + c.Name);
        return Vector3.zero;
    }

    public Vector3 GetWorkPosition(Character c)
    {
        Emote.EmoteSubType eBuildingType = c.GetWorkType();

#if UNITY_EDITOR
        bool bValidType = false;
        foreach(var emote in Service.InfoManager.EmoteMap[Emote.EmoteType.Occupation])
        {
            if(emote.SubType == eBuildingType)
            {
                bValidType = true;
                break;
            }
        }

        Debug.Assert(bValidType);
#endif

        var allBuildings = FindObjectsOfType<Building>();
        foreach(var building in allBuildings)
        {
            if(building.BuildingType == eBuildingType)
            {
                return building.UseBuildingPosition;
            }
        }

        Debug.LogError("Failed to find building of type " + eBuildingType.ToString() + " in the scene.");
        return Vector3.zero;
    }

}