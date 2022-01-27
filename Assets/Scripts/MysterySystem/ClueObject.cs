using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A clue is a container of multiple emotes that form a clue given to the player
/// </summary>
public class ClueObject
{
    public enum ClueType
    {
        SawInLocation,          // Saw a character in one of their locations during the phase
        SawPassingBy,           // Saw a character passing by their locations during the phase (just passing by, not stopping in the location)
        SawAtWork,              // Saw a character doing their work
        CommentFacialFeatures,  // Comments on a characters facial features
        CommentClothing,        // Comments on a characters clothing  
        CommentGossip,          // Random gossip on/between characters that aren't useful

        VisualFromGhost,        // Visual clue given by a ghost (high variance between true/false clues)
    }

    public static float GetDefaultWeight(ClueType eType)
    {
        switch (eType)
        {
            case ClueType.SawInLocation:            return 50.0f;
            case ClueType.SawPassingBy:             return 25.0f;
            case ClueType.SawAtWork:                return 70.0f;
            case ClueType.CommentFacialFeatures:    return 25.0f;
            case ClueType.CommentClothing:          return 25.0f;
            case ClueType.CommentGossip:            return 25.0f;
            case ClueType.VisualFromGhost:          return 100.0f;
        }

        return 20.0f;
    }

    public ClueObject(ClueType eType)
    {
        Type = eType;
        Weight = GetDefaultWeight(eType);
    }

    public ClueType Type;

    // Weight for picking this clue - standardise between 0.0f and 100.0f of how likely this clue will be picked
    //  NOTE: clue weights don't need to add to 100.0f
    public float Weight = 0.0f;

    // The list of emotes to form the sentence.
    public List<Emote> Emotes;

    public Character GivenByCharacter;
    public Character RelatesToCharacter;

    public Character.Descriptor GhostGivenClueType;

    public int LocationSeenIn = -1;

    // Whether the string of emotes relates to a true statement.
    public bool IsTruth = true;

    // What day was the clue given on
    public int Day = 0;

    // Time of day the clue was given
    public WerewolfGame.TOD TimeOfDay;

    public void Generate()
    {
        Day = Service.Game.CurrentDay;
        TimeOfDay = Service.Game.CurrentTimeOfDay;

        Emotes = new List<Emote>();

#if UNITY_EDITOR
        ValidateData();
#endif

        switch (Type)
        {
            case ClueType.SawInLocation:
                break;
            case ClueType.SawPassingBy:
                break;
            case ClueType.SawAtWork:
                break;
            case ClueType.CommentFacialFeatures:
                break;
            case ClueType.CommentClothing:
                break;
            case ClueType.CommentGossip:
                break;
            case ClueType.VisualFromGhost:
                Debug.Assert(GhostGivenClueType != Character.Descriptor.Occupation);
                break;
        }

    }

#if UNITY_EDITOR
    void ValidateData()
    {
        Debug.Assert(GivenByCharacter != null);
        Debug.Assert(RelatesToCharacter != null);

        switch (Type)
        {
            case ClueType.SawInLocation:
                Debug.Assert(Emote.IsLocationValid(LocationSeenIn));
                break;
            case ClueType.SawPassingBy:
                Debug.Assert(Emote.IsLocationValid(LocationSeenIn));
                break;
            case ClueType.SawAtWork:
                break;
            case ClueType.CommentFacialFeatures:
                break;
            case ClueType.CommentClothing:
                break;
            case ClueType.CommentGossip:
                break;
            case ClueType.VisualFromGhost:
                break;
        }
    }
#endif
}
