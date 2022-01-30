using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A clue is a container of multiple emotes that form a clue given to the player
/// </summary>
[System.Serializable]
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
            case ClueType.CommentClothing:          return 40.0f;
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

    public float GetWeightForThisClue()
    {
        float baseWeight = Weight;
        
        // If it relates to the werewolf, should very strongly weight it towards this clue
        if(RelatesToCharacter != null && RelatesToCharacter.IsWerewolf)
        {
            baseWeight *= 10f;

            // BIG weight towards werewolf clues if the player doesn't have as many clues as the current day
            //  try to heavily guarantee that the player averages at least one good clue per day
            if(Service.Player.NumberOfCluesAboutWerewolf < Service.Game.CurrentDay)
            {
                baseWeight *= 10f;
            }
        }

        return baseWeight;
    }

    // Weight for picking this clue - standardise between 0.0f and 100.0f of how likely this clue will be picked
    //  NOTE: clue weights don't need to add to 100.0f
    public float Weight = 0.0f;

    // The list of emotes to form the sentence.
    public List<Emote> Emotes = new List<Emote>();

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
                GenerateSawInLocationEmotes();
                break;
            case ClueType.SawPassingBy:
                GenerateSawPassingByEmotes();
                break;
            case ClueType.SawAtWork:
                GenerateSawAtWorkEmotes();
                break;
            case ClueType.CommentFacialFeatures:
                GenerateCommentFacialFeaturesEmotes();
                break;
            case ClueType.CommentClothing:
                GenerateCommentClothingEmotes();
                break;
            case ClueType.CommentGossip:
                GenerateCommentGossipEmotes();
                break;
            case ClueType.VisualFromGhost:
                GenerateVisualFromGhostEmotes();
                break;
        }

        // Randomly add on the opinion they have of this character to all emotes
        if (Type != ClueType.VisualFromGhost
            && UnityEngine.Random.Range(0, 100.0f) < Service.Config.ChanceToAddOpinionOfCharacterAtClueEnd)
        {
            // Add on: "Opinion" "CharacterHeadshot"
            AddEmote(Emote.EmoteSubType.Specific_Spacing);
            AddEmote(Service.Population.GetCharactersOpinionOf(GivenByCharacter, RelatesToCharacter));
            AddEmote(RelatesToCharacter.GetHeadshotEmoteSubType());
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
                Debug.Assert(LocationSeenIn == -1);
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
#endif

    void AddEmote(Emote.EmoteSubType eSubType)
    {
        Debug.Assert(eSubType != Emote.InvalidSubType);
        if (eSubType != Emote.InvalidSubType)
        {
            Emotes.Add(Service.InfoManager.EmoteMapBySubType[eSubType]);
        }
    }

    void GenerateSawInLocationEmotes()
    {
        // Should be: "Eyes" "CharacterHeadshot" "Location"
        AddEmote(Emote.EmoteSubType.Specific_Eyes);
        AddEmote(RelatesToCharacter.GetHeadshotEmoteSubType());
        AddEmote(Emote.GetLocationEnum(LocationSeenIn));
    }

    void GenerateSawPassingByEmotes()
    {
        // Should be: "Eyes" "CharacterHeadshot" "Footsteps" "Location"
        AddEmote(Emote.EmoteSubType.Specific_Eyes);
        AddEmote(RelatesToCharacter.GetHeadshotEmoteSubType());
        AddEmote(Emote.EmoteSubType.Specific_Footsteps);
        AddEmote(Emote.GetLocationEnum(LocationSeenIn));
    }

    void GenerateSawAtWorkEmotes()
    {
        // Should be: "Eyes" "CharacterHeadshot" "Occupation"
        AddEmote(Emote.EmoteSubType.Specific_Eyes);
        AddEmote(RelatesToCharacter.GetHeadshotEmoteSubType());

        Emote.EmoteSubType eWorkType = RelatesToCharacter.GetWorkType();

        //Debug.Assert(eWorkType != Emote.InvalidSubType, 
        //    string.Format("Found invalid work type for {0}. How did we get to generating a clue for this?", RelatesToCharacter.Name));

        // If the work type is invalid, likely is a lie
        if(eWorkType == Emote.InvalidSubType)
        {
            Debug.Log(string.Format("Created fake work for [{0}] {1}", RelatesToCharacter.Index, RelatesToCharacter.Name));
            eWorkType = Service.InfoManager.GetRandomEmoteOfType(Emote.EmoteType.Occupation).SubType;
            IsTruth = false;
        }

        AddEmote(eWorkType);
    }

    void GenerateCommentFacialFeaturesEmotes()
    {
        // Should be: "CharacterHeadshot" "Facial" "Random: Approves/Disapproves"
        AddEmote(RelatesToCharacter.GetHeadshotEmoteSubType());
        AddEmote(RelatesToCharacter.GetFacialFeatureType());

        if(UnityEngine.Random.Range(0.0f, 100.0f) < Service.Config.ChanceToAddApprovalDisapprovalAtClueEnd)
        {
            AddEmote(Emote.GetRandomWeightedApprovalDisapproval(GivenByCharacter, RelatesToCharacter));
        }
    }

    void GenerateCommentClothingEmotes()
    {
        // Should be: "CharacterHeadshot" "Optional: Condition" "Clothing" "Random: Approves/Disapproves"
        AddEmote(RelatesToCharacter.GetHeadshotEmoteSubType());

        List<Emote> clothing = new List<Emote>();
        
        if(RelatesToCharacter.CurrentClothingCondition != null)
        {
            clothing.Add(RelatesToCharacter.CurrentClothingCondition);
        }
        foreach(var e in RelatesToCharacter.GetDescriptors(Character.Descriptor.Clothing))
        {
            clothing.Add(e);
        }
        
        foreach (var emote in clothing)
        {
            AddEmote(emote.SubType);
        }

        if (UnityEngine.Random.Range(0.0f, 100.0f) < Service.Config.ChanceToAddApprovalDisapprovalAtClueEnd)
        {
            AddEmote(Emote.GetRandomWeightedApprovalDisapproval(GivenByCharacter, RelatesToCharacter));
        }
    }

    void GenerateCommentGossipEmotes()
    {
        // Should be: "CharacterHeadshot" "Gossip" "Random: Approves/Disapproves"
        AddEmote(RelatesToCharacter.GetHeadshotEmoteSubType());
        AddEmote(Service.InfoManager.GetRandomEmoteOfType(Emote.EmoteType.Gossip).SubType);

        if (UnityEngine.Random.Range(0.0f, 100.0f) < Service.Config.ChanceToAddApprovalDisapprovalAtClueEnd)
        {
            AddEmote(Emote.GetRandomWeightedApprovalDisapproval(GivenByCharacter, RelatesToCharacter));
        }
    }

    void GenerateVisualFromGhostEmotes()
    {
        // Should be: "CharacterHeadshot" "Hair/Facial/Clothing"
        AddEmote(Emote.EmoteSubType.Specific_Werewolf);

        List<Emote> descriptors = RelatesToCharacter.GetDescriptors(GhostGivenClueType);
        foreach(var emote in descriptors)
        {
            AddEmote(emote.SubType);
        }
    }
}
