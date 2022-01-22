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

    // Vars

    public string Name;
    public SortedDictionary<Descriptor, List<Emote>> Descriptors = new SortedDictionary<Descriptor, List<Emote>>();
    public bool IsWerewolf;
    public bool IsAlive = true;
    public Schedule TaskSchedule = new Schedule();

    public Task CurrentTask;
    public Building Home;

    private int currentTaskIndex = 0;

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

    public SortedDictionary<Descriptor, List<Emote>> GetRandomDescriptors(int iCount)
    {
        Debug.Assert(iCount <= DescriptorMax);

        var grabbedList = new SortedDictionary<Descriptor, List<Emote>>();

        while(grabbedList.Count < iCount)
        {
            var iRandIndex = UnityEngine.Random.Range(0, DescriptorMax);
            var desc = (Descriptor)iRandIndex;

            if (!grabbedList.ContainsKey(desc))
            {
                grabbedList.Add(desc, new List<Emote>());
                grabbedList[desc] = Descriptors[desc];
            }
        }

        return grabbedList;
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

            // We still have tasks left
            if (currentTaskIndex + 1 < t.Count)
            {
                currentTaskIndex++;
            }
            // Otherwise loop to the start
            else
            {
                currentTaskIndex = 0;
            }

            Debug.Assert(currentTaskIndex < t.Count);
            if (currentTaskIndex < t.Count)
            {
                CurrentTask = t[currentTaskIndex];
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
            if(Service.Game.TimeOfDay == WerewolfGame.TOD.Day)
            {
                TryGetFirstTask(ref TaskSchedule.DayTasks);
            }
            else if (Service.Game.TimeOfDay == WerewolfGame.TOD.Night)
            {
                TryGetFirstTask(ref TaskSchedule.NightTasks);
            }
        }
        else
        {
            if (Service.Game.TimeOfDay == WerewolfGame.TOD.Day)
            {
                TryNextTask(ref TaskSchedule.DayTasks);
            }
            else if (Service.Game.TimeOfDay == WerewolfGame.TOD.Night)
            {
                TryNextTask(ref TaskSchedule.NightTasks);
            }
        }
    }

    public void Start()
    {
        
    }

    public void Update()
    {
        if(CurrentTask == null || CurrentTask.ShouldFinish)
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

    public void OnTimeOfDayPhaseShift() 
    {
        if (CurrentTask != null)
        {
            CurrentTask.Timer = 0.0f;
        }

        CurrentTask = null;
        currentTaskIndex = 0;
    }
}
