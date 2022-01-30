using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Service
{
    public static InformationManager InfoManager;
    public static PopulationManager Population;
    public static ConfigManager Config;
    public static WerewolfGame Game;
    public static ClueManager Clue;
    public static PhaseSolver PhaseSolve;
    public static PlayerManager Player;
    public static AudioManager Audio;
    public static DebugLevelLoader Transition;
    public static OptionsManager Options;
    public static TransitionScreenManager TransitionScreen;
    public static LocationManager Location;
    public static LightingManager Lighting;
    public static CaseFileRenderer CaseFile;
    public static DialogueRenderer Dialogue;

    public static DialogueManager DialogueManager;
    public static CharacterGenerator CharacterGenerator;
    public static TooltipManager TooltipManager;
    public static EmoteLibrary EmoteLibrary;

    public static float MusicVolume = 0.5f;

}
