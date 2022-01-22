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
        Gossip
    }

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
        Location_10,

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

        Gossip_RelationshipKiss,
        Gossip_RelationshipFight,
        Gossip_RelationshipMarriage,
    }

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
