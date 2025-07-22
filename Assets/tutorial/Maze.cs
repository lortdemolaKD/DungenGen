
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class mapLocation 
{ 
    public int x;
    public int z;
    public mapLocation(int _x, int _z)
    {
        x = _x; 
        z = _z;
    }
}

public class Maze : MonoBehaviour
{
    public List<mapLocation> m_locations = new List<mapLocation> {
       new mapLocation(1,0),
       new mapLocation(0,1),
       new mapLocation(-1,0),
       new mapLocation(0,-1)};
    public int width = 30;
    public int height = 30;
    public byte[,] map;
    public int scale = 6;

    public GameObject strait;
    public GameObject cross;
    public GameObject coner;
    public GameObject Tsection;
    public GameObject endpice;

    public GameObject wallpice;
    public GameObject floorpice;
    public GameObject celingpice;

    public GameObject pilar;
    public GameObject door;

    public GameObject FPC;
    // Start is called before the first frame update
    void Start()
    {
        initializeMap();
        Generate();
        CreateRooms(5,2,6);
        DrawMap();
        SpawnPlayer();
    }
    

    public virtual void CreateRooms(int v1, int v2, int v3)
    {
        for (int i = 0; i < v1; i++)
        {
            int startX = Random.Range(3, width -3);
            int startZ = Random.Range(3, height -3);
            int roomWidth = Random.Range(v2, v3);
            int roomDepth = Random.Range(v2, v3);
            for(int x = startX; x < width - 3 && x <startX + roomWidth; x++) 
            {
                for (int z = startZ; z < height - 3 && z < startZ + roomDepth; z++)
                {
                    map[x, z] = 0;
                }
            }
        }
    }

    private void SpawnPlayer()
    {
        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                if (map[x,z] == 0)
                {
                    FPC.transform.position = new Vector3(x* scale, 0, z * scale);
                    return;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void initializeMap()
    {
        map = new byte[width, height];
        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                map[x, z] = 1;//(byte)UnityEngine.Random.Range(0, 2);
            }
        }
    }
    public virtual void Generate()
    {
        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                if(UnityEngine.Random.Range(0,100)> 50)
                    map[x, z] = 0;
            }
        }
    }
    void DrawMap()
    {
        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                if (map[x,z] == 1)
                {
                  //  Vector3 pos = new Vector3(x*scale, 0, z*scale);
                  //  GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                  //  wall.transform.localScale = new Vector3(scale,scale,scale);
                  //  wall.transform.position = pos;
                }
                else  if (search2D(x, z,new int[] {5,0,5,1,0,1,5,0,5}))
                {
                    Vector3 pos = new Vector3(x * scale, 0, z * scale);
                    GameObject wall = Instantiate(strait,pos,Quaternion.identity);
                    
                }
                else if (search2D(x, z, new int[] { 5, 1, 5, 0, 0, 0, 5, 1, 5 }))
                {
                    Vector3 pos = new Vector3(x * scale, 0, z * scale);
                    GameObject wall = Instantiate(strait, pos, Quaternion.identity);
                    wall.transform.Rotate(0, 90, 0);

                }
                else if (search2D(x, z, new int[] { 5, 1, 5, 1, 0, 1, 5, 0, 5 }))// END 
                {
                    Vector3 pos = new Vector3(x * scale, 0, z * scale);
                    GameObject wall = Instantiate(endpice, pos, Quaternion.identity);
                }
                else if (search2D(x, z, new int[] { 5, 0, 5, 1, 0, 1, 5, 1, 5 }))// END Upsidedown
                {
                    Vector3 pos = new Vector3(x * scale, 0, z * scale);
                    GameObject wall = Instantiate(endpice, pos, Quaternion.identity);
                    wall.transform.Rotate(0, 180, 0);
                }
                else if (search2D(x, z, new int[] { 5, 1, 5, 0, 0, 1, 5, 1, 5 }))// -| END
                {
                    Vector3 pos = new Vector3(x * scale, 0, z * scale);
                    GameObject wall = Instantiate(endpice, pos, Quaternion.identity);
                    wall.transform.Rotate(0, 90, 0);
                }
                else if (search2D(x, z, new int[] { 5, 1, 5, 1, 0, 0, 5, 1, 5 }))// |- END
                {
                    Vector3 pos = new Vector3(x * scale, 0, z * scale);
                    GameObject wall = Instantiate(endpice, pos, Quaternion.identity);
                    wall.transform.Rotate(0, -90, 0);
                }
                else if (search2D(x, z, new int[] { 1, 0, 1, 0, 0, 0, 1, 0,1 }))//cross
                {
                    Vector3 pos = new Vector3(x * scale, 0, z * scale);
                    GameObject wall = Instantiate(cross, pos, Quaternion.identity);
                    

                }
                else if (search2D(x, z, new int[] { 5, 1, 5, 0, 0, 1, 1, 0, 5 }))//corner UR
                {
                    Vector3 pos = new Vector3(x * scale, 0, z * scale);
                    GameObject wall = Instantiate(coner, pos, Quaternion.identity);
                    wall.transform.Rotate(0, 180, 0);

                }
                else if (search2D(x, z, new int[] { 5, 1, 5, 1, 0, 0, 5, 0, 1 }))//corner UL
                {
                    Vector3 pos = new Vector3(x * scale, 0, z * scale);
                    GameObject wall = Instantiate(coner, pos, Quaternion.identity);
                    wall.transform.Rotate(0, 90, 0);

                }
                else if (search2D(x, z, new int[] { 5, 0, 1, 1, 0, 0, 5, 1, 5 }))//corner DR
                {
                    Vector3 pos = new Vector3(x * scale, 0, z * scale);
                    GameObject wall = Instantiate(coner, pos, Quaternion.identity);
                    

                }
                else if (search2D(x, z, new int[] { 1, 0, 5, 0, 0, 1, 5, 1, 5 }))//corner DL
                {
                    Vector3 pos = new Vector3(x * scale, 0, z * scale);
                    GameObject wall = Instantiate(coner, pos, Quaternion.identity);
                    wall.transform.Rotate(0, -90, 0);

                }
                else if (search2D(x, z, new int[] { 1, 0, 1, 0, 0, 0, 5, 1, 5 }))// T Upsidedown
                {
                    Vector3 pos = new Vector3(x * scale, 0, z * scale);
                    GameObject wall = Instantiate(Tsection, pos, Quaternion.identity);
                    

                }
                else if (search2D(x, z, new int[] { 5, 1, 5, 0, 0, 0, 1, 0, 1 }))// T
                {
                    Vector3 pos = new Vector3(x * scale, 0, z * scale);
                    GameObject wall = Instantiate(Tsection, pos, Quaternion.identity);
                    wall.transform.Rotate(0, 180, 0);

                }
                else if (search2D(x, z, new int[] { 1,0,5,0,0,1,1,0,5 }))// -|
                {
                    Vector3 pos = new Vector3(x * scale, 0, z * scale);
                    GameObject wall = Instantiate(Tsection, pos, Quaternion.identity);
                    wall.transform.Rotate(0, -90, 0);


                }
                else if (search2D(x, z, new int[] { 5,0,1,1,0,0,5,0,1 }))// |--
                {
                    Vector3 pos = new Vector3(x * scale, 0, z * scale);
                    GameObject wall = Instantiate(Tsection, pos, Quaternion.identity);
                    wall.transform.Rotate(0, 90, 0);
                }
                else if (map[x, z] == 0 && (CountSquareN(x,z) >1 && CountDiagnalN(x, z)>= 1 ||
                    CountSquareN(x, z) >= 1 && CountDiagnalN(x, z) > 1))// |--
                {
                   
                    GameObject floor = Instantiate(floorpice);
                    floor.transform.position = new Vector3(x * scale, 0,z * scale);
                    floor.name +="_room";
                    GameObject ciling = Instantiate(celingpice); 
                    ciling.transform.position = new Vector3(x * scale, 0, z * scale);
                    GameObject pilacorner;

                    locateWall(x, z);
                    if (top)
                    {
                        floor.name = floor.name + "__t";
                        GameObject wall_1 = Instantiate(wallpice);
                        wall_1.transform.position = new Vector3(x * scale, 0, z * scale);
                        wall_1.transform.Rotate(0,-90,0);
                        if (map[x+1,z] == 0 && map[x + 1, z+1] == 0)
                        {
                            pilacorner = Instantiate(pilar);
                            pilacorner.transform.position = new Vector3(x * scale, 0, z * scale);
                            pilacorner.name = "R-P";
                        }
                        if (map[x - 1, z] == 0 && map[x - 1, z + 1] == 0)
                        {
                            pilacorner = Instantiate(pilar);
                            pilacorner.transform.position = new Vector3((x-1) * scale, 0, z * scale);
                            
                            pilacorner.name = "L-P";
                        }
                    }
                    if (bottom)
                    {
                        floor.name = floor.name + "__b";
                        GameObject wall_2 = Instantiate(wallpice);
                        wall_2.transform.position = new Vector3(x * scale, 0, z * scale);
                        wall_2.transform.Rotate(0, 90, 0);
                        if (map[x + 1, z] == 0 && map[x + 1, z - 1] == 0)
                        {
                            pilacorner = Instantiate(pilar);
                            pilacorner.transform.position = new Vector3(x * scale, 0, z * scale);
                            pilacorner.name = "R-P";
                            pilacorner.transform.Rotate(0, 90, 0);
                        }
                        if (map[x - 1, z] == 0 && map[x - 1, z - 1] == 0)
                        {
                            pilacorner = Instantiate(pilar);
                            pilacorner.transform.position = new Vector3((x - 1) * scale, 0, z * scale);
                            pilacorner.name = "L-P";
                            pilacorner.transform.Rotate(0, 90, 0);
                        }
                    }
                    if (right)
                    {
                        floor.name = floor.name + "__r";
                        GameObject wall_3 = Instantiate(wallpice);
                        wall_3.transform.position = new Vector3(x * scale, 0, z * scale);

                        if (map[x, z+1] == 0 && map[x + 1, z + 1] == 0)
                        {
                            pilacorner = Instantiate(pilar);
                            pilacorner.transform.position = new Vector3(x * scale, 0, z * scale);
                            pilacorner.name = "R-P";
                            
                        }
                        if (map[x , z-1] == 0 && map[x + 1, z - 1] == 0)
                        {
                            pilacorner = Instantiate(pilar);
                            pilacorner.transform.position = new Vector3(x  * scale, 0, z * scale);
                            pilacorner.name = "L-P";
                            pilacorner.transform.Rotate(0, 90, 0);
                        }
                    }
                    if (left)
                    {
                            floor.name = floor.name + "__l";
                        GameObject wall_4 = Instantiate(wallpice);
                        wall_4.transform.position = new Vector3(x * scale, 0, z * scale);
                        wall_4.transform.Rotate(0, 180, 0);
                        if (map[x, z + 1] == 0 && map[x - 1, z + 1] == 0)
                        {
                            pilacorner = Instantiate(pilar);
                            pilacorner.transform.position = new Vector3(x * scale, 0, z * scale);
                            pilacorner.name = "R-P";
                            pilacorner.transform.Rotate(0, -90, 0);
                        }
                        if (map[x, z - 1] == 0 && map[x - 1, z - 1] == 0)
                        {
                            pilacorner = Instantiate(pilar);
                            pilacorner.transform.position = new Vector3(x * scale, 0, z * scale);
                            pilacorner.name = "L-P";
                            pilacorner.transform.Rotate(0, 180, 0);
                        }
                    }
                    GameObject doorway;
                    locateDoors(x,z);
                    if (top_D)
                    {
                        doorway = Instantiate(door);
                        doorway.transform.position = new Vector3(x * scale, 0, z * scale);
                        //doorway.transform.Rotate(0, -90, 0);
                    }
                    if (bottom_D)
                    {
                        doorway = Instantiate(door);
                        doorway.transform.position = new Vector3(x * scale, 0, z * scale);
                        doorway.transform.Rotate(0,180, 0);
                    }
                    if (right_D)
                    {
                        doorway = Instantiate(door);
                        doorway.transform.position = new Vector3(x * scale, 0, z * scale);
                        doorway.transform.Rotate(0, 90, 0);

                    }
                    if (left_D)
                    {
                        doorway = Instantiate(door);
                        doorway.transform.position = new Vector3(x * scale, 0, z * scale);
                        doorway.transform.Rotate(0, -90, 0);
                    }
                }
                else
                {
                    GameObject floor = Instantiate(floorpice);
                    floor.transform.position = new Vector3(x * scale, 0, z * scale);
                    floor.name += CountSquareN(x,z) + CountDiagnalN(x, z).ToString() + map[x, z].ToString();
                    floor.GetComponent<MeshRenderer>().materials[0].color = Color.red;
                    GameObject ciling = Instantiate(celingpice);
                    ciling.transform.position = new Vector3(x * scale, 0, z * scale);
                    //Vector3 pos = new Vector3(x*scale, 0, z*scale);
                  // GameObject block = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                  //  block.transform.localScale = new Vector3(scale,scale,scale);
                  //  block.transform.position = pos;
                }

                Debug.Log(x+"  "+z);

            }
        }
    }
    bool top;
    bool bottom;
    bool right;
    bool left;
    bool top_D;
    bool bottom_D;
    bool right_D;
    bool left_D;

    public void locateWall(int x,int z)
    {
        top = false;
        bottom = false;   
        right = false;
        left = false;

        if (x <= 0 || x >= width-1 || z <= 0 || z >= height-1) return;

        if (map[x,z + 1] == 1) top = true;
        if (map[x, z - 1] == 1) bottom = true;
        if (map[x + 1, z ] == 1) right = true;
        if (map[x - 1, z ] == 1) left = true;
        
    }
    public void locateDoors(int x, int z)
    {
        top_D = false;
        bottom_D = false;
        right_D = false;
        left_D = false;

        if (x <= 0 || x >= width - 1 || z <= 0 || z >= height - 1) return;

        /*if (map[x, z + 1] == 0 && ((map[x - 1, z + 1] == 1 && map[x + 1, z + 1] == 1) 
            || (map[x - 1, z + 1] == 0 && map[x + 1, z + 1] == 1) 
            || (map[x - 1, z + 1] == 1 && map[x + 1, z + 1] == 0))) top_D = true;*/
        //search2D(x, z, new int[] { 0,0,1,1,0,0,5,5,5 })



        if (map[x, z + 1] == 0 && ((map[x - 1, z + 1] == 1 && map[x + 1, z + 1] == 1)
            || search2D(x, z, new int[] { 0, 0, 1, 1, 0, 0, 5, 5, 5 }) 
            || search2D(x, z, new int[] { 1, 0, 0, 0, 0, 1, 5, 5, 5 })) ) top_D = true;
        if (map[x, z - 1] == 0 && ((map[x - 1, z - 1] == 1 && map[x + 1, z - 1] == 1)
            || search2D(x, z, new int[] { 5, 5, 5, 1, 0, 0, 0, 0, 1 })
            || search2D(x, z, new int[] { 5, 5, 5, 0, 0, 1, 1, 0, 0 })) ) bottom_D = true;
        if (map[x + 1, z] == 0 && ((map[x + 1, z + 1] == 1 && map[x + 1, z - 1] == 1)
            || search2D(x, z, new int[] { 5, 1, 0, 5, 0, 0, 5, 0, 1 })
            || search2D(x, z, new int[] { 5, 0, 1, 5, 0, 0, 5, 1, 0 })) ) right_D = true;
        if (map[x - 1, z] == 0 && ((map[x - 1, z + 1] == 1 && map[x - 1, z - 1] == 1)
            || search2D(x, z, new int[] { 1, 0, 5, 0, 0, 5, 0, 1, 5 })
            || search2D(x, z, new int[] { 0, 1, 5, 0, 0, 5, 1, 0, 5 })) ) left_D = true;

    }
    bool search2D(int c,int r, int[] pattern)
    {
        int count = 0;
        int pos = 0;
        for(int z = 1; z > -2; z--)
        {
            for(int x = -1;x < 2;x++)
            {
                if (pattern[pos] == map[c + x, r + z] || pattern[pos] == 5)
                    count++;
                pos++;
            }
        }
        return ( count == 9);
    }

    public int CountSquareN(int x ,int z)
    {
        int count = 0;
        if (x <= 0 || x >= width - 1 || z <= 0 || z >= height - 1) return 5;
        if (map[x-1,z] == 0) count++;
        if (map[x + 1, z] == 0) count++;
        if (map[x, z + 1] == 0) count++;
        if (map[x, z - 1] == 0) count++;
        return count;

    }
    public int CountDiagnalN(int x, int z)
    {
        int count = 0;
        if (x <= 0 || x >= width - 1 || z <= 0 || z >= height - 1) return 5;
        if (map[x - 1, z-1] == 0) count++;
        if (map[x + 1, z-1] == 0) count++;
        if (map[x - 1, z + 1] == 0) count++;
        if (map[x + 1, z + 1] == 0) count++;
        return count;

    }
    public int CountAllN(int x, int z)
    {
       
        return CountSquareN(x, z) + CountDiagnalN(x,z);
       

    }
}
