using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crawler : Maze
{
    public override void Generate()
    {
        for (int i = 0; i < 2; i++)
        {
            crawlV();
        }
        for (int i = 0; i < 3; i++)
        {
            crawlH();
        }

    }
    private void crawlV()
    {
        bool done = false;
        int x = Random.Range(1, width);
        int z = 1;
        while (!done)
        {
            map[x, z] = 0;
            if (Random.Range(0, 2) == 1)
                x += Random.Range(-1, 2);
            else
                z += Random.Range(0, 2);
            done |= (x < 1 || x >= width-1 || z < 1 || z >= height - 1);
        }
    }
    private void crawlH()
    {
        bool done = false;
        int x = 1;
        int z = Random.Range(1, height);
        while (!done)
        {
            map[x, z] = 0;
            if (Random.Range(0, 2) == 1)
                x += Random.Range(0, 2);
            else
                z += Random.Range(-1, 2);
            done |= (x < 1 || x >= width - 1 || z < 1 || z >= height - 1);
        }
    }
}
