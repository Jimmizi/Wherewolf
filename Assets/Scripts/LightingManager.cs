using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingManager : MonoBehaviour
{
    [SerializeField]
    public Color DayColor;

    [SerializeField]
    public Color NightColor;

    [SerializeField]
    public List<GameObject> ExtraMaterials;

    public List<SpriteRenderer> allSpriteRenderers = new List<SpriteRenderer>();

    public void SetDay()
    {
        foreach(var r in allSpriteRenderers)
        {
            r.color = DayColor;
        }

        foreach (var go in ExtraMaterials)
        {
            go.GetComponent<Renderer>().material.SetColor("_BaseColor", DayColor);
        }
    }

    public void SetNight()
    {
        foreach (var r in allSpriteRenderers)
        {
            var linkedChar = r.GetComponent<CharacterLink>();
            if(linkedChar)
            {
                if (!linkedChar.LinkedCharacter.IsAlive)
                {
                    continue;
                }
            }

            r.color = NightColor;
        }

        foreach (var go in ExtraMaterials)
        {
            go.GetComponent<Renderer>().material.SetColor("_BaseColor", NightColor);
        }
    }

    void Awake()
    {
        Service.Lighting = this;
    }


    void Update()
    {
        if (Service.Population.CharacterCreationDone)
        {
            if(allSpriteRenderers.Count == 0)
            {
                GrabAllRenderers();
            }
        }
    }

    void GrabAllRenderers()
    {
        allSpriteRenderers.AddRange(GetComponentsInChildren<SpriteRenderer>());
    }
}
