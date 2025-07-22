using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Willson : Maze
{
   
    List<mapLocation> Notused = new List<mapLocation>();
    public override void Generate()
    {
        int x = Random.Range(1,width-1);
        int z = Random.Range(1, height-1);
        map[x, z] = 2;
        while (GetAC() > 1)
            RandomWalk();
    }

    int countSMN(int x, int z)
    {
       int count = 0;
       foreach(mapLocation mapLocation in m_locations)
        { 
            int nx = x + mapLocation.x;
            int nz = z + mapLocation.z;
            if (map[nx, nz] == 2)
            {
                count++;
            }
        }
        return count;
    }
    int GetAC()
    {
        Notused.Clear();
        for(int z = 1; z < height - 1; z++) 
            for (int x = 1; x < width - 1; x++) 
            { 
                if(countSMN(x, z) == 0)
                {
                    Notused.Add(new mapLocation(x,z));
                }
            }
        return Notused.Count;
       
    }
    void RandomWalk()
    {
        List<mapLocation> inWalk = new List<mapLocation>();
        int cx ;
        int cz ;
        int rstartIndex = Random.Range(0, Notused.Count);
        cx = Notused[rstartIndex].x; 
        cz = Notused[rstartIndex].z;
        inWalk.Add(new mapLocation(cx, cz));

        int countloops = 0;
        bool validpath = false;
        while (cx > 0 && cx < width - 1 && cz > 0 && cz < height - 1 && countloops < 5000&& !validpath)
        {
            map[cx, cz] = 0;
            if (countSMN(cx, cz) > 1)
                break;
            int rd = Random.Range(0, m_locations.Count);
            int nx = cx + m_locations[rd].x;
            int nz = cz + m_locations[rd].z;
           if (CountSquareN(nx, nz) < 2) 
           { 
                cx = nx;
                cz = nz;
                inWalk.Add(new mapLocation(cx, cz));
            }
           validpath = countSMN(cx, cz) == 1;
            countloops++;
            
          
        }
        if (validpath)
        {
            map[cx, cz] = 0;
            foreach (mapLocation loc in inWalk)
            {
                map[loc.x, loc.z] = 2;
            }
            inWalk.Clear();
        }
        else
        {
            foreach (mapLocation loc in inWalk)
            {
                map[loc.x, loc.z] = 1;
            }
            inWalk.Clear();
        }
    }
}
