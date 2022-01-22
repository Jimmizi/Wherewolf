using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationManager : MonoBehaviour
{
    // Public vars
    public List<Character> ActiveCharacters = new List<Character>();

    // Private vars
    private int werewolfIndex = -1;
    private Vector2 vScrollPosition = new Vector2();
    private Vector2 vScrollPositionForCounts = new Vector2();
    private Vector2 vScrollPositionForInfo = new Vector2();
    private bool bShowDebug = false;

    // API

    // Get the character for the werewolf
    public Character GetWerewolf() => werewolfIndex > -1 ? ActiveCharacters[werewolfIndex] : null;


    // Internal Functions

    private void Awake()
    {
        Service.Population = this;
    }

    void Start()
    {
        InitialisePopulation();
    }

    void Update()
    {
#if UNITY_EDITOR
        if(Input.GetKeyDown(KeyCode.F1))
        {
            bShowDebug = !bShowDebug;
        }
#endif
    }

#if UNITY_EDITOR

    int iTextHeight = 16;
    int iTextBoxWidth = 250;
    int iTextBoxHeight = 24;
    int iPaddingBetweenCharacters = 24;
    SortedDictionary<Emote.EmoteType, SortedDictionary<Emote.EmoteSubType, int>> subTypeNumberCounts = new SortedDictionary<Emote.EmoteType, SortedDictionary<Emote.EmoteSubType, int>>();

    private void OnGUI()
    {
        if (!bShowDebug) 
        { 
            return; 
        }

        GUI.Box(new Rect(15, 15, 950, 700), "");
        GUI.Label(new Rect(6, 0, 400, 24), "Population Debug");

        if (ActiveCharacters.Count > 0)
        {
            vScrollPosition = GUI.BeginScrollView(new Rect(20, 20, 490, 690), vScrollPosition, new Rect(0, 0, 500, 1200));
            {
                Vector2 vPosition = new Vector2(5, 5);
                RenderCharacterDebug(ref vPosition, GetWerewolf(), werewolfIndex);

                for(int i = 0; i < ActiveCharacters.Count; ++i)
                {
                    if (i == werewolfIndex)
                    {
                        continue;
                    }

                    if(i == 10)
                    {
                        vPosition = new Vector2(5 + iTextBoxWidth + iPaddingBetweenCharacters, 5);
                    }

                    RenderCharacterDebug(ref vPosition, ActiveCharacters[i], i);
                }
            }
            GUI.EndScrollView();

            // First time tally up the numbers
            if (subTypeNumberCounts.Count == 0)
            {
                foreach (var c in ActiveCharacters)
                {
                    foreach (var emoteList in c.Descriptors.Values)
                    {
                        foreach (var emote in emoteList)
                        {
                            if(!subTypeNumberCounts.ContainsKey(emote.Type))
                            {
                                subTypeNumberCounts.Add(emote.Type, new SortedDictionary<Emote.EmoteSubType, int>());
                            }

                            if (!subTypeNumberCounts[emote.Type].ContainsKey(emote.SubType))
                            {
                                subTypeNumberCounts[emote.Type].Add(emote.SubType, 0);
                            }

                            subTypeNumberCounts[emote.Type][emote.SubType]++;
                        }
                    }
                }
            }

            vScrollPositionForCounts = GUI.BeginScrollView(new Rect(510, 20, 250, 690), vScrollPositionForCounts, new Rect(0, 0, 200, 1200));
            {
                Vector2 vPosition = new Vector2(5, 5);
                Character ww = GetWerewolf();

                GUI.Label(new Rect(vPosition.x, vPosition.y, 250, iTextBoxHeight), "(Alive) Matches to Werewolf:");
                vPosition.y += iTextHeight;

                // Show which elements of other characters match against the werewolf
                for (int i = 0; i < ActiveCharacters.Count; ++i)
                {
                    if(i == werewolfIndex)
                    {
                        continue;
                    }

                    Character c = ActiveCharacters[i];

                    if(!c.IsAlive)
                    {
                        continue;
                    }

                    float fNamePosition = vPosition.y;
                    vPosition.y += iTextHeight;

                    bool bAnyMatch = false;

                    for (int t = 0; t <= (int)Character.Descriptor.Clothing; ++t)
                    {
                        var desc = (Character.Descriptor)t;

                        if (c.DoDescriptorsMatch(ref ww, desc))
                        {
                            bAnyMatch = true;

                            GUI.Label(new Rect(vPosition.x + 15, vPosition.y, 225, iTextBoxHeight), string.Format("Matching {0}", desc.ToString()));
                            vPosition.y += iTextHeight;
                        }
                    }

                    if (bAnyMatch)
                    {
                        GUI.Label(new Rect(vPosition.x, fNamePosition, 240, iTextBoxHeight), string.Format("[{0}] {1}", i, c.Name));
                        vPosition.y += iTextHeight;
                    }
                    else
                    {
                        vPosition.y = fNamePosition;
                    }
                }
                                
                foreach (var map in subTypeNumberCounts)
                {
                    GUI.Label(new Rect(vPosition.x, vPosition.y, 150, iTextBoxHeight), map.Key.ToString());
                    vPosition.y += iTextHeight;

                    foreach (var subType in map.Value)
                    {
                        string sKeyName = subType.Key.ToString();
                        string sSubTypeName = sKeyName.Substring(sKeyName.IndexOf("_") + 1);

                        GUI.Label(new Rect(vPosition.x + 15, vPosition.y, 250, iTextBoxHeight), string.Format("{0}: {1}", sSubTypeName, subType.Value));
                        vPosition.y += iTextHeight;
                    }

                    vPosition.y += iPaddingBetweenCharacters;
                }
            }
            GUI.EndScrollView();

            vScrollPositionForInfo = GUI.BeginScrollView(new Rect(760, 20, 190, 690), vScrollPositionForInfo, new Rect(0, 0, 200, 1200));
            {
                Vector2 vPosition = new Vector2(5, 5);

                int iNumAlive = 0;
                foreach(var c in ActiveCharacters)
                {
                    if(c.IsAlive && !c.IsWerewolf)
                    {
                        iNumAlive++;
                    }
                }

                GUI.Label(new Rect(vPosition.x, vPosition.y, 150, iTextBoxHeight), string.Format("Num Alive: {0}", iNumAlive));
            }
            GUI.EndScrollView();
        }
    }

    private void RenderCharacterDebug(ref Vector2 vPos, Character c, int iIndex)
    {
        string MakeStringFromDescriptor(string sLabel, Character.Descriptor eType)
        {
            string str = sLabel + ": ";
            foreach (var emote in c.Descriptors[eType])
            {
                str += emote.Name + " ";
            }
            return str;
        }

        GUI.Label(new Rect(vPos.x, vPos.y, 100, iTextBoxHeight), string.Format("[{0}] {1}", iIndex, c.Name));
        if (c.IsWerewolf)
        {
            GUI.Label(new Rect(vPos.x + 100, vPos.y, 75, iTextBoxHeight), "(Werewolf!)");
        }
        else
        {
            GUI.Label(new Rect(vPos.x + 100, vPos.y, 75, iTextBoxHeight), string.Format("(Alive: {0})", c.IsAlive ? "Y" : "N"));
        }
        vPos.y += iTextHeight;

        GUI.Label(new Rect(vPos.x + 16, vPos.y, iTextBoxWidth, iTextBoxHeight), MakeStringFromDescriptor("Hair", Character.Descriptor.Hair));
        vPos.y += iTextHeight;

        GUI.Label(new Rect(vPos.x + 16, vPos.y, iTextBoxWidth, iTextBoxHeight), MakeStringFromDescriptor("Facial", Character.Descriptor.Facial));
        vPos.y += iTextHeight;

        GUI.Label(new Rect(vPos.x + 16, vPos.y, iTextBoxWidth, iTextBoxHeight), MakeStringFromDescriptor("Occupation", Character.Descriptor.Occupation));
        vPos.y += iTextHeight;

        GUI.Label(new Rect(vPos.x + 16, vPos.y, iTextBoxWidth, iTextBoxHeight), MakeStringFromDescriptor("Clothing", Character.Descriptor.Clothing));
        vPos.y += iTextHeight;

        vPos.y += iPaddingBetweenCharacters;
    }
#endif

    IEnumerator InitialisePopulationStaggered()
    {
        // First generate the werewolf
        AddCharacter(isWerewolf: true);
        yield return new WaitForSeconds(0.04f);

        for (int i = 0; i < 19; ++i)
        {
            AddCharacter();
            yield return new WaitForSeconds(0.04f);
        }
    }

    void InitialisePopulation()
    {
        StartCoroutine(InitialisePopulationStaggered());
    }

    void AddCharacter(bool generateHairStyle = true, bool generateFacial = true, bool generateOccupation = true, bool generateClothing = true, bool isWerewolf = false)
    {
        ActiveCharacters.Add(GenerateCharacter(generateHairStyle, generateFacial, generateOccupation, generateClothing, isWerewolf));
    }

    Character GenerateCharacter(bool generateHairStyle = true, bool generateFacial = true, bool generateOccupation = true, bool generateClothing = true, bool isWerewolf = false)
    {
        Character c = new Character();

        c.IsWerewolf = isWerewolf;
        if(isWerewolf)
        {
            Debug.Assert(werewolfIndex == -1, "Already have a werewolf, and trying to generate another.");
            werewolfIndex = ActiveCharacters.Count;
        }

        // Grab a random name.
        c.Name = Service.InfoManager.GetRandomName();

        for(int i = 0; i <= (int)Character.Descriptor.Clothing; ++i)
        {
            c.Descriptors.Add((Character.Descriptor)i, new List<Emote>());
        }
        
        // Grab one of each type of descriptor

        if (generateHairStyle)
        {
            // Hair Style
            var hs = Service.InfoManager.GetRandomEmoteOfType(Emote.EmoteType.HairStyle);

            if (hs.SubType != Emote.EmoteSubType.HairStyle_Bald)
            {
                c.Descriptors[Character.Descriptor.Hair].Add(Service.InfoManager.GetRandomEmoteOfType(Emote.EmoteType.Color));
            }

            c.Descriptors[Character.Descriptor.Hair].Add(hs);
        }

        if (generateFacial)
        {
            // Facial Feature
            c.Descriptors[Character.Descriptor.Facial].Add(Service.InfoManager.GetRandomEmoteOfType(Emote.EmoteType.FacialFeature));
        }

        if (generateOccupation)
        {
            // Occupation
            c.Descriptors[Character.Descriptor.Occupation].Add(Service.InfoManager.GetRandomEmoteOfType(Emote.EmoteType.Occupation));
        }

        if (generateClothing)
        {
            // Clothing
            c.Descriptors[Character.Descriptor.Clothing].Add(Service.InfoManager.GetRandomEmoteOfType(Emote.EmoteType.Color));
            c.Descriptors[Character.Descriptor.Clothing].Add(Service.InfoManager.GetRandomEmoteOfType(Emote.EmoteType.Clothing));
        }

        return c;
    }



}
