using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// The emote class encompasses a single emote icon.
/// </summary>
[System.Serializable]
public class Emote
{
    public enum EmoteType
    {
        HairStyle,
        FacialFeature,
        Color,
        Location,
        Occupation,
        TimeOfDay,
        Clothing,
        Condition,
        Specific,
        Gossip,
        CharacterHeadshot,
        Opinion,
    }

    // If adding to this, update GetTypeOfSubEmote() and GetLastSubType
    public enum EmoteSubType
    {
        HairStyle_Long,
        HairStyle_Medium,
        HairStyle_Short,
        HairStyle_Bald,

        Facial_Ugly,
        Facial_Beautiful,
        Facial_Warts,
        Facial_BigNose,
        Facial_BigEyes,
        Facial_BigMouth,
        Facial_BigEars,

        Color_Brown,
        Color_Black,
        Color_White,
        Color_Blue,
        Color_Red,
        Color_Orange,
        Color_Yellow,
        Color_Green,
        Color_Purple,

        Location_1,
        Location_2,
        Location_3,
        Location_4,
        Location_5,
        Location_6,
        Location_7,
        Location_8,
        Location_9,

        Occupation_Bank,
        Occupation_Hospital,
        Occupation_Law,
        Occupation_School,
        Occupation_Tavern,
        Occupation_Blacksmith,
        Occupation_WoodCutter,
        Occupation_Engineer,
        Occupation_Fisher,
        Occupation_Library,
        Occupation_GeneralStore,

        TimeOfDay_Day,
        TimeOfDay_Night,

        Condition_Dirty,
        Condition_Wet,
        Condition_Bloody,
        Condition_Clean,
        Condition_Torn,

        Clothing_Shirt,
        Clothing_Jacket,
        Clothing_Boots,
        Clothing_Trousers,
        Clothing_Hat,
        Clothing_Glasses,
        Clothing_Gloves,

        Specific_Footsteps,
        Specific_AccidentKnife,
        Specific_AccidentFall,
        Specific_Eyes,
        Specific_Werewolf,
        Specific_Approves,
        Specific_Disapproves,
        Specific_TalkAction,
        Specific_StakeAction,

        Gossip_RelationshipKiss,
        Gossip_RelationshipFight,
        Gossip_RelationshipMarriage,

        CharacterHeadshot_1,
        CharacterHeadshot_2,
        CharacterHeadshot_3,
        CharacterHeadshot_4,
        CharacterHeadshot_5,
        CharacterHeadshot_6,
        CharacterHeadshot_7,
        CharacterHeadshot_8,
        CharacterHeadshot_9,
        CharacterHeadshot_10,
        CharacterHeadshot_11,
        CharacterHeadshot_12,
        CharacterHeadshot_13,
        CharacterHeadshot_14,
        CharacterHeadshot_15,
        CharacterHeadshot_16,
        CharacterHeadshot_17,
        CharacterHeadshot_18,
        CharacterHeadshot_19,
        CharacterHeadshot_20,

        Opinion_Love,
        Opinion_Like,
        Opinion_Neutral,
        Opinion_Dislike,
        Opinion_Hate,
    }

    public Emote(EmoteSubType eSubType)
    {
        Type = GetTypeOfSubEmote(eSubType);
        SubType = eSubType;
    }

    public static int LocationMin => 0;
    public static int LocationMax => 8;
    public static EmoteSubType GetLocationEnum(int iLocation) => EmoteSubType.Location_1 + iLocation;
    public static bool IsLocationValid(int iLoc)
    {
        return iLoc >= LocationMin && iLoc <= LocationMax;
    }

    public static EmoteType InvalidType => (EmoteType)(-1);
    public static EmoteSubType InvalidSubType => (EmoteSubType)(-1);
    public static EmoteSubType GetLastSubType => EmoteSubType.Opinion_Hate;

    private static List<float> opinionWeighting = new List<float>
    {
        0.1f, // Love
        0.5f, // Like
        1.0f, // Neutral
        0.5f, // Dislike
        0.1f  // Hate
    };
    private static List<float> werewolfOpinionWeighting = new List<float>
    {
        0.0f, // Love
        0.2f, // Like
        0.3f, // Neutral
        0.6f, // Dislike
        1.0f  // Hate
    };

    public static EmoteSubType GetRandomWeightedOpinion(bool bUseWerewolfTable = false)
    {
        /*EmoteSubType.Opinion_Love, 
        EmoteSubType.Opinion_Like, 
        EmoteSubType.Opinion_Neutral,
        EmoteSubType.Opinion_Dislike,
        EmoteSubType.Opinion_Hate*/

        int iIndex = Randomiser.GetRandomIndexFromWeights(bUseWerewolfTable ? werewolfOpinionWeighting : opinionWeighting);
        return EmoteSubType.Opinion_Love + iIndex;
    }
    public static EmoteSubType GetRandomWeightedApprovalDisapproval(Character givingApprovalChar, Character approvalOfChar)
    {
        float fApprovalChance = 0.0f;
        switch(Service.Population.GetCharactersOpinionOf(givingApprovalChar, approvalOfChar))
        {
            case EmoteSubType.Opinion_Love:      fApprovalChance = 95.0f; break;
            case EmoteSubType.Opinion_Like:      fApprovalChance = 75.0f; break;
            case EmoteSubType.Opinion_Neutral:   fApprovalChance = 50.0f; break;
            case EmoteSubType.Opinion_Dislike:   fApprovalChance = 25.0f; break;
            case EmoteSubType.Opinion_Hate:      fApprovalChance = 10.0f; break;
        }

        bool bApproves = UnityEngine.Random.Range(0.0f, 100.0f) < fApprovalChance;

        return bApproves
            ? EmoteSubType.Specific_Approves
            : EmoteSubType.Specific_Disapproves;
    }

    public static EmoteType GetTypeOfSubEmote(EmoteSubType eType)
    {
        switch (eType)
        {
            case EmoteSubType.HairStyle_Long:
            case EmoteSubType.HairStyle_Medium:
            case EmoteSubType.HairStyle_Short:
            case EmoteSubType.HairStyle_Bald:
                return EmoteType.HairStyle;

            case EmoteSubType.Facial_Ugly:
            case EmoteSubType.Facial_Beautiful:
            case EmoteSubType.Facial_Warts:
            case EmoteSubType.Facial_BigNose:
            case EmoteSubType.Facial_BigEyes:
            case EmoteSubType.Facial_BigMouth:
            case EmoteSubType.Facial_BigEars:
                return EmoteType.FacialFeature;

            case EmoteSubType.Color_Brown:
            case EmoteSubType.Color_Black:
            case EmoteSubType.Color_White:
            case EmoteSubType.Color_Blue:
            case EmoteSubType.Color_Red:
            case EmoteSubType.Color_Orange:
            case EmoteSubType.Color_Yellow:
            case EmoteSubType.Color_Green:
            case EmoteSubType.Color_Purple:
                return EmoteType.Color;

            case EmoteSubType.Location_1:
            case EmoteSubType.Location_2:
            case EmoteSubType.Location_3:
            case EmoteSubType.Location_4:
            case EmoteSubType.Location_5:
            case EmoteSubType.Location_6:
            case EmoteSubType.Location_7:
            case EmoteSubType.Location_8:
            case EmoteSubType.Location_9:
                return EmoteType.Location;

            case EmoteSubType.Occupation_Bank:
            case EmoteSubType.Occupation_Hospital:
            case EmoteSubType.Occupation_Law:
            case EmoteSubType.Occupation_School:
            case EmoteSubType.Occupation_Tavern:
            case EmoteSubType.Occupation_Blacksmith:
            case EmoteSubType.Occupation_WoodCutter:
            case EmoteSubType.Occupation_Engineer:
            case EmoteSubType.Occupation_Fisher:
            case EmoteSubType.Occupation_Library:
            case EmoteSubType.Occupation_GeneralStore:
                return EmoteType.Occupation;

            case EmoteSubType.TimeOfDay_Day:
            case EmoteSubType.TimeOfDay_Night:
                return EmoteType.TimeOfDay;

            case EmoteSubType.Condition_Dirty:
            case EmoteSubType.Condition_Wet:
            case EmoteSubType.Condition_Bloody:
            case EmoteSubType.Condition_Clean:
            case EmoteSubType.Condition_Torn:
                return EmoteType.Condition;

            case EmoteSubType.Clothing_Shirt:
            case EmoteSubType.Clothing_Jacket:
            case EmoteSubType.Clothing_Boots:
            case EmoteSubType.Clothing_Trousers:
            case EmoteSubType.Clothing_Hat:
            case EmoteSubType.Clothing_Glasses:
            case EmoteSubType.Clothing_Gloves:
                return EmoteType.Clothing;

            case EmoteSubType.Specific_Footsteps:
            case EmoteSubType.Specific_AccidentKnife:
            case EmoteSubType.Specific_AccidentFall:
            case EmoteSubType.Specific_Eyes:
            case EmoteSubType.Specific_Werewolf:
            case EmoteSubType.Specific_Approves:
            case EmoteSubType.Specific_Disapproves:
            case EmoteSubType.Specific_TalkAction:
            case EmoteSubType.Specific_StakeAction:
                return EmoteType.Specific;

            case EmoteSubType.Gossip_RelationshipKiss:
            case EmoteSubType.Gossip_RelationshipFight:
            case EmoteSubType.Gossip_RelationshipMarriage:
                return EmoteType.Gossip;

            case EmoteSubType.CharacterHeadshot_1:
            case EmoteSubType.CharacterHeadshot_2:
            case EmoteSubType.CharacterHeadshot_3:
            case EmoteSubType.CharacterHeadshot_4:
            case EmoteSubType.CharacterHeadshot_5:
            case EmoteSubType.CharacterHeadshot_6:
            case EmoteSubType.CharacterHeadshot_7:
            case EmoteSubType.CharacterHeadshot_8:
            case EmoteSubType.CharacterHeadshot_9:
            case EmoteSubType.CharacterHeadshot_10:
            case EmoteSubType.CharacterHeadshot_11:
            case EmoteSubType.CharacterHeadshot_12:
            case EmoteSubType.CharacterHeadshot_13:
            case EmoteSubType.CharacterHeadshot_14:
            case EmoteSubType.CharacterHeadshot_15:
            case EmoteSubType.CharacterHeadshot_16:
            case EmoteSubType.CharacterHeadshot_17:
            case EmoteSubType.CharacterHeadshot_18:
            case EmoteSubType.CharacterHeadshot_19:
            case EmoteSubType.CharacterHeadshot_20:
                return EmoteType.CharacterHeadshot;

            case EmoteSubType.Opinion_Love:
            case EmoteSubType.Opinion_Like:
            case EmoteSubType.Opinion_Neutral:
            case EmoteSubType.Opinion_Dislike:
            case EmoteSubType.Opinion_Hate:
                return EmoteType.Opinion;
        }

        return InvalidType;
    }

    [SerializeField]
    public string Name;

    [SerializeField]
    public string Description;

    // Has the player uncovered the meaning of this emote?
    //  Mostly to do with name discovery
    [HideInInspector]
    public bool HasDiscovered = false;

    [SerializeField]
    public EmoteType Type;

    [SerializeField]
    public EmoteSubType SubType;

    [SerializeField]
    public Sprite EmoteImage;
}
