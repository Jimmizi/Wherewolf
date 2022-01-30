using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character
{
    public enum Descriptor
    {
        Hair,
        Facial,
        Occupation,
        Clothing
    }

    public static int DescriptorMax => (int)Descriptor.Clothing + 1;
    public static Descriptor InvalidDescriptor => (Descriptor)(-1);

    // Vars

    public string Name;
    public SortedDictionary<Descriptor, List<Emote>> Descriptors = new SortedDictionary<Descriptor, List<Emote>>();
    public bool IsWerewolf;
    public bool IsVictim;
    public bool IsAlive = true;
    public bool DeathAnnounced = false;
    public bool HasGeneratedGhostClues = false;
    public bool ChosenForStakeTarget = false;
    public Schedule TaskSchedule = new Schedule();

    public int VoiceNumber;
    public void TriggerSpeechSound()
    {
        Service.Audio.PlaySpeech(VoiceNumber);
    }


    public Emote CurrentClothingCondition = null;

    // Pointer towards the last list of clues the character was able to give
    //public List<ClueObject> LastClueGroup;

    public ClueObject TryServeSpecificClueToPlayer(ClueObject.ClueType eType, Character relatesToCharacter = null)
    {
        if (IsAlive)
        {
            // When only looking for a type, we'll just grab that type from the pre-generated clues to give
            if (relatesToCharacter == null)
            {
                List<ClueObject> myCluesToGive = Service.PhaseSolve.CurrentPhase.CharacterCluesToGive[this];
                foreach (var clue in myCluesToGive)
                {
                    if (clue.Type == eType)
                    {
                        Debug.Log(string.Format("Requested specific type, but no character - {0} gave player {1} clue about {2}",
                            Name,
                            clue.Type.ToString(),
                            clue.RelatesToCharacter?.Name ?? "invalidrelatestocharacter"));

                        HasGivenAClueThisPhase = true;

                        return clue;
                    }
                }
            }
            // But when relating to a character, we'll see if the phase history includes this and then freshly generate a clue
            else
            {
                // Test to make sure we don't already have a clue ready for this character
                List<ClueObject> myCluesToGive = Service.PhaseSolve.CurrentPhase.CharacterCluesToGive[this];
                foreach(var clue in myCluesToGive)
                {
                    if(clue.Type == eType && clue.RelatesToCharacter == relatesToCharacter)
                    {
                        return clue;
                    }
                }

                bool TryGetType(ClueObject.ClueType eTypeToTry, out ClueObject clueOut)
                {
                    clueOut = null;

                    List<Tuple<Character, int>> seenCharacters = new List<Tuple<Character, int>>();

                    switch (eTypeToTry)
                    {
                        case ClueObject.ClueType.SawInLocation:
                        case ClueObject.ClueType.SawAtWork:
                        case ClueObject.ClueType.CommentFacialFeatures:
                        case ClueObject.ClueType.CommentClothing:
                            seenCharacters = Service.PhaseSolve.CurrentPhase.CharacterSeenMap[this];
                            break;
                        case ClueObject.ClueType.SawPassingBy:
                            seenCharacters = Service.PhaseSolve.CurrentPhase.CharacterSawPassingByMap[this];
                            break;
                    }

                    foreach (var charLoc in seenCharacters)
                    {
                        if (charLoc.Item1 == relatesToCharacter)
                        {
                            clueOut = new ClueObject(eTypeToTry)
                            {
                                GivenByCharacter = this,
                                RelatesToCharacter = relatesToCharacter,
                                LocationSeenIn = eTypeToTry == ClueObject.ClueType.SawAtWork ? -1 : charLoc.Item2
                            };

                            clueOut.Generate();

                            return true;
                        }
                    }

                    return false;
                }

                if (eType != ClueObject.ClueType.CommentGossip
                    && eType != ClueObject.ClueType.VisualFromGhost)
                {
                    ClueObject clue = null;
                    if(TryGetType(eType, out clue))
                    {
                        return clue;
                    }
                }
            }
        }

        // Fallback to just serving a random clue
        return ServeClueToPlayer();
    }

    public ClueObject ServeClueToPlayer()
    {
        if(Service.PhaseSolve.CurrentPhase == null)
        {
            Debug.LogError("Tried to give clue but currentphase was null?!");
            return null;
        }

        int iClueIndex = 0;
        List<ClueObject> myCluesToGive = Service.PhaseSolve.CurrentPhase.CharacterCluesToGive[this];

        if (myCluesToGive.Count > 1)
        {
            List<float> weightList = new List<float>();

            foreach (var c in myCluesToGive)
            {
                weightList.Add(c.GetWeightForThisClue());
            }

            iClueIndex = Randomiser.GetRandomIndexFromWeights(weightList);
            if (iClueIndex == -1)
            {
                Debug.LogError(string.Format("Failed to get random clue index. list had {0} clues ", weightList.Count));
                return null;
            }
        }

        Debug.Log(string.Format("{0} gave player {1} clue about {2}", 
            Name,
            myCluesToGive[iClueIndex].Type.ToString(), 
            myCluesToGive[iClueIndex].RelatesToCharacter?.Name ?? "invalidrelatestocharacter"));

        if(!IsAlive)
        {
            HasGhostGivenClue = true;
        }

        HasGivenAClueThisPhase = true;

        return myCluesToGive[iClueIndex];
    }

    public WerewolfGame.TOD DeathTimeOfDay;
    public int DeathDay;
    public int DeathLocation = -1;
    public int Age;

    public int Index;

    public Task CurrentTask;
    public Building Home;

    public bool HasGivenAClueThisPhase = false;

    public bool IsBeingTalkedTo = false;
    private Vector3 vPreviousDestination = new Vector3();
    public void SetBeingTalkedTo()
    {
        IsBeingTalkedTo = true;

        PhysicalCharacter pc = Service.Population.PhysicalCharacterMap[this];

        vPreviousDestination = pc.CurrentDestination;
        pc.CurrentDestination = pc.gameObject.transform.position;
    }
    public void ReleaseFromBeingTalkedTo()
    {
        IsBeingTalkedTo = false;

        PhysicalCharacter pc = Service.Population.PhysicalCharacterMap[this];
        pc.CurrentDestination = vPreviousDestination;
    }



    // Don't clear this - ghost can only give the clue once and then will disappear or remain static for the rest of the game
    public bool HasGhostGivenClue = false;

    private int currentTaskIndex = 0;

    public bool HasDiscoveredName()
    {
        return Service.InfoManager.EmoteMapBySubType[GetHeadshotEmoteSubType()].HasDiscovered;
    }

    public void SetNameDiscovered()
    {
        Service.InfoManager.EmoteMapBySubType[GetHeadshotEmoteSubType()].HasDiscovered = true;
    }

    public string GetName()
    {
        return HasDiscoveredName() ? Name : "???";
    }

    public bool CanTalkTo()
    {
        // If a ghost, and hasn't given a clue yet
        if(!IsAlive && !HasGhostGivenClue)
        {
            return true;
        }

        // If alive, and hasn't give a clue yet
        if(IsAlive && !HasGivenAClueThisPhase)
        {
            return true;
        }

        return false;
    }

    // Functions

    public Emote.EmoteSubType GetWorkType()
    {
        if(Descriptors[Descriptor.Occupation].Count == 0)
        {
            return Emote.InvalidSubType;
        }

        // Shouldn't be more than one occupation
        Debug.Assert(Descriptors[Descriptor.Occupation].Count == 1);

        return Descriptors[Descriptor.Occupation][0].SubType;
    }

    public Emote.EmoteSubType GetFacialFeatureType()
    {
        if (Descriptors[Descriptor.Facial].Count == 0)
        {
            Debug.LogError(string.Format("[{0}] {1} doesn't have a facial feature descriptor.", Index, Name));
            return Emote.InvalidSubType;
        }

        // Shouldn't be more than one facial feature
        Debug.Assert(Descriptors[Descriptor.Facial].Count == 1);

        return Descriptors[Descriptor.Facial][0].SubType;
    }


    public int GetWorkLocation()
    {
        Vector3 vDummyPos;
        int iWorkLocation = -1;

        Service.Population.GetWorkPositionAndLocation(this, out vDummyPos, out iWorkLocation);
        return iWorkLocation;
    }

    public Emote.EmoteSubType GetHeadshotEmoteSubType()
    {
        return Emote.EmoteSubType.CharacterHeadshot_1 + Index;
    }

    public int GetALocationCurrentlyIn()
    {
        List<int> iLocationList = new List<int>();

        if(Service.Game.CurrentTimeOfDay == WerewolfGame.TOD.Day)
        {
            foreach(var t in TaskSchedule.DayTasks)
            {
                if(Emote.IsLocationValid(t.Location))
                {
                    iLocationList.Add(t.Location);
                }
            }
        }
        else if (Service.Game.CurrentTimeOfDay == WerewolfGame.TOD.Night)
        {
            foreach (var t in TaskSchedule.NightTasks)
            {
                if (Emote.IsLocationValid(t.Location))
                {
                    iLocationList.Add(t.Location);
                }
            }
        }

        if(iLocationList.Count == 0)
        {
            return -1;
        }
        else if(iLocationList.Count == 1)
        {
            return iLocationList[0];
        }

        return iLocationList[UnityEngine.Random.Range(0, iLocationList.Count)];
    }

    public List<Emote> GetDescriptors(Descriptor eType)
    {
        return Descriptors[eType];
    }

    // Does the descriptors of a descriptor type match between this and another character
    public bool DoDescriptorsMatch(ref Character other, Descriptor eType)
    {
        if(other.Descriptors[eType].Count != Descriptors[eType].Count)
        {
            return false;
        }

        if(Descriptors[eType].Count == 0)
        {
            return false;
        }

        for(int i = 0; i < Descriptors[eType].Count; ++i)
        {
            if (Descriptors[eType][i].SubType != other.Descriptors[eType][i].SubType)
            {
                return false;
            }
        }

        return true;
    }

    public static List<Descriptor> GetAllDescriptorsInAList()
    {
        List<Descriptor> descriptorTypes = new List<Descriptor>()
            { Descriptor.Hair, Descriptor.Facial, Descriptor.Occupation, Descriptor.Clothing };

        return descriptorTypes;
    }

    public static Descriptor GetRandomDescriptorType(List<Descriptor> excludeTypes = null)
    {
        List<Descriptor> descriptorTypes = GetAllDescriptorsInAList();

        if (excludeTypes != null)
        {
            foreach(var et in excludeTypes)
            {
                if(descriptorTypes.Contains(et))
                {
                    descriptorTypes.Remove(et);
                }
            }
        }

        if(descriptorTypes.Count == 0)
        {
            Debug.LogWarning("Why call this will all type excluded?");
            return InvalidDescriptor;
        }
        else if(descriptorTypes.Count == 1)
        {
            return descriptorTypes[0];
        }

        return descriptorTypes[UnityEngine.Random.Range(0, descriptorTypes.Count)];
    }

    public SortedDictionary<Descriptor, List<Emote>> GetRandomDescriptors(int iCount, List<Descriptor> getOfType = null)
    {
        var grabbedList = new SortedDictionary<Descriptor, List<Emote>>();

        // If we only have one get of type, you should just call GetDescriptors
        Debug.Assert(getOfType == null || getOfType.Count > 1);
        if(getOfType != null && getOfType.Count == 1)
        {
            grabbedList.Add(getOfType[0], GetDescriptors(getOfType[0]));
            return grabbedList;
        }

        if(iCount > DescriptorMax)
        {
            Debug.LogError("DoDescriptorsMatch: Trying to grab more (" + iCount + ") descriptors than there are types.");
            iCount = DescriptorMax;
        }

        // If we only want to include a certain amount of types that is under how many we want to grab, we 
        //  would enter an infinite loop - so just clear them for release
        if(getOfType != null && getOfType.Count < iCount)
        {
#if UNITY_EDITOR
            string sGetOfTypes = "";
            foreach(var t in getOfType)
            {
                sGetOfTypes += t.ToString() + ",";
            }
            Debug.LogError("DoDescriptorsMatch: Too many grabs (" + iCount + ") for too few inclusion types (" + getOfType.Count + ").");
#endif
            getOfType = null;
        }

        while(grabbedList.Count < iCount)
        {
            var iRandIndex = UnityEngine.Random.Range(0, DescriptorMax);
            var desc = (Descriptor)iRandIndex;

            if(getOfType != null && !getOfType.Contains(desc))
            {
                continue;
            }

            if (!grabbedList.ContainsKey(desc))
            {
                grabbedList.Add(desc, new List<Emote>());
                grabbedList[desc] = Descriptors[desc];
            }
        }

        return grabbedList;
    }

    public bool CanBeKilledByWerewolf(WerewolfGame.TOD eTod, bool bSleepImmunity = true)
    {
        if(!IsAlive)
        {
            return false;
        }

        if(IsWerewolf)
        {
            return false;
        }

        if(eTod == WerewolfGame.TOD.Night && bSleepImmunity)
        {
            if(TaskSchedule?.NightTasks != null && TaskSchedule.NightTasks.Count > 0)
            {
                if(TaskSchedule.NightTasks[0].Type == Task.TaskType.Sleep)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public bool IsSleeping()
    {
        if(CurrentTask != null)
        {
            if(CurrentTask.Type == Task.TaskType.Sleep)
            {
                return true;
            }
        }

        if (Service.Game.CurrentTimeOfDay == WerewolfGame.TOD.Day)
        {
            if (TaskSchedule?.DayTasks != null && TaskSchedule.DayTasks.Count > 0)
            {
                if (TaskSchedule.DayTasks[0].Type == Task.TaskType.Sleep)
                {
                    return true;
                }
            }
        }
        if (Service.Game.CurrentTimeOfDay == WerewolfGame.TOD.Night)
        {
            if (TaskSchedule?.NightTasks != null && TaskSchedule.NightTasks.Count > 0)
            {
                if (TaskSchedule.NightTasks[0].Type == Task.TaskType.Sleep)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool WillTravelThroughLocationDuringTasks(WerewolfGame.TOD eTod, List<int> iLocations, out int iLocSeenIn)
    {
        iLocSeenIn = 0;

        List<Task> taskList = eTod == WerewolfGame.TOD.Day
            ? TaskSchedule.DayTasks
            : TaskSchedule.NightTasks;

        if(taskList == null)
        {
            return false;
        }

        if(taskList.Count <= 1)
        {
            return false;
        }

        for(int i = 0; i < taskList.Count - 1; ++i)
        {
            int thisTaskLocation = taskList[i].Location;
            int nextTaskLocation = taskList[i + 1].Location;

            // No travel
            if(thisTaskLocation == nextTaskLocation)
            {
                continue;
            }

            // Make sure they're valid locations
            if(thisTaskLocation >= 0 && nextTaskLocation >= 0
                && thisTaskLocation <= Emote.LocationMax && nextTaskLocation <= Emote.LocationMax)
            {
                List<int> pathTaken = Service.Population.GetPathBetweenLocations(thisTaskLocation, nextTaskLocation);
                if(pathTaken.Count == 0)
                {
                    continue;
                }

                // If any part of the characters task would take them through the passed in location, return true
                foreach (var iLoc in iLocations)
                {
                    if(pathTaken.Contains(iLoc))
                    {
                        iLocSeenIn = iLoc;
                        return true;
                    }
                }
            }
        }

        return false;
    }

    // Private functions

    void GotoNextTask()
    {
        // Will be true in the case of the character being a ghost
        if(TaskSchedule == null)
        {
            return;
        }

        void TryNextTask(ref List<Task> t)
        {
            if(t.Count == 0)
            {
                return;
            }

            bool bDeviate = false;

            // We still have tasks left
            if (currentTaskIndex + 1 < t.Count)
            {
                currentTaskIndex++;

                // Give a small change for characters to deviate from their schedule, so that they're not 100% predictable.
                bDeviate = UnityEngine.Random.Range(0.0f, 100.0f) < 5.0f;
            }
            // Otherwise loop to the start
            else
            {
                currentTaskIndex = 0;
            }

            if (!bDeviate)
            {
                Debug.Assert(currentTaskIndex < t.Count);
                if (currentTaskIndex < t.Count)
                {
                    CurrentTask = t[currentTaskIndex];
                }
            }
            else
            {
                // If deviating from the schedule, get a random task type that isn't what we would have picked next
                CurrentTask = new Task(this, Task.GetRandomTaskType(t[currentTaskIndex].Type, Service.Game.CurrentTimeOfDay == WerewolfGame.TOD.Day));
            }
        }

        void TryGetFirstTask(ref List<Task> t)
        {
            if (t.Count > 0)
            {
                CurrentTask = t[0];
                currentTaskIndex = 0;
            }
        }

        if(CurrentTask == null)
        {
            if(Service.Game.CurrentTimeOfDay == WerewolfGame.TOD.Day)
            {
                TryGetFirstTask(ref TaskSchedule.DayTasks);
            }
            else if (Service.Game.CurrentTimeOfDay == WerewolfGame.TOD.Night)
            {
                TryGetFirstTask(ref TaskSchedule.NightTasks);
            }
        }
        else
        {
            if (Service.Game.CurrentTimeOfDay == WerewolfGame.TOD.Day)
            {
                TryNextTask(ref TaskSchedule.DayTasks);
            }
            else if (Service.Game.CurrentTimeOfDay == WerewolfGame.TOD.Night)
            {
                TryNextTask(ref TaskSchedule.NightTasks);
            }
        }

        CurrentTask?.UpdatePosition();
        if(CurrentTask != null)
        {
            Service.Population.PhysicalCharacterMap[this].SetNotWait();
        }
    }

    public void Update()
    {
        if(IsBeingTalkedTo)
        {
            return;
        }

        if (!IsAlive)
        {
            return;
        }

        if(IsSleeping())
        {
            return;
        }

        if (CurrentTask == null || CurrentTask.ShouldFinish)
        {
            if (CurrentTask != null)
            {
                CurrentTask.Timer = 0.0f;
            }

            GotoNextTask();
        }
        else if(CurrentTask != null)
        {
            CurrentTask.Update();
        }
    }

    public void TryWarpToTaskLocation()
    {
        if (CurrentTask != null)
        {
            CurrentTask.WarpToTaskPosition();
        }
        else
        {
            Debug.Log(string.Format("TryWarpToTaskLocation: {0} had to task to warp to.", Name));
        }
    }

    public void OnTimeOfDayPhaseShift(bool bWarpToTaskPosition = false) 
    {
        if(!IsAlive)
        {
            return;
        }

        if (CurrentTask != null)
        {
            CurrentTask.Timer = 0.0f;
        }

        CurrentTask = null;
        currentTaskIndex = 0;

        // 20% chance of adding in a sleep for the current task, so that the 
        //  character doesn't appear during this phase at all.
        if (Service.Game.CurrentTimeOfDay == WerewolfGame.TOD.Day 
            && !IsWerewolf 
            && !IsVictim
            && UnityEngine.Random.Range(0.0f, 100.0f) <= 20.0f)
        {
            // Make sure we're not by chance putting loads of character to sleep
            int iNumberThatWillSleep = Service.Population.GetNumberOfCharactersThatWillSleep(WerewolfGame.TOD.Day);
            if(iNumberThatWillSleep < 7)
            {
                CurrentTask = new Task(this, Task.TaskType.Sleep);
            }
        }
    }
}
