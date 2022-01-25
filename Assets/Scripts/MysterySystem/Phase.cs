using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phase
{
    public WerewolfGame.TOD TimeOfDay;
    public Character Victim = null;

    public Dictionary<Character, List<Character>> CharacterSeenMap = new Dictionary<Character, List<Character>>();
    public Dictionary<Character, List<Task>> CharacterTasks = new Dictionary<Character, List<Task>>();

    public Phase(Phase other)
    {
        TimeOfDay = other.TimeOfDay;
        CharacterSeenMap = other.CharacterSeenMap;
        CharacterTasks = other.CharacterTasks;
        Victim = other.Victim;
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
