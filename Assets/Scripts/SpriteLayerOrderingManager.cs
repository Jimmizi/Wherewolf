using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteLayerOrderingManager : MonoBehaviour
{
    public List<SpriteOrderer> allSprites = new List<SpriteOrderer>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Service.Population.CharacterCreationDone)
        {
            if(allSprites.Count == 0)
            {
                allSprites.AddRange(GetComponentsInChildren<SpriteOrderer>());
            }

            SortAllSprites();
        }
    }

    void SortAllSprites()
    {
        SortedDictionary<float, SpriteOrderer> byDistance = new SortedDictionary<float, SpriteOrderer>();

        foreach(var s in allSprites)
        {
            if(!s.IsValid())
            {
                continue;
            }

            float fZ = s.GetPositionZ();

            while(byDistance.ContainsKey(fZ))
            {
                fZ += 0.001f;
            }

            byDistance.Add(fZ, s);
        }

        int iSortLayer = 0;

        List<SpriteOrderer> sortedIntoDistance = new List<SpriteOrderer>();
        sortedIntoDistance.AddRange(byDistance.Values);

        for (int i = sortedIntoDistance.Count - 1; i >= 0; --i)
        {
            sortedIntoDistance[i].Base.sortingOrder = iSortLayer++;
            foreach(var child in sortedIntoDistance[i].Children)
            {
                child.sortingOrder = iSortLayer++;
            }
        }

    }
}
