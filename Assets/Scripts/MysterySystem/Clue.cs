using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A clue is a container of multiple emotes that form a clue given to the player
/// </summary>
public class Clue
{
    public Character RelatesToCharacter;

    // The list of emotes to form the sentence.
    public List<Emote> Emotes;

    // Whether the string of emotes relates to a true statement.
    public bool IsTruth;
}
