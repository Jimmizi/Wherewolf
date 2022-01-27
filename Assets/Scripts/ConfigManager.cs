using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigManager : MonoBehaviour
{
    // Last day of the game
    public const int NumberOfDaysBeforeGameFailure = 20;

    [SerializeField]
    public int CharacterHasOccupationChance = 50;

    [SerializeField]
    public float WanderRandomTimeMin = 20.0f;
    [SerializeField]
    public float WanderRandomTimeMax = 60.0f;

    [SerializeField]
    public float IdleRandomTimeMin = 15.0f;
    [SerializeField]
    public float IdleRandomTimeMax = 45.0f;

    [SerializeField]
    public float WorkRandomTimeMin = 30.0f;
    [SerializeField]
    public float WorkRandomTimeMax = 120.0f;

    // To give the player an extra hint, have the werewolf "sleep" the phase after killing a victim.
    [SerializeField]
    public bool WerewolfDisappearsAfterMurder = true;

    // Chance of a ghost lying when giving a clue
    [SerializeField]
    public float GhostLieChance = 40.0f;

    // Falloff from the GhostLieChance after every ghost lie given (e.g. if GhostLieChance is 40% and
    //  GhostLieChanceFalloff is 10%, after four ghosts have lied, they can no longer lie).
    [SerializeField]
    public float GhostLieChanceFalloff = 10.0f;

    // Falloff from GhostLieChance, multiplies GhostLieChanceFalloffPerDay per how many days have passed
    //  in the game, so later days in the game will have a smaller chance of ghosts lying to the player.
    [SerializeField]
    public float GhostLieChanceFalloffPerDay = 1.0f;

    // Chance of an alive character lying when giving a clue
    [SerializeField]
    public float CharacterLieChance = 0.0f;

    [SerializeField]
    public float WerewolfLieChance = 20.0f;

    // In the late game, allow ghosts to give true descriptors that are unique to the werewolf
    //  Enabling this will give the player a unique piece of information after
    //  NumberOfDeathsToClassifyLateGame characters have been killed
    [SerializeField]
    public bool AllowLateGameUniqueIdentifiersFromGhosts = true;

    // How many characters have to be killed by the werewolf before we classify the player as
    //  being in the end phase of the game (10 victims will be generated over the course of the game).
    [SerializeField]
    public int NumberOfDeathsToClassifyLateGame = 6;

    // Start is called before the first frame update
    void Awake()
    {
        Service.Config = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
