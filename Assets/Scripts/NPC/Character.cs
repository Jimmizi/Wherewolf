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


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
