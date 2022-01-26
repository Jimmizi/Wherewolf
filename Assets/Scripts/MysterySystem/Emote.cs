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
    }

    // If adding to this, update AddDefaultEmotes() in InformationManager.cs
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
        CharacterHeadshot_12,
        CharacterHeadshot_13,
        CharacterHeadshot_14,
        CharacterHeadshot_15,
        CharacterHeadshot_16,
        CharacterHeadshot_17,
        CharacterHeadshot_18,
        CharacterHeadshot_19,
        CharacterHeadshot_20,
    }

    public static int LocationMin => 0;
    public static int LocationMax => 8;
    public static EmoteSubType GetLocationEnum(int iLocation) => EmoteSubType.Location_1 + iLocation;

    public static EmoteSubType InvalidSubType => (EmoteSubType)(-1);

    [SerializeField]
    public string Name;

    [SerializeField]
    public string Description;

    [SerializeField]
    public EmoteType Type;

    [SerializeField]
    public EmoteSubType SubType;

    [SerializeField]
    public Sprite EmoteImage;
}
