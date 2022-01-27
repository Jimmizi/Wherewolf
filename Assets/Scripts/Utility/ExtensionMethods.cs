using System;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    public static void Copy<T>(this List<T> l, List<T> other)
    {
        foreach(var e in other)
        {
            l.Add(e);
        }
    }

    private static System.Random rng = new System.Random();
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
