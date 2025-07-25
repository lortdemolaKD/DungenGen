﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
using Graphs;
using UnityEditor;
using static UnityEditor.FilePathAttribute;


/*public class Generator3D : MonoBehaviour {
    enum CellType {
        None,
        Room,
        Hallway,
        Stairs
    }
    [System.Serializable]
    public class Rule
    {
        public GameObject room;
        public Vector2Int size;
    }
    class Room {
        public BoundsInt bounds;

        public Room(Vector3Int location, Vector3Int size) {
            bounds = new BoundsInt(location, size);
        }

        public static bool Intersect(Room a, Room b) {
            return !((a.bounds.position.x >= (b.bounds.position.x + b.bounds.size.x)) || ((a.bounds.position.x + a.bounds.size.x) <= b.bounds.position.x)
                || (a.bounds.position.y >= (b.bounds.position.y + b.bounds.size.y)) || ((a.bounds.position.y + a.bounds.size.y) <= b.bounds.position.y)
                || (a.bounds.position.z >= (b.bounds.position.z + b.bounds.size.z)) || ((a.bounds.position.z + a.bounds.size.z) <= b.bounds.position.z));
        }
    }

    [SerializeField]
    Vector3Int size;
    [SerializeField]
    int roomCount;
    [SerializeField]
    Vector3Int roomMaxSize;
    [SerializeField]
    Rule[] roomS;
    [SerializeField]
    GameObject cubePrefabR;
    [SerializeField]
    GameObject cubePrefab1;
    [SerializeField]
    Material redMaterial;
    [SerializeField]
    Material blueMaterial;
    [SerializeField]
    Material greenMaterial;

    Randoms random;
    Grid3D<CellType> grid;
    List<Room> rooms;
    Delaunay3D delaunay;
    HashSet<Prim.Edge> selectedEdges;

    void Start() {
        random = new Randoms();
        grid = new Grid3D<CellType>(size, Vector3Int.zero);
        rooms = new List<Room>();

        PlaceRooms();
        Triangulate();
        CreateHallways();
        PathfindHallways();
    }

    void PlaceRooms() {
        for (int i = 0; i < roomCount; i++) {
            Rule roomz = roomS[Random.Range(0, roomS.Length)];
            Vector3Int location = new Vector3Int(
                random.Next(0, size.x),
                random.Next(0, size.y),
                random.Next(0, size.z)
            );

            Vector3Int roomSize = new Vector3Int(
                roomz.size.x,
                1,
                roomz.size.y
            );

            bool add = true;
            Room newRoom = new Room(location, roomSize);
            Room buffer = new Room(location + new Vector3Int(-1, 0, -1), roomSize + new Vector3Int(2, 0, 2));

            foreach (var room in rooms) {
                if (Room.Intersect(room, buffer)) {
                    add = false;
                    break;
                }
            }

            if (newRoom.bounds.xMin < 0 || newRoom.bounds.xMax >= size.x
                || newRoom.bounds.yMin < 0 || newRoom.bounds.yMax >= size.y
                || newRoom.bounds.zMin < 0 || newRoom.bounds.zMax >= size.z) {
                add = false;
            }

            if (add) {
                rooms.Add(newRoom);
                PlaceRoom(newRoom.bounds.position, newRoom.bounds.size,roomz);

                foreach (var pos in newRoom.bounds.allPositionsWithin) {
                    grid[pos] = CellType.Room;
                }
            }
        }
    }

    void Triangulate() {
        List<Vertex> vertices = new List<Vertex>();

        foreach (var room in rooms) {
            vertices.Add(new Vertex<Room>((Vector3)room.bounds.position + ((Vector3)room.bounds.size) / 2, room));
        }

        delaunay = Delaunay3D.Triangulate(vertices);
    }

    void CreateHallways() {
        List<Prim.Edge> edges = new List<Prim.Edge>();

        foreach (var edge in delaunay.Edges) {
            edges.Add(new Prim.Edge(edge.U, edge.V));
        }

        List<Prim.Edge> minimumSpanningTree = Prim.MinimumSpanningTree(edges, edges[0].U);

        selectedEdges = new HashSet<Prim.Edge>(minimumSpanningTree);
        var remainingEdges = new HashSet<Prim.Edge>(edges);
        remainingEdges.ExceptWith(selectedEdges);

        foreach (var edge in remainingEdges) {
            if (random.NextDouble() < 0.125) {
                selectedEdges.Add(edge);
            }
        }
    }

    void PathfindHallways() {
        DungeonPathfinder3D aStar = new DungeonPathfinder3D(size);

        foreach (var edge in selectedEdges) {
            var startRoom = (edge.U as Vertex<Room>).Item;
            var endRoom = (edge.V as Vertex<Room>).Item;

            var startPosf = startRoom.bounds.center;
            var endPosf = endRoom.bounds.center;
            var startPos = new Vector3Int((int)startPosf.x, (int)startPosf.y, (int)startPosf.z);
            var endPos = new Vector3Int((int)endPosf.x, (int)endPosf.y, (int)endPosf.z);

            var path = aStar.FindPath(startPos, endPos, (DungeonPathfinder3D.Node a, DungeonPathfinder3D.Node b) => {
                var pathCost = new DungeonPathfinder3D.PathCost();

                var delta = b.Position - a.Position;

                if (delta.y == 0) {
                    //flat hallway
                    pathCost.cost = Vector3Int.Distance(b.Position, endPos);    //heuristic

                    if (grid[b.Position] == CellType.Stairs) {
                        return pathCost;
                    } else if (grid[b.Position] == CellType.Room) {
                        pathCost.cost += 5;
                    } else if (grid[b.Position] == CellType.None) {
                        pathCost.cost += 1;
                    }

                    pathCost.traversable = true;
                } else {
                    //staircase
                    if ((grid[a.Position] != CellType.None && grid[a.Position] != CellType.Hallway)
                        || (grid[b.Position] != CellType.None && grid[b.Position] != CellType.Hallway)) return pathCost;

                    pathCost.cost = 100 + Vector3Int.Distance(b.Position, endPos);    //base cost + heuristic

                    int xDir = Mathf.Clamp(delta.x, -1, 1);
                    int zDir = Mathf.Clamp(delta.z, -1, 1);
                    Vector3Int verticalOffset = new Vector3Int(0, delta.y, 0);
                    Vector3Int horizontalOffset = new Vector3Int(xDir, 0, zDir);

                    if (!grid.InBounds(a.Position + verticalOffset)
                        || !grid.InBounds(a.Position + horizontalOffset)
                        || !grid.InBounds(a.Position + verticalOffset + horizontalOffset)) {
                        return pathCost;
                    }

                    if (grid[a.Position + horizontalOffset] != CellType.None
                        || grid[a.Position + horizontalOffset * 2] != CellType.None
                        || grid[a.Position + verticalOffset + horizontalOffset] != CellType.None
                        || grid[a.Position + verticalOffset + horizontalOffset * 2] != CellType.None) {
                        return pathCost;
                    }

                    pathCost.traversable = true;
                    pathCost.isStairs = true;
                }

                return pathCost;
            });

            if (path != null) {
                for (int i = 0; i < path.Count; i++) {
                    var current = path[i];

                    if (grid[current] == CellType.None) {
                        grid[current] = CellType.Hallway;
                    }

                    if (i > 0) {
                        var prev = path[i - 1];

                        var delta = current - prev;

                        if (delta.y != 0) {
                            int xDir = Mathf.Clamp(delta.x, -1, 1);
                            int zDir = Mathf.Clamp(delta.z, -1, 1);
                            Vector3Int verticalOffset = new Vector3Int(0, delta.y, 0);
                            Vector3Int horizontalOffset = new Vector3Int(xDir, 0, zDir);
                            
                            grid[prev + horizontalOffset] = CellType.Stairs;
                            grid[prev + horizontalOffset * 2] = CellType.Stairs;
                            grid[prev + verticalOffset + horizontalOffset] = CellType.Stairs;
                            grid[prev + verticalOffset + horizontalOffset * 2] = CellType.Stairs;

                            PlaceStairs(prev + horizontalOffset, prev + verticalOffset + horizontalOffset * 2);
                           // PlaceStairs(prev + horizontalOffset * 2);
                           // PlaceStairs(prev + verticalOffset + horizontalOffset);
                           // PlaceStairs(prev + verticalOffset + horizontalOffset * 2);
                        }

                        Debug.DrawLine(prev + new Vector3(0.5f, 0.5f, 0.5f), current + new Vector3(0.5f, 0.5f, 0.5f), Color.blue, 100, false);
                    }
                }

                foreach (var pos in path) {
                    if (grid[pos] == CellType.Hallway) {
                        PlaceHallway(pos);
                    }
                }
            }
        }
    }

   

    void PlaceRoom(Vector3Int location, Vector3Int size,Rule room) {
   
        GameObject go = Instantiate(room.room, location, Quaternion.identity);
        //go.GetComponent<Transform>().localScale = size;
       
    }

    void PlaceHallway(Vector3Int location) {
        
        GameObject go = Instantiate(cubePrefabR, location, Quaternion.identity);
        go.GetComponent<Transform>().localScale = new Vector3Int(1, 1, 1);
        go.GetComponent<MeshRenderer>().material = blueMaterial;
    }

    void PlaceStairs(Vector3Int location, Vector3Int to) {
        Vector3Int n = location - to;
        GameObject go;
        if (n.y > 0) {
            go = Instantiate(cubePrefab1, location, Quaternion.identity);
        if (n.x != 0)
        {
            switch (n.x)
            {
                case 1:
                    go.transform.Rotate(new Vector3(0f, -90f, 0f));
                        go.transform.position +=  new Vector3(1f, 0f, 0f);
                    break;
                case -1:
                    go.transform.Rotate(new Vector3(0f, 90f, 0f));
                    go.transform.position += new Vector3(0f, 0f, 1f);
                    break;
                default:
                    break;
            }
        }
        if (n.z != 0)
        {
            switch (n.z)
            {
                case 1:
                    go.transform.Rotate(new Vector3(0f, 180f, 0f));
                    go.transform.position += new Vector3(1f, 0f, 1f);
                    break;
                case -1:
                    go.transform.Rotate(new Vector3(0f, 0f, 0f));
                    break;
                default:
                    break;
            }
        }
        }
        else
        {
            go = Instantiate(cubePrefab1, to, Quaternion.identity);
            if (n.x != 0)
            {
                switch (n.x)
                {
                    case 1:
                        go.transform.Rotate(new Vector3(0f, 90f, 0f));
                        go.transform.position += new Vector3(0f, 0f, 1f);
                        break;
                    case -1:
                       go.transform.Rotate(new Vector3(0f, -90f, 0f));
                        go.transform.position += new Vector3(1f, 0f, 0f);
                        break;
                    default:
                        break;
                }
            }
            if (n.z != 0)
            {
                switch (n.z)
                {
                    case 1:
                        go.transform.Rotate(new Vector3(0f, 0f, 0f));
                        break;
                    case -1:
                        go.transform.Rotate(new Vector3(0f, 180f, 0f));
                        go.transform.position += new Vector3(1f, 0f, 1f);
                        break;
                    default:
                        break;
                }
            }
        }
        Debug.DrawLine(location, to, Color.red, 100, false);
        Debug.DrawLine(location , location + new Vector3(0f, 0.5f, 0f), Color.red, 100, false);
        Debug.DrawLine(to, to + new Vector3(0f, 0.5f, 0f), Color.black, 100, false);
        Debug.Log(location+"  "+ to+"  "+ n);
        go.GetComponent<Transform>().localScale = new Vector3Int(1, 1, 1);
       
    }
}
*/
public class Generator3D : MonoBehaviour
{
    enum CellType
    {
        None,
        Room,
        Hallway,
        Stairs
    }

    class Room
    {
        public BoundsInt bounds;

        public Room(Vector3Int location, Vector3Int size)
        {
            bounds = new BoundsInt(location, size);
        }

        public static bool Intersect(Room a, Room b)
        {
            return !((a.bounds.position.x >= (b.bounds.position.x + b.bounds.size.x)) || ((a.bounds.position.x + a.bounds.size.x) <= b.bounds.position.x)
                || (a.bounds.position.y >= (b.bounds.position.y + b.bounds.size.y)) || ((a.bounds.position.y + a.bounds.size.y) <= b.bounds.position.y)
                || (a.bounds.position.z >= (b.bounds.position.z + b.bounds.size.z)) || ((a.bounds.position.z + a.bounds.size.z) <= b.bounds.position.z));
        }
    }

    [SerializeField]
    Vector3Int size;
    [SerializeField]
    int roomCount;
    [SerializeField]
    Vector3Int roomMaxSize;
    [SerializeField]
    GameObject cubePrefab;
    [SerializeField]
    Material redMaterial;
    [SerializeField]
    Material blueMaterial;
    [SerializeField]
    Material greenMaterial;
    [SerializeField]
    Material Material1;
    [SerializeField]
    Material Material2;
    Random random;
    Grid3D<CellType> grid;
    List<Room> rooms;
    Delaunay3D delaunay;
    HashSet<Prim.Edge> selectedEdges;

    void Start()
    {
        random = new Random(0);
        grid = new Grid3D<CellType>(size, Vector3Int.zero);
        rooms = new List<Room>();

        PlaceRooms();
        Triangulate();
        CreateHallways();
        PathfindHallways();
    }

    void PlaceRooms()
    {
        for (int i = 0; i < roomCount; i++)
        {
            Vector3Int location = new Vector3Int(
                random.Next(0, size.x),
                random.Next(0, size.y),
                random.Next(0, size.z)
            );

            Vector3Int roomSize = new Vector3Int(
                random.Next(1, roomMaxSize.x + 1),
                random.Next(1, roomMaxSize.y + 1),
                random.Next(1, roomMaxSize.z + 1)
            );

            bool add = true;
            Room newRoom = new Room(location, roomSize);
            Room buffer = new Room(location + new Vector3Int(-1, 0, -1), roomSize + new Vector3Int(2, 0, 2));

            foreach (var room in rooms)
            {
                if (Room.Intersect(room, buffer))
                {
                    add = false;
                    break;
                }
            }

            if (newRoom.bounds.xMin < 0 || newRoom.bounds.xMax >= size.x
                || newRoom.bounds.yMin < 0 || newRoom.bounds.yMax >= size.y
                || newRoom.bounds.zMin < 0 || newRoom.bounds.zMax >= size.z)
            {
                add = false;
            }

            if (add)
            {
                rooms.Add(newRoom);
                PlaceRoom(newRoom);

                foreach (var pos in newRoom.bounds.allPositionsWithin)
                {
                    grid[pos] = CellType.Room;
                }
            }
        }
    }

    void Triangulate()
    {
        List<Vertex> vertices = new List<Vertex>();

        foreach (var room in rooms)
        {
            vertices.Add(new Vertex<Room>((Vector3)room.bounds.position + ((Vector3)room.bounds.size) / 2, room));
        }

        delaunay = Delaunay3D.Triangulate(vertices);
    }

    void CreateHallways()
    {
        List<Prim.Edge> edges = new List<Prim.Edge>();

        foreach (var edge in delaunay.Edges)
        {
            edges.Add(new Prim.Edge(edge.U, edge.V));
        }

        List<Prim.Edge> minimumSpanningTree = Prim.MinimumSpanningTree(edges, edges[0].U);

        selectedEdges = new HashSet<Prim.Edge>(minimumSpanningTree);
        var remainingEdges = new HashSet<Prim.Edge>(edges);
        remainingEdges.ExceptWith(selectedEdges);

        foreach (var edge in remainingEdges)
        {
            if (random.NextDouble() < 0.125)
            {
                selectedEdges.Add(edge);
            }
        }
    }

    void PathfindHallways()
    {
        DungeonPathfinder3D aStar = new DungeonPathfinder3D(size);

        foreach (var edge in selectedEdges)
        {
            var startRoom = (edge.U as Vertex<Room>).Item;
            var endRoom = (edge.V as Vertex<Room>).Item;

            var startPosf = startRoom.bounds.center;
            var endPosf = endRoom.bounds.center;
            var startPos = new Vector3Int((int)startPosf.x, (int)startPosf.y, (int)startPosf.z);
            var endPos = new Vector3Int((int)endPosf.x, (int)endPosf.y, (int)endPosf.z);

            var path = aStar.FindPath(startPos, endPos, (DungeonPathfinder3D.Node a, DungeonPathfinder3D.Node b) => {
                var pathCost = new DungeonPathfinder3D.PathCost();

                var delta = b.Position - a.Position;

                if (delta.y == 0)
                {
                    //flat hallway
                    pathCost.cost = Vector3Int.Distance(b.Position, endPos);    //heuristic

                    if (grid[b.Position] == CellType.Stairs)
                    {
                        return pathCost;
                    }
                    else if (grid[b.Position] == CellType.Room)
                    {
                        pathCost.cost += 5;
                    }
                    else if (grid[b.Position] == CellType.None)
                    {
                        pathCost.cost += 1;
                    }

                    pathCost.traversable = true;
                }
                else
                {
                    //staircase
                    if ((grid[a.Position] != CellType.None && grid[a.Position] != CellType.Hallway)
                        || (grid[b.Position] != CellType.None && grid[b.Position] != CellType.Hallway)) return pathCost;

                    pathCost.cost = 100 + Vector3Int.Distance(b.Position, endPos);    //base cost + heuristic

                    int xDir = Mathf.Clamp(delta.x, -1, 1);
                    int zDir = Mathf.Clamp(delta.z, -1, 1);
                    Vector3Int verticalOffset = new Vector3Int(0, delta.y, 0);
                    Vector3Int horizontalOffset = new Vector3Int(xDir, 0, zDir);

                    if (!grid.InBounds(a.Position + verticalOffset)
                        || !grid.InBounds(a.Position + horizontalOffset)
                        || !grid.InBounds(a.Position + verticalOffset + horizontalOffset))
                    {
                        return pathCost;
                    }

                    if (grid[a.Position + horizontalOffset] != CellType.None
                        || grid[a.Position + horizontalOffset * 2] != CellType.None
                        || grid[a.Position + verticalOffset + horizontalOffset] != CellType.None
                        || grid[a.Position + verticalOffset + horizontalOffset * 2] != CellType.None)
                    {
                        return pathCost;
                    }

                    pathCost.traversable = true;
                    pathCost.isStairs = true;
                }

                return pathCost;
            });

            if (path != null)
            {
                for (int i = 0; i < path.Count; i++)
                {
                    var current = path[i];

                    if (grid[current] == CellType.None)
                    {
                        grid[current] = CellType.Hallway;
                    }

                    if (i > 0)
                    {
                        var prev = path[i - 1];

                        var delta = current - prev;

                        if (delta.y != 0)
                        {
                            int xDir = Mathf.Clamp(delta.x, -1, 1);
                            int zDir = Mathf.Clamp(delta.z, -1, 1);
                            Vector3Int verticalOffset = new Vector3Int(0, delta.y, 0);
                            Vector3Int horizontalOffset = new Vector3Int(xDir, 0, zDir);

                            grid[prev + horizontalOffset] = CellType.Stairs;
                            grid[prev + horizontalOffset * 2] = CellType.Stairs;
                            grid[prev + verticalOffset + horizontalOffset] = CellType.Stairs;
                            grid[prev + verticalOffset + horizontalOffset * 2] = CellType.Stairs;

                            PlaceStairs(prev + horizontalOffset);
                            PlaceStairs(prev + horizontalOffset * 2);
                            PlaceStairs(prev + verticalOffset + horizontalOffset);
                            PlaceStairs(prev + verticalOffset + horizontalOffset * 2);
                        }

                        Debug.DrawLine(prev + new Vector3(0.5f, 0.5f, 0.5f), current + new Vector3(0.5f, 0.5f, 0.5f), Color.blue, 100, false);
                    }
                }

                foreach (var pos in path)
                {
                    if (grid[pos] == CellType.Hallway)
                    {
                        PlaceHallway(pos);
                    }
                }
            }
        }
    }

    void PlaceCube(Vector3Int location, Vector3Int size, Material material)
    {
        GameObject go = Instantiate(cubePrefab, location, Quaternion.identity);
        go.GetComponent<Transform>().localScale = size;
        go.GetComponent<MeshRenderer>().material = material;
    }

    void PlaceRoom(Room room)
    {
        GameObject floor = new GameObject("Floor");
        MeshRenderer floorRenderer = floor.AddComponent<MeshRenderer>();
        MeshFilter floorFilter = floor.AddComponent<MeshFilter>();
        floorRenderer.material = greenMaterial; // Assign a material to the floor

        // Define the vertices for the floor
        Vector3[] floorVertices = new Vector3[] {
        new Vector3(room.bounds.min.x, room.bounds.min.y, room.bounds.min.z),
        new Vector3(room.bounds.max.x, room.bounds.min.y, room.bounds.min.z),
        new Vector3(room.bounds.max.x, room.bounds.min.y, room.bounds.max.z),
        new Vector3(room.bounds.min.x, room.bounds.min.y, room.bounds.max.z)
    };

        // Define the triangles for the floor
        int[] floorTriangles = new int[] {
        // Triangle 1
        0, 2, 1,
        // Triangle 2
        0, 3, 2
    };

        // Create the mesh for the floor
        Mesh floorMesh = new Mesh();
        floorMesh.vertices = floorVertices;
        floorMesh.triangles = floorTriangles;
        floorMesh.RecalculateNormals(); // Helps with lighting
        floorFilter.mesh = floorMesh; // Assign the mesh to the MeshFilter

        // Repeat the process for the ceiling...
        GameObject ceiling = new GameObject("Ceiling");
        MeshRenderer ceilingRenderer = ceiling.AddComponent<MeshRenderer>();
        MeshFilter ceilingFilter = ceiling.AddComponent<MeshFilter>();
        ceilingRenderer.material = blueMaterial; // Assign a material to the ceiling

        // Use the same vertices for the ceiling but with a different y value
        Vector3[] ceilingVertices = new Vector3[] {
        new Vector3(room.bounds.min.x, room.bounds.max.y, room.bounds.min.z),
        new Vector3(room.bounds.max.x, room.bounds.max.y, room.bounds.min.z),
        new Vector3(room.bounds.max.x, room.bounds.max.y, room.bounds.max.z),
        new Vector3(room.bounds.min.x, room.bounds.max.y, room.bounds.max.z)
    };

        // The triangles will be wound in the opposite order to face downwards
        int[] ceilingTriangles = new int[] {
        // Triangle 1
        0, 1, 2,
        // Triangle 2
        0, 2, 3
    };

        // Create the mesh for the ceiling
        Mesh ceilingMesh = new Mesh();
        ceilingMesh.vertices = ceilingVertices;
        ceilingMesh.triangles = ceilingTriangles;
        ceilingMesh.RecalculateNormals();
        ceilingFilter.mesh = ceilingMesh;

        // Parent the floor and ceiling to the room GameObject for organization
        //floor.transform.parent = room.gameObject.transform;
        //ceiling.transform.parent = room.gameObject.transform;
        //PlaceCube(room.bounds.position, room.bounds.size, redMaterial);
        GenerateWalls(room);
    }
    void GenerateWalls(Room room)
    {
        // Assuming the walls are axis-aligned, we create 4 walls
        Vector3[] corners = new Vector3[] {
        new Vector3(room.bounds.min.x, room.bounds.min.y, room.bounds.min.z),
        new Vector3(room.bounds.max.x, room.bounds.min.y, room.bounds.min.z),
        new Vector3(room.bounds.max.x, room.bounds.min.y, room.bounds.max.z),
        new Vector3(room.bounds.min.x, room.bounds.min.y, room.bounds.max.z)
    };

        float height = room.bounds.size.y;

        for (int i = 0; i < 4; i++)
        {
            GameObject wall = new GameObject("Wall " + i);
            MeshRenderer wallRenderer = wall.AddComponent<MeshRenderer>();
            MeshFilter wallFilter = wall.AddComponent<MeshFilter>();
            wallRenderer.material = Material1; // Assign a material to the wall

            Vector3[] wallVertices = new Vector3[] {
            corners[i],
            corners[(i + 1) % 4],
            corners[(i + 1) % 4] + Vector3.up * height,
            corners[i] + Vector3.up * height
        };

            int[] wallTriangles = new int[] {
            // Triangle 1
            0, 2, 1,
            // Triangle 2
            0, 3, 2
        };

            // Create the mesh for the wall
            Mesh wallMesh = new Mesh();
            wallMesh.vertices = wallVertices;
            wallMesh.triangles = wallTriangles;
            wallMesh.RecalculateNormals(); // Helps with lighting
            wallFilter.mesh = wallMesh; // Assign the mesh to the MeshFilter

            // Parent the wall to the room GameObject for organization
           // wall.transform.parent = room.gameObject.transform;
        }
    }

    void PlaceHallway(Vector3Int location)
    {
        PlaceCube(location, new Vector3Int(1, 1, 1), blueMaterial);
    }

    void PlaceStairs(Vector3Int location)
    {
        PlaceCube(location, new Vector3Int(1, 1, 1), greenMaterial);
    }
}