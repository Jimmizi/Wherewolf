using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A clue is a container of multiple emotes that form a clue given to the player
/// </summary>
public class ClueObject
{
    public Character RelatesToCharacter;
    public Character GivenByCharacter;

    // What day was the clue given on
    public int GivenOnDay = 0;

    // Was it daytime when given the clue
    public bool GivenDuringDay;

    // The list of emotes to form the sentence.
    public List<Emote> Emotes;

    // Whether the string of emotes relates to a true statement.
    public bool IsTruth;
}
