using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phase
{
    public WerewolfGame.TOD TimeOfDay;
    public Character Victim = null;

    // Map of characters a character has seen in locations throughout this phase
    public Dictionary<Character, List<Tuple<Character, int>>> CharacterSeenMap = new Dictionary<Character, List<Tuple<Character, int>>>();

    // Map of characters a character has seen passing through their locations throughout this phase
    public Dictionary<Character, List<Tuple<Character, int>>> CharacterSawPassingByMap = new Dictionary<Character, List<Tuple<Character, int>>>();

    // Map of tasks a character performed during this phase
    public Dictionary<Character, List<Task>> CharacterTasks = new Dictionary<Character, List<Task>>();

    // Map of clues a character was able to give to the player this phase
    public Dictionary<Character, List<ClueObject>> CharacterCluesToGive = new Dictionary<Character, List<ClueObject>>();

    public bool IsCharacterInSeenMap(Character characterWhoSaw, Character characterToSee)
    {
        foreach(var tup in CharacterSeenMap[characterWhoSaw])
        {
            if(tup.Item1 == characterToSee)
            {
                return true;
            }
        }

        return false;
    }

    public Phase(Phase other)
    {
        TimeOfDay = other.TimeOfDay;
        Victim = other.Victim;

        CharacterSeenMap = other.CharacterSeenMap;
        CharacterSawPassingByMap = other.CharacterSawPassingByMap;
        CharacterTasks = other.CharacterTasks;
        CharacterCluesToGive = other.CharacterCluesToGive;
    }
    public Phase(WerewolfGame.TOD eTod)
    {
        TimeOfDay = eTod;
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
