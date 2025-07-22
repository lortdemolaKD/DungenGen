using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RDFsearch : Maze
{
 
    public override void Generate()
    {
        Generate(Random.Range(1, width), Random.Range(1, height));
    }
   
    void Generate(int x, int z)
    {
      
        if (CountSquareN(x, z) >= 2) return;
        map[x, z] = 0;

        m_locations.Shuffle();

        Generate(x + m_locations[0].x, z + m_locations[0].z);
        Generate(x + m_locations[1].x, z + m_locations[1].z);
        Generate(x + m_locations[2].x, z + m_locations[2].z);
        Generate(x + m_locations[3].x, z + m_locations[3].z);
    }
}
