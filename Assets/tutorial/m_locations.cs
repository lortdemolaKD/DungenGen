using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class m_locations
{
   private static System.Random _random = new System.Random();
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1) 
        {
            n--; 
            int k = _random.Next(n+ 1);
            (list[n], list[k]) = (list[k], list[n]);
        }
    }
}
