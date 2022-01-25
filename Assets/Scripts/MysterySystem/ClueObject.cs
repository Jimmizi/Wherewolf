using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A clue is a container of multiple emotes that form a clue given to the player
/// </summary>
public class ClueObject
{
    // The list of emotes to form the sentence.
    public List<Emote> Emotes;

    public Character GivenByCharacter;
    public Character RelatesToCharacter;

    // Whether the string of emotes relates to a true statement.
    public bool IsTruth;

    // What day was the clue given on
    public int Day = 0;

    // Time of day the clue was given
    public WerewolfGame.TOD TimeOfDay;
}
