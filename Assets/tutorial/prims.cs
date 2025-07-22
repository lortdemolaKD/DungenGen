using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class prims : Maze
{

    public override void Generate()
    {
        int x = 2;
        int z = 2;
        map[x,z] = 0;
        List<mapLocation> walls = new List<mapLocation>();
        walls.Add(new mapLocation(x + 1, z));
        walls.Add(new mapLocation(x - 1, z));
        walls.Add(new mapLocation(x, z + 1));
        walls.Add(new mapLocation(x, z - 1));

        int countloops = 0;
        while (walls.Count > 0 && countloops < 5000)
        {
            int rwall = Random.Range(0, walls.Count);
            x = walls[rwall].x;
            z = walls[rwall].z;
            walls.RemoveAt(rwall);
            if(CountSquareN(x, z) == 1) 
            {
                map[x,z] = 0;
                walls.Add(new mapLocation(x + 1, z));
                walls.Add(new mapLocation(x - 1, z));
                walls.Add(new mapLocation(x, z + 1));
                walls.Add(new mapLocation(x, z - 1));
            }
            countloops++;
        }
    }
}
