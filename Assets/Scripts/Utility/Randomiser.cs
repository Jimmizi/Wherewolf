using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Randomiser
{
    public static int GetRandomIndexFromWeights(List<float> fWeights)
    {
        float fTotal = 0.0f;
        foreach(var w in fWeights)
        {
            fTotal += w;
        }

        float fRandom = UnityEngine.Random.Range(0.0f, fTotal);
        float fWeightSoFar = 0.0f;
        
        for(int i = 0; i < fWeights.Count; ++i)
        {
            fWeightSoFar += fWeights[i];
            if(fRandom < fWeightSoFar)
            {
                return i;
            }
        }

        return 0;
    }

    public static List<int> GetRandomCharacterProcessingOrder()
    {
        List<int> vIndices = new List<int>();
        for(int i = 0; i < ConfigManager.NumberOfCharactersToGenerate; ++i)
        {
            vIndices.Add(i);
        }

        vIndices.Shuffle();
        return vIndices;
    }
}
