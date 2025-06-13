using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
using Graphs;
using System;
using Unity.AI.Navigation;
using UnityEngine.UI;



public class RoomsGenerator : MonoBehaviour
{
    private GameObject player;

    [SerializeField]
    GameObject floor;

    [SerializeField]
    Vector2Int gridSize;            // The size of the grid where the rooms will be placed. MUST BE ODD NUMBERS, SO THE ORIGIN [0, 0] WILL BE AT THE CENTER.

    [SerializeField]
    int roomAmount;

    [SerializeField]
    List<GameObject> roomPrefabs_2_doors;   // List of room prefabs with 2 doors

    [SerializeField]
    List<GameObject> roomPrefabs_3_doors;   // List of room prefabs with 3 doors

    [SerializeField]
    List<GameObject> roomPrefabs_4_doors;   // List of room prefabs with 4 doors

    //--------------------------------------------------------------------------------------------------------------

    [Space(2)]
    [Header("Rooms Prefabs")]
    // 1 Door Rooms Prefabs
    [SerializeField]
    List<GameObject> Up_roomsPrefabs;   // List of room prefabs with a door in the up position
    [SerializeField]
    List<GameObject> Right_roomsPrefabs;   // List of room prefabs with a door in the right position
    [SerializeField]
    List<GameObject> Down_roomsPrefabs;   // List of room prefabs with a door in the down position
    [SerializeField]
    List<GameObject> Left_roomsPrefabs;   // List of room prefabs with a door in the left position

    // 2 Doors Rooms Prefabs
    [SerializeField]
    List<GameObject> Up_Right_roomsPrefabs;   // List of room prefabs with doors in the up and right positions

    [SerializeField]
    List<GameObject> Up_Left_roomsPrefabs;   // List of room prefabs with doors in the up and left positions

    [SerializeField]
    List<GameObject> Down_Right_roomsPrefabs;   // List of room prefabs with doors in the down and right positions

    [SerializeField]
    List<GameObject> Down_Left_roomsPrefabs;   // List of room prefabs with doors in the down and left positions

    [SerializeField]
    List<GameObject> Up_Down_roomsPrefabs;   // List of room prefabs with doors in the up and down positions

    [SerializeField]
    List<GameObject> Right_Left_roomsPrefabs;   // List of room prefabs with doors in the right and left positions

    // 3 Doors Rooms Prefabs
    [SerializeField]
    List<GameObject> Up_Right_Left_roomsPrefabs;   // List of room prefabs with doors in the up, right and left positions

    [SerializeField]
    List<GameObject> Up_Right_Down_roomsPrefabs;   // List of room prefabs with doors in the up, right and down positions

    [SerializeField]
    List<GameObject> Up_Left_Down_roomsPrefabs;   // List of room prefabs with doors in the up, left and down positions

    [SerializeField]
    List<GameObject> Right_Left_Down_roomsPrefabs;   // List of room prefabs with doors in the right, left and down positions

    // 4 Doors Rooms Prefabs
    [SerializeField]
    List<GameObject> Up_Right_Left_Down_roomsPrefabs;   // List of room prefabs with doors in the up, right, left and down positions

    //--------------------------------------------------------------------------------------------------------------

    [SerializeField]
    GameObject startRoomPrefab;
    [SerializeField]
    List<GameObject> bossRoomPrefab;
    [SerializeField]
    GameObject hallwayPrefab;
    [SerializeField]
    GameObject hallwayCornerPrefab;
    [SerializeField]
    GameObject hallwayTJunctionPrefab;
    [SerializeField]
    GameObject hallwayCrossJunctionPrefab;
    [SerializeField]
    List<GameObject> shopRoomPrefab;

    [Space(2)]
    [Header("Debug Prefabs")]
    [SerializeField]
    GameObject debugRoomPrefab;
    [SerializeField]
    GameObject debugShopOrBossRoomPrefab;

    [SerializeField]
    GameObject debugAirPrefab;

    [SerializeField]
    GameObject debugDoorPrefab;

    [SerializeField]
    GameObject debugHallwayPrefab;

    [SerializeField]
    GameObject debugProtectedPrefab;

    [SerializeField]
    GameObject debugEndHallwayPrefab;

    [SerializeField]
    GameObject debugHallwayPrefab_3x3;


    [Space(2)]
    [Header("Edge Conservation Probability")]
    [SerializeField]
    [Range(0.01f, 1.0f)]
    float EdgeConservationProbability = 0.15f;

    [SerializeField]
    GameObject navMeshPrefab;

    private NavMeshSurface navMeshSurface;

    Random random;
    Grid2D<RoomType> grid;
    List<Room> rooms;

    private int leafsFound = 0;

    //Saves the seed for the random number generator
    private int seed;
    private bool isSeeded = false;

    Delaunay delaunayTriangulation;
    HashSet<Edge> selectedEdges;

    [SerializeField]
    bool debugMode = false;

    public GameObject root;
    private GameObject nav;
    private RunesManager runesManager;

    [Space(2)]
    [Header("MapUI")]
    [SerializeField]
    GameObject mapUI;
    [SerializeField]
    GameObject prefabMapSlot;
    [SerializeField]
    Sprite mapRoom;
    [SerializeField]
    Sprite mapDoor;
    [SerializeField]
    Sprite mapHallway;
    private Grid2D<Image> mapGrid; // The grid that will be used to display the map in the UI    

    void Start(){

        floor.transform.localScale = new Vector3(gridSize.x+10, 1, gridSize.y+10);
        player = GameObject.FindGameObjectWithTag("Player");
        runesManager = GameObject.FindObjectOfType<RunesManager>();
        if (player == null)
        {
            Debug.LogError("Player not found in the scene. Please make sure there is a GameObject with the tag 'Player'.");
            return;
        }
        Generate();

    }

    public void SetSeed(int seed)
    {
        this.seed = seed;
        isSeeded = true;
    }

    public void removeSeed(){
        isSeeded = false;
    }

    public void Generate()
    {

        // Sets the player position to 0,0,0
        player.transform.position = new Vector3(0, 1.2f, 0);
        runesManager.ReloadRunesNotPickedUp();
        GetComponent<LevelInformation>().NextLevel();

        if (root != null)
        {
            Destroy(root); // Destroys the previous root object if it exists
            Destroy(nav); // Destroys the previous navMeshSurface if it exists

        }

        root = new GameObject("RoomsGeneratorRoot");

        if (isSeeded)
        {
            random = new Random(seed);
        }
        else
        {
            random = new Random();                 //TODO: Change this to a random seed later
        }

        Vector2Int origin = new Vector2Int(gridSize.x / 2, gridSize.y / 2);
        Debug.LogWarning("DEBUG: Grid Size: " + gridSize + " Origin: " + origin);

        grid = new Grid2D<RoomType>(gridSize, origin);
        mapGrid = new Grid2D<Image>(gridSize, origin); // Initializes the map grid for the UI
        rooms = new List<Room>();
        leafsFound = 0;

        player.GetComponent<UpdatePlayerPositionInMap_Controller>().ResetGrid(); // Resets the player position in the map UI

        GenerateRoomsLocations();      // Generate the rooms locations
        TriangulateRooms();   // Triangulate the rooms -> This will be done with the Delaunay Triangulation
        SelectHallways();        // Select the edges that will be used to create the corridors -> This will be done with the Prim's Algorithm
        PlaceRooms();           // Place the rooms in the scene -> This will select the prefabs depending on the number of doors needed, which will depend on the edges of each node
        if (debugMode) placeDebugConnectionsBetweenRooms(); // Place the debug connections between the rooms
        mapRooms();             // Map the rooms in the grid
        GenerateHallways();  // Generate the corridors 
        PlaceHallways();        // Place the hallways in the scene
        if (debugMode) printGridInMap();       // Print the grid in the scene -> This will be used for debugging purposes
        PrintMapInUI();
        StartCoroutine(WaitForNavMesh()); // Wait for the nav mesh to be generated

    }

    IEnumerator WaitForNavMesh()
    {
        nav = Instantiate(navMeshPrefab, Vector3.zero, Quaternion.identity);
        nav.SetActive(true);
        navMeshSurface = nav.GetComponent<NavMeshSurface>();
        //yield return new WaitForSeconds(0.1f); // Wait for the navMeshSurface to be initialized
        yield return null;
        //NavMesh.RemoveAllNavMeshData(); // Removes all the previous nav mesh data
        navMeshSurface.BuildNavMesh(); // Builds the nav mesh for the rooms and hallways
    }

    bool isDoor(int x, int y){
        return grid[x, y] == RoomType.N_door || grid[x, y] == RoomType.E_door || grid[x, y] == RoomType.S_door || grid[x, y] == RoomType.W_door;
    }

    void printGridInMap(){
        //if gridsize is 11x11, the origin will be at 5,5 and the grid will be from -5 to 5
        Debug.Log("X: " + gridSize.x/2 + " Y: " + gridSize.y/2 + "MinX: " + (-gridSize.x / 2) + " MinY: " + (-gridSize.y / 2) + " MaxX: " + (gridSize.x / 2) + " MaxY: " + (gridSize.y / 2));
        for(int i = -gridSize.x / 2; i <= gridSize.x / 2; i++){
            for(int j = -gridSize.y / 2; j <= gridSize.y / 2; j++){
                
                if(i== 0 && j == 0){
                    Instantiate(debugShopOrBossRoomPrefab, new Vector3(i, 35, j * -1), Quaternion.identity);
                }

                switch (grid[i, j])
                {
                    case RoomType.none:
                        Instantiate(debugAirPrefab, new Vector3(i, 30, j * -1), Quaternion.identity);
                        break;
                    case RoomType.normalRoom:
                        Instantiate(debugRoomPrefab, new Vector3(i, 30, j * -1), Quaternion.identity);
                        break;
                    case RoomType.shopRoom:
                        Instantiate(debugRoomPrefab, new Vector3(i, 30, j * -1), Quaternion.identity);
                        break;
                    case RoomType.rewardRoom:
                        Instantiate(debugRoomPrefab, new Vector3(i, 30, j * -1), Quaternion.identity);
                        break;
                    case RoomType.bossRoom:
                        Instantiate(debugRoomPrefab, new Vector3(i, 30, j * -1), Quaternion.identity);
                        break;
                    case RoomType.startRoom:
                        Instantiate(debugRoomPrefab, new Vector3(i, 30, j * -1), Quaternion.identity);
                        break;
                    case RoomType.hallway:
                        Instantiate(debugHallwayPrefab, new Vector3(i, 30, j * -1), Quaternion.identity);
                        break;
                    case RoomType.N_door:
                        Instantiate(debugDoorPrefab, new Vector3(i, 30, j * -1), Quaternion.identity);
                        break;
                    case RoomType.E_door:
                        Instantiate(debugDoorPrefab, new Vector3(i, 30, j * -1), Quaternion.identity);
                        break;
                    case RoomType.S_door:
                        Instantiate(debugDoorPrefab, new Vector3(i, 30, j * -1), Quaternion.identity);
                        break;
                    case RoomType.W_door:
                        Instantiate(debugDoorPrefab, new Vector3(i, 30, j * -1), Quaternion.identity);
                        break;

                }
            }
        }
    }

    private void PrintMapInUI()
    {
        // Clears the previous map
        foreach (Transform child in mapUI.transform)
        {
            Destroy(child.gameObject);
        }

        mapGrid = new Grid2D<Image>(gridSize, new Vector2Int(gridSize.x / 2, gridSize.y / 2)); // Reinitializes the map grid for the UI

        // Iterates through the grid and creates a map slot for each position, and adding the map image to the map grid

        for (int i = -gridSize.x / 2; i <= gridSize.x / 2; i++)
        {
            for (int j = -gridSize.y / 2; j <= gridSize.y / 2; j++)
            {
                GameObject mapSlot = Instantiate(prefabMapSlot, mapUI.transform);
                mapSlot.name = "MapSlot_" + i + "_" + j; // Sets the name of the map slot for easier debugging
                Image mapImage = mapSlot.GetComponent<Image>();

                //Sets its color alfa to 0, so it is not visible at the start
                mapImage.color = new Color(Color.white.r, Color.white.g, Color.white.b, 0f);

                mapGrid[j, i] = mapImage; // Store the image in the map grid
            }
        }

        // Iterates through the grid and sets as active the positions not marked as "None" in the map grid
        for (int i = -gridSize.x / 2; i <= gridSize.x / 2; i++)
        {
            for (int j = -gridSize.y / 2; j <= gridSize.y / 2; j++)
            {
                if (grid[j, i] != RoomType.none)
                {
                    //Sets its alfa to its max value
                    mapGrid[j, i].color = new Color(Color.white.r, Color.white.g, Color.white.b, 255);
                }

                if (isDoor(j, i))
                {
                    // If the position is a door, we set the prefabMapDoor as the image
                    mapGrid[j, i].sprite = mapDoor;
                }
                else if (grid[j, i] == RoomType.hallway)
                {
                    // If the position is not a room or a door, we set the prefabMapHallway as the image
                    mapGrid[j, i].sprite = mapHallway;
                }
                else if (grid[j, i] == RoomType.normalRoom)
                {
                    // If the position is a boss room or a shop room, we set the prefabMapRoom as the image
                    mapGrid[j, i].sprite = mapRoom;
                }

            }
        }
        
        player.GetComponent<UpdatePlayerPositionInMap_Controller>().SetGrid(mapGrid); // Sets the map grid in the player controller to update the player position in the map UI

    }

    void GenerateRoomsLocations(){

        int debug_tries = 0;

        PlaceBaseRooms(); // Place the base rooms (start room, boss room and shop rooms)

        // Then we place the rest of the rooms, making sure they don't overlap with each other
        Vector2Int room_needeed_space = new Vector2Int(3, 3);
        for(int i = 0; i < roomAmount;){
            Vector2Int location = new Vector2Int(random.Next(-gridSize.x / 2, gridSize.x / 2), random.Next(-gridSize.y / 2, gridSize.y / 2));

            bool add = true;

            Room newRoom = new Room(room_needeed_space, location);

            foreach(Room room in rooms){
                if(Room.Intersects(newRoom, room)){
                    add = false;
                    break;
                }
            }

            if(newRoom.bounds.xMin < -gridSize.x / 2 || newRoom.bounds.xMax > gridSize.x / 2 
                || newRoom.bounds.yMin < -gridSize.y / 2 || newRoom.bounds.yMax > gridSize.y / 2){
                add = false;
            }

            if(add){
                rooms.Add(newRoom);
                PlaceDebugRoomPrefab(location);
                i++;
            } else {
                debug_tries++;
                Debug.Log("Room not added Room: " + i + " Location: " + location);
                if(debug_tries > 30){
                    Debug.Log("Debug tries exceeded");
                    debug_tries = 0;
                    i++;
                }

            }

        }

        Debug.Log("DEBUG: Rooms Generated: " + rooms.Count);
    }

    // This methos places the start room, the boss room and the 3 shops rooms in the grid
    // The start room will be placed at the center of the grid
    // The boss room and the 3 shops will each be generated in a random position in the grid, but each one will be placed in a different quadrant of the grid
    void PlaceBaseRooms()
    {

        // First, we place the start room. Which will allways be at 0, 0
        PlaceRoom(startRoomPrefab, new Vector2Int(-1, -1));
        PlaceDebugRoomPrefab(new Vector2Int(-1, -1));

        Room startRoom = new Room(new Vector2Int(3, 3), new Vector2Int(-1, -1));
        startRoom.isStartRoom = true;
        startRoom.roomPrefab = startRoomPrefab;
        rooms.Add(startRoom);


    }

    void PlaceRoom(GameObject roomPrefab, Vector2Int position)
    {
        GameObject room = Instantiate(roomPrefab, new Vector3(position.x * 12, 0, position.y * -12), Quaternion.identity);
        // Sets room as a child of the root object
        room.transform.parent = root.transform;
    }

    private void PlaceDebugRoomPrefab(Vector2Int position, bool shopOrBossRoom = false){

        if(!debugMode){
            return;
        }

        //Adapts the x and z coordinates having in mind that the DebugRoomPrefab has its pivot at the center of the object
        int x = (position.x+1) * 12;
        int z = (position.y+1) * -12;
        GameObject room;
        if(shopOrBossRoom)      room = Instantiate(debugShopOrBossRoomPrefab, new Vector3(x, -6, z), Quaternion.identity);
        else                    room = Instantiate(debugRoomPrefab, new Vector3(x, -6, z), Quaternion.identity);
        //Resizes the object to occupy 24x1x24 units
        room.transform.localScale = new Vector3(24, 1, 24);
    }

    void TriangulateRooms(){

        List<Vertex<Room>> vertices = new List<Vertex<Room>>();
        foreach(Room room in rooms){
            vertices.Add(new Vertex<Room>(room.bounds.center, room));
        }

        delaunayTriangulation = Delaunay.Triangulate(vertices);

        Debug.Log("VERY IMPORTANT DEBUG (DELUNAY VERTICES): " + delaunayTriangulation.vertices[0].Item.bounds.center);

    }

    void DEBUG_PrintUnconsideredEdges(HashSet<Edge> unconsideredEdges){
        foreach(Edge edge in unconsideredEdges){
            Vector2 start = new Vector2(edge.U.Position.x*12, edge.U.Position.y*-12);
            Vector2 end = new Vector2(edge.V.Position.x*12, edge.V.Position.y*-12);

            Vector2 midpoint = (start + end) / 2;

            float distance = Vector2.Distance(start, end);

            float angle = Mathf.Atan2(end.y - start.y, end.x - start.x) * Mathf.Rad2Deg;

            GameObject hallway = Instantiate(debugProtectedPrefab, new Vector3(midpoint.x, -6, midpoint.y), Quaternion.identity);

            hallway.transform.localScale = new Vector3(distance, hallway.transform.localScale.y, hallway.transform.localScale.z);

            hallway.transform.rotation = Quaternion.Euler(0, -angle, 0);
        }
    }

    void SelectHallways(){

        List<Edge> edges = new List<Edge>();

        foreach(Edge edge in delaunayTriangulation.edges){
            edges.Add(new Edge(edge.U, edge.V));
        }

        Debug.Log("DEBUG -> Edges Count: " + edges.Count);

        List<Edge> minimumSpanningTree = Prim.CalculateMinimumSpanningTree(edges, edges[0].U);

        selectedEdges = new HashSet<Edge>(minimumSpanningTree);
        Debug.Log("DEBUG -> Selected Edges Count (Minimum Spanning Tree): " + selectedEdges.Count);
        // Adds to selected Edges all the edges that connect to the vertex[0], which is the start room allways
        foreach(Edge edge in edges){
            if(edge.U.Equals(delaunayTriangulation.vertices[0])){
                selectedEdges.Add(edge);
            }
        }

        //Now searches for all the leaves of the minimum spanning tree in order to mark them as leafs and adds them to the selected edges, so they are conserved
        int leafs = 0;
        for(int i=0; i<delaunayTriangulation.vertices.Count && leafs < 4; i++){
            int connectedEdges = 0;
            foreach(Edge edge in selectedEdges){
                if(edge.U.Equals(delaunayTriangulation.vertices[i]) || edge.V.Equals(delaunayTriangulation.vertices[i])){
                    connectedEdges++;
                }

                if(connectedEdges > 1){
                    break;
                }
            }

            if(connectedEdges == 1){
                delaunayTriangulation.vertices[i].Item.isLeaf = true;
                if (leafs == 0)
                {
                    delaunayTriangulation.vertices[i].Item.isBossRoom = true; // The first leaf is the start room
                }
                else
                {
                    delaunayTriangulation.vertices[i].Item.isShopRoom = true; // The rest of the leaves are shop rooms
                }
                leafs++;
            }
        }

        leafsFound = leafs;
        Debug.LogWarning("DEBUG -> Leafs Count: " + leafs);
        List<Edge> remainingEdges = new List<Edge>(edges);

        foreach(Edge edge in selectedEdges){
            remainingEdges.Remove(edge);
        }

        //int debug_protected = 1;
        int leafProtected = 0;
        HashSet<Edge> unconsideredEdges = new HashSet<Edge>();
        foreach (Vertex<Room> vertex in delaunayTriangulation.vertices)
        {
            if (vertex.Item.isLeaf)
            {

                foreach (Edge edge in remainingEdges)
                {
                    if (edge.U.Equals(vertex) || edge.V.Equals(vertex))
                    {
                        unconsideredEdges.Add(edge);
                        //Debug.Log("DEBUG -> Leaf Protected: " + leafProtected);


                    }
                }
                leafProtected++;
                //debug_protected++;

            }
            
            if(leafProtected >= leafs){
                Debug.Log("DEBUG -> All Leaf: " + leafProtected);
                break;
            }

            // if(debug_protected > 1){
            //     break;
            // }
        }

        //DEBUG: --------------------------------------------------------------------------------------------------------------
        if(debugMode) DEBUG_PrintUnconsideredEdges(unconsideredEdges);
        //DEBUG: --------------------------------------------------------------------------------------------------------------


        Debug.Log("DEBUG -> Remaining Edges Count: " + remainingEdges.Count);

        foreach(Edge edge in remainingEdges){

            if(!unconsideredEdges.Contains(edge) && random.NextDouble() < EdgeConservationProbability){
                Debug.Log("DEBUG -> Edge Conserved");
                selectedEdges.Add(edge);
            }
        }

        Debug.Log("DEBUG -> Selected Edges Count (After Edge Conservation): " + selectedEdges.Count);

        for(int i=0; i< delaunayTriangulation.vertices.Count; i++){

            if(delaunayTriangulation.vertices[i].Item.isStartRoom){
                delaunayTriangulation.vertices[i].Item.AddExit(4);
                continue;
            }

            // Checks how many edges are connected to the vertex
            int edgesConnected = 0;
            foreach(Edge edge in selectedEdges){
                if(edge.U.Equals(delaunayTriangulation.vertices[i]) || edge.V.Equals(delaunayTriangulation.vertices[i])){
                    edgesConnected++;

                    if(edge.U.Equals(delaunayTriangulation.vertices[i])){

                        delaunayTriangulation.vertices[i].Item.SetExit(edge.V.Item.bounds);

                    } else {

                        delaunayTriangulation.vertices[i].Item.SetExit(edge.U.Item.bounds);

                    }


                }
            }

            // adds the needed exits to the room
            delaunayTriangulation.vertices[i].Item.AddExit(edgesConnected);

        }

    }

    // This method will Instantiate rectangles between the rooms representing the edges of the delaunay triangulation
    // after the minimum spanning tree has been calculated and the edges have been selected
    void placeDebugConnectionsBetweenRooms(){
        foreach(Edge edge in selectedEdges){
            // Sets the start and end points of the line
            Vector2 start = new Vector2(edge.U.Position.x*12, edge.U.Position.y*-12);
            Vector2 end = new Vector2(edge.V.Position.x*12, edge.V.Position.y*-12);

            // Calculates the centre point
            Vector2 midpoint = (start + end) / 2;

            // Calculates the distance between the two points to enlarge the cube
            float distance = Vector2.Distance(start, end);

            // calculates the angle of rotation between the two points
            float angle = Mathf.Atan2(end.y - start.y, end.x - start.x) * Mathf.Rad2Deg;

            // Instantiates the prefab
            GameObject hallway = Instantiate(debugHallwayPrefab, new Vector3(midpoint.x, -6, midpoint.y), Quaternion.identity);

            // Scale the cube to fit between the two points, connecting them
            hallway.transform.localScale = new Vector3(distance, hallway.transform.localScale.y, hallway.transform.localScale.z);

            // Rotates the cube to fit the angle between the two points
            hallway.transform.rotation = Quaternion.Euler(0, -angle, 0);
        }
    }
    private GameObject ChoosePrefab(Room room){

        bool[] exits = room.GetExits();
        //Order: [up, right, down, left] or [N, E, S, W]

        // 2 Doors Rooms Prefabs
        if (exits[0] && exits[1] && !exits[2] && !exits[3])
        {      // Up, Right doors -> Up_Right_roomsPrefabs      
            return Up_Right_roomsPrefabs[random.Next(0, Up_Right_roomsPrefabs.Count)];
        }
        else if (exits[0] && !exits[1] && exits[2] && !exits[3])
        {     // Up, Down doors -> Up_Down_roomsPrefabs   
            return Up_Down_roomsPrefabs[random.Next(0, Up_Left_roomsPrefabs.Count)];
        }
        else if (exits[0] && !exits[1] && !exits[2] && exits[3])
        {     // Up, Left doors -> Up_Left_roomsPrefabs
            return Up_Left_roomsPrefabs[random.Next(0, Up_Left_roomsPrefabs.Count)];
        }
        else if (!exits[0] && exits[1] && !exits[2] && exits[3])
        {     // Right, Left doors -> right_Left_roomsPrefabs
            return Right_Left_roomsPrefabs[random.Next(0, Right_Left_roomsPrefabs.Count)];
        }
        else if (!exits[0] && exits[1] && exits[2] && !exits[3])
        {      // Down, Right doors -> Down_Right_roomsPrefabs
            return Down_Right_roomsPrefabs[random.Next(0, Down_Right_roomsPrefabs.Count)];
        }
        else if (!exits[0] && !exits[1] && exits[2] && exits[3])
        {     // Down, Left doors -> Down_Left_roomsPrefabs
            return Down_Left_roomsPrefabs[random.Next(0, Down_Left_roomsPrefabs.Count)];
        }

        // 3 Doors Rooms Prefabs
        else if (exits[0] && exits[1] && !exits[2] && exits[3])
        {        // Up, Right, Left doors -> Up_Right_Left_roomsPrefabs
            return Up_Right_Left_roomsPrefabs[random.Next(0, Up_Right_Down_roomsPrefabs.Count)];
        }
        else if (exits[0] && exits[1] && exits[2] && !exits[3])
        {      // Up, Right, Down doors -> Up_Right_Down_roomsPrefabs
            return Up_Right_Down_roomsPrefabs[random.Next(0, Up_Right_Down_roomsPrefabs.Count)];
        }
        else if (exits[0] && !exits[1] && exits[2] && exits[3])
        {       // Up, Left, Down doors -> Up_Left_Down_roomsPrefabs
            return Up_Left_Down_roomsPrefabs[random.Next(0, Up_Left_Down_roomsPrefabs.Count)];
        }
        else if (!exits[0] && exits[1] && exits[2] && exits[3])
        {       // Right, Left, Down doors -> Right_Left_Down_roomsPrefabs
            return Right_Left_Down_roomsPrefabs[random.Next(0, Right_Left_Down_roomsPrefabs.Count)];
        }

        // 1 Door Rooms Prefabs
        else if (exits[0] && !exits[1] && !exits[2] && !exits[3])
        {
            if (room.isBossRoom)
            {
                return bossRoomPrefab[0];
            }
            else if (room.isShopRoom || leafsFound < 4)
            {
                return shopRoomPrefab[0];
            }
            else
            {
                if (leafsFound < 4)
                {
                    leafsFound++;
                    return shopRoomPrefab[0];
                }
                return Up_roomsPrefabs[random.Next(0, Up_roomsPrefabs.Count)];
            }
        }
        else if (!exits[0] && exits[1] && !exits[2] && !exits[3])
        {
            if (room.isBossRoom)
            {
                return bossRoomPrefab[1];
            }
            else if (room.isShopRoom || leafsFound < 4)
            {
                return shopRoomPrefab[1];
            }
            else
            {
                if (leafsFound < 4)
                {
                    leafsFound++;
                    return shopRoomPrefab[1];
                }
                return Right_roomsPrefabs[random.Next(0, Up_roomsPrefabs.Count)];
            }
        }
        else if (!exits[0] && !exits[1] && exits[2] && !exits[3])
        {
            if (room.isBossRoom)
            {
                return bossRoomPrefab[2];
            }
            else if (room.isShopRoom)
            {

                return shopRoomPrefab[2];
            }
            else
            {
                if (leafsFound < 4)
                {
                    leafsFound++;
                    return shopRoomPrefab[2];
                }
                return Down_roomsPrefabs[random.Next(0, Up_roomsPrefabs.Count)];
            }
        }
        else if (!exits[0] && !exits[1] && !exits[2] && exits[3])
        {
            if (room.isBossRoom)
            {
                return bossRoomPrefab[3];
            }
            else if (room.isShopRoom)
            {
                return shopRoomPrefab[3];
            }
            else
            {
                if (leafsFound < 4)
                {
                    leafsFound++;
                    return shopRoomPrefab[3];
                }
                return Left_roomsPrefabs[random.Next(0, Up_roomsPrefabs.Count)];
            }
        }
        else if (exits[0] && exits[1] && exits[2] && exits[3])   // 4 Doors Rooms Prefabs
        {
            return Up_Right_Left_Down_roomsPrefabs[random.Next(0, Up_Right_Left_Down_roomsPrefabs.Count)];
        }
        else
        {
            return new GameObject("EmptyRoom"); // If no exits are found, return an empty room prefab
        }

    }

    void PlaceRooms(){


        for(int i=0; i<delaunayTriangulation.vertices.Count; i++){

            Vector2Int position = new Vector2Int(delaunayTriangulation.vertices[i].Item.bounds.x, delaunayTriangulation.vertices[i].Item.bounds.y);

            if(delaunayTriangulation.vertices[i].Item.isStartRoom){
                PlaceRoom(startRoomPrefab, position);
                Debug.Log("DEBUG -> Room[ " + i + " ]: " + delaunayTriangulation.vertices[i].Item.roomPrefab.name + " assigned as the start room with 4 doors"); 
            }else{
            
                GameObject roomPrefab = ChoosePrefab(delaunayTriangulation.vertices[i].Item);
                PlaceRoom(roomPrefab, position);
                delaunayTriangulation.vertices[i].Item.roomPrefab = roomPrefab;
            
            }
        }

        rooms.Clear();

        // Adds the rooms to the rooms list, now with the room prefabs
        foreach(Vertex<Room> vertex in delaunayTriangulation.vertices){
            if(vertex.Item.roomPrefab == null){
                Debug.Log("DEBUG -> Room prefab is null");
            }
            rooms.Add(vertex.Item);
        }

    }

    void mapRooms(){

        foreach(Room room in rooms){

            for(int i = room.bounds.xMin; i < room.bounds.xMax; i++){
                for(int j = room.bounds.yMin; j < room.bounds.yMax; j++){
                    grid[i, j] = RoomType.normalRoom;
                }
            }

            if (room.roomPrefab == null || room.roomPrefab.GetComponent<RoomDataScript>() == null)
            {
                Debug.Log("Empty room generated");
                continue; // Skip this room if it doesn't have the component
            }
            if (room.roomPrefab.GetComponent<RoomDataScript>().size.x < 3 || room.roomPrefab.GetComponent<RoomDataScript>().size.y < 3)
            {
                // If the room is smaller than 3x3, the unoccupied spaces will be set as "Air"
                // if the room is 2x2 (for example), the positions [0, 0], [0, 1], [1, 0] and [1, 1] will be occupied and the rest will be "Air"

                for (int i = room.bounds.xMin; i < room.bounds.xMax; i++)
                {
                    for (int j = room.bounds.yMin; j < room.bounds.yMax; j++)
                    {
                        if (i < room.bounds.xMin + room.roomPrefab.GetComponent<RoomDataScript>().size.x && j < room.bounds.yMin + room.roomPrefab.GetComponent<RoomDataScript>().size.y)
                        {
                            continue;
                        }
                        grid[i, j] = RoomType.none;
                    }
                }

            }

            if(room.roomPrefab.GetComponent<RoomDataScript>().isIrregular){
                // If the room is irregular sets as "Air" the unoccupied spaces
                foreach(Vector2Int pos in room.roomPrefab.GetComponent<RoomDataScript>().irregularValues){
                    grid[room.bounds.xMin + pos.x, room.bounds.yMin + pos.y] = RoomType.none;
                }
            }

            // Places the doors in the grid
            RoomType[] doorType = new RoomType[] { RoomType.N_door, RoomType.E_door, RoomType.S_door, RoomType.W_door };

            for(int k = 0; k < 4; k++){
                if(room.roomPrefab.GetComponent<RoomDataScript>().doorPositions[k]){
                    grid[room.bounds.xMin + room.roomPrefab.GetComponent<RoomDataScript>().doorPositionsCoordinates[k].x, room.bounds.yMin + room.roomPrefab.GetComponent<RoomDataScript>().doorPositionsCoordinates[k].y] = doorType[k];
                }
            }                     
        }

    }

    bool is_WalkablePosition(int pos_x, int pos_y){
        return grid[pos_x, pos_y] == RoomType.none || grid[pos_x, pos_y] == RoomType.hallway;
    }

    void GenerateHallways(){
        
        // The hallways will be generated using the A* algorithm connecting the rooms with the selected edges
        // This will add them to the grid as hallways and later will be used to place the hallway prefabs in the scene in the PlaceHallways method

        Debug.Log("DEBUG -> Generating Hallways");
        Vector2Int start;
        Vector2Int end;

        foreach(Edge edge in selectedEdges){
            //bool alreadyConnected = false;
            // String debug_str = "";

            start = new Vector2Int(edge.U.Item.bounds.x, edge.U.Item.bounds.y);
            end = new Vector2Int(edge.V.Item.bounds.x, edge.V.Item.bounds.y);
           

            // First, we check in wich direction is the end point in relation to the start point
            // This will be used to determine which exit of the start room and which exit of the end room will be connected

            int x = end.x + (gridSize.x*2) - (start.x + (gridSize.x*2));    // If x is positive, the end point is to the right of the start point
            int y = end.y + (gridSize.y*2) - (start.y + (gridSize.y*2));    // If y is positive, the end point is above the start point

            // Checks which direction is more significant
            if(Math.Abs(x) > Math.Abs(y)){

                // X (Horizontal) is more significant
                
                if(x > 0){
                    
                    Debug.Log("DEBUG (X Relevant) -> X is positive (" + edge.U.Item.roomPrefab.name + ", " + edge.V.Item.roomPrefab.name + ") Values: X=" + x + " Y=" + y + " BOUNDS: end.x=" + end.x + " start.x=" + start.x + " end.y=" + end.y + "start.y=" + start.y);

                    // If x is positive, will connect the right exit of the start room with the left exit of the end room
                    start.x += edge.U.Item.roomPrefab.GetComponent<RoomDataScript>().doorPositionsCoordinates[1].x + 1; // +1 to avoid overlapping with the door and insted set the start point at the first hallway position (right of the door)
                    start.y += edge.U.Item.roomPrefab.GetComponent<RoomDataScript>().doorPositionsCoordinates[1].y;

                    end.x += edge.V.Item.roomPrefab.GetComponent<RoomDataScript>().doorPositionsCoordinates[3].x - 1; // -1 to avoid overlapping with the door and insted set the end point at the last hallway position (left of the door)
                    end.y += edge.V.Item.roomPrefab.GetComponent<RoomDataScript>().doorPositionsCoordinates[3].y;


                } else {
                    
                    Debug.Log("DEBUG (X Relevant) -> X is negative (" + edge.U.Item.roomPrefab.name + ", " + edge.V.Item.roomPrefab.name + ") Values: X=" + x + " Y=" + y + " BOUNDS: end.x=" + end.x + " start.x=" + start.x + " end.y=" + end.y + "start.y=" + start.y);
                    // If x is negative, will connect the left exit of the start room with the right exit of the end room

                    start.x += edge.U.Item.roomPrefab.GetComponent<RoomDataScript>().doorPositionsCoordinates[3].x - 1; // -1 to avoid overlapping with the door and insted set the start point at the last hallway position (left of the door)
                    start.y += edge.U.Item.roomPrefab.GetComponent<RoomDataScript>().doorPositionsCoordinates[3].y;

                    end.x += edge.V.Item.roomPrefab.GetComponent<RoomDataScript>().doorPositionsCoordinates[1].x + 1; // +1 to avoid overlapping with the door and insted set the end point at the first hallway position (right of the door)
                    end.y += edge.V.Item.roomPrefab.GetComponent<RoomDataScript>().doorPositionsCoordinates[1].y;

                }


            } else {

                // Y (Vertical) is more significant
                if(y > 0){
                    
                    Debug.Log("DEBUG (Y Relevant) -> Y is positive (" + edge.U.Item.roomPrefab.name + ", " + edge.V.Item.roomPrefab.name + ") Values: X=" + x + " Y=" + y + " BOUNDS: end.x=" + end.x + " start.x=" + start.x + " end.y=" + end.y + "start.y=" + start.y);
                    // If y is positive, will connect the down exit of the start room with the up exit of the end room

                    start.x += edge.U.Item.roomPrefab.GetComponent<RoomDataScript>().doorPositionsCoordinates[2].x;
                    start.y += edge.U.Item.roomPrefab.GetComponent<RoomDataScript>().doorPositionsCoordinates[2].y + 1; // +1 to avoid overlapping with the door and insted set the start point at the last hallway position (up of the door)

                    end.x += edge.V.Item.roomPrefab.GetComponent<RoomDataScript>().doorPositionsCoordinates[0].x;
                    end.y += edge.V.Item.roomPrefab.GetComponent<RoomDataScript>().doorPositionsCoordinates[0].y - 1; // +1 to avoid overlapping with the door and insted set the end point at the first hallway position (down of the door)

                } else {

                    Debug.Log("DEBUG (Y Relevant) -> Y is negative (" + edge.U.Item.roomPrefab.name + ", " + edge.V.Item.roomPrefab.name + ") Values: X=" + x + " Y=" + y + " BOUNDS: end.x=" + end.x + " start.x=" + start.x + " end.y=" + end.y + "start.y=" + start.y);
                    // If y is negative, will connect the up exit of the start room with the down exit of the end room

                    start.x += edge.U.Item.roomPrefab.GetComponent<RoomDataScript>().doorPositionsCoordinates[0].x;
                    start.y += edge.U.Item.roomPrefab.GetComponent<RoomDataScript>().doorPositionsCoordinates[0].y - 1; // -1 to avoid overlapping with the door and insted set the start point at the last hallway position (down of the door)

                    end.x += edge.V.Item.roomPrefab.GetComponent<RoomDataScript>().doorPositionsCoordinates[2].x;
                    end.y += edge.V.Item.roomPrefab.GetComponent<RoomDataScript>().doorPositionsCoordinates[2].y + 1; // +1 to avoid overlapping with the door and insted set the end point at the first hallway position (up of the door)

                }


            }
            
            List<Vector2Int> path = new List<Vector2Int>();

            if(FindPath(start, end, path)){
                Debug.Log("DEBUG -> Hallway Connected");
                AddPathToGrid(path);
            } else if(FindPath(end, start, path)){
                Debug.Log("DEBUG -> Hallway Connected (Reversed)");
                AddPathToGrid(path);
            } else {
                Debug.LogWarning("ERROR -> Hallway not connected (" + start.x + ", " + start.y + ") to (" + end.x + ", " + end.y + ")");
                
            }

        }

        // Finally marks as hallways all the positions around the start room which is set at [0, 0] and ocupies a 3x3
        for(int i = -2; i <= 2; i++){
            for(int j = -2; j <= 2; j++){
                if(grid[i, j] == RoomType.none){
                    grid[i, j] = RoomType.hallway;
                }
            }
        }

    }

    bool FindPath(Vector2Int start, Vector2Int end, List<Vector2Int> path){
            
            bool end_reached = false;
            bool aux_moved_position = false;
            int debug_counter_exit = 0;
            int x_hallway = start.x;
            int y_hallway = start.y;
            path.Clear();

            while(!end_reached){

                if(x_hallway == end.x && y_hallway == end.y && is_WalkablePosition(x_hallway, y_hallway)){
                    //grid[x_hallway, y_hallway] = RoomType.hallway;
                    path.Add(new Vector2Int(x_hallway, y_hallway));
                    return true;
                }

                if(grid[x_hallway, y_hallway] == RoomType.none){
                    //grid[x_hallway, y_hallway] = RoomType.hallway;
                    path.Add(new Vector2Int(x_hallway, y_hallway));
                }

                if(debug_counter_exit == 100){
                    Debug.Log("Infinite Loop Detected, ended by force at count 50. Stuck at: (" + x_hallway + ", " + y_hallway + ")");
                    if(debugMode){
                        Instantiate(debugProtectedPrefab, new Vector3(x_hallway, 31, y_hallway*-1), Quaternion.identity);
                        Instantiate(debugEndHallwayPrefab, new Vector3(start.x, 31, start.y*-1), Quaternion.identity);
                        Instantiate(debugEndHallwayPrefab, new Vector3(end.x, 31, end.y*-1), Quaternion.identity);
                    }
                    return false;
                }

                aux_moved_position = false;

                // If it is still not in the same line as the end point, will move to the right or left
                if(x_hallway != end.x){
                    
                    // If the end point is to the right of the start point, will move to the right
                    if(end.x + (gridSize.x*2) - (x_hallway + (gridSize.x*2)) > 0 && is_WalkablePosition(x_hallway+1, y_hallway)){
                        
                        x_hallway++;
                        aux_moved_position = true;

                    // If the end point is to the left of the start point, will move to the left
                    } else if (is_WalkablePosition(x_hallway-1, y_hallway)){

                        x_hallway--;
                        aux_moved_position = true;
                    }

                }

                // If it is on the same x line as the end point, will move up or down until it reaches the end point
                if(!aux_moved_position && y_hallway!=end.y){


                    if(end.y + (gridSize.y*2) - (start.y + (gridSize.y*2)) > 0 && is_WalkablePosition(x_hallway, y_hallway+1)){
                        y_hallway++;
                    } else if(is_WalkablePosition(x_hallway, y_hallway-1)){
                        y_hallway--;
                    } else {
                        // It it can't move up or down, will move to the right or left randomly to avoid getting stuck (50% chance)
                        if(random.Next(0, 2) == 0){
                            if(is_WalkablePosition(x_hallway+1, y_hallway)){
                                x_hallway++;
                            }
                        } else {
                            if(is_WalkablePosition(x_hallway-1, y_hallway)){
                                x_hallway--;
                            }
                        }
                    }

                }

               

                debug_counter_exit++;

            }
            return true;
    }

    (Vector2Int, Vector2Int) GenerateHallways_fix(int x, int y, Edge edge, bool option){
        
        Vector2Int start = new Vector2Int(edge.U.Item.bounds.x, edge.U.Item.bounds.y);
        Vector2Int end = new Vector2Int(edge.V.Item.bounds.x, edge.V.Item.bounds.y);
        Debug.Log("DEBUG -> GenerateHallways_fix: x=" + x + " y=" + y + " option=" + option + "START: (" + start.x+ ", " + start.y+ ") END: (" + end.x+ ", " + end.y+ ")");

        if(option){
                            // Y (Vertical) is more significant
                if(y > 0){
                    
                    Debug.Log("DEBUG (Y Relevant) -> Y is positive (" + edge.U.Item.roomPrefab.name + ", " + edge.V.Item.roomPrefab.name + ") Values: X=" + x + " Y=" + y + " BOUNDS: end.x=" + end.x + " start.x=" + start.x + " end.y=" + end.y + "start.y=" + start.y);
                    // If y is positive, will connect the down exit of the start room with the up exit of the end room

                    start.x += edge.U.Item.roomPrefab.GetComponent<RoomDataScript>().doorPositionsCoordinates[2].x;
                    start.y += edge.U.Item.roomPrefab.GetComponent<RoomDataScript>().doorPositionsCoordinates[2].y + 1; // +1 to avoid overlapping with the door and insted set the start point at the last hallway position (up of the door)

                    end.x += edge.V.Item.roomPrefab.GetComponent<RoomDataScript>().doorPositionsCoordinates[0].x;
                    end.y += edge.V.Item.roomPrefab.GetComponent<RoomDataScript>().doorPositionsCoordinates[0].y - 1; // +1 to avoid overlapping with the door and insted set the end point at the first hallway position (down of the door)

                } else {

                    Debug.Log("DEBUG (Y Relevant) -> Y is negative (" + edge.U.Item.roomPrefab.name + ", " + edge.V.Item.roomPrefab.name + ") Values: X=" + x + " Y=" + y + " BOUNDS: end.x=" + end.x + " start.x=" + start.x + " end.y=" + end.y + "start.y=" + start.y);
                    // If y is negative, will connect the up exit of the start room with the down exit of the end room

                    start.x += edge.U.Item.roomPrefab.GetComponent<RoomDataScript>().doorPositionsCoordinates[0].x;
                    start.y += edge.U.Item.roomPrefab.GetComponent<RoomDataScript>().doorPositionsCoordinates[0].y - 1; // -1 to avoid overlapping with the door and insted set the start point at the last hallway position (down of the door)

                    end.x += edge.V.Item.roomPrefab.GetComponent<RoomDataScript>().doorPositionsCoordinates[2].x;
                    end.y += edge.V.Item.roomPrefab.GetComponent<RoomDataScript>().doorPositionsCoordinates[2].y + 1; // +1 to avoid overlapping with the door and insted set the end point at the first hallway position (up of the door)

                }
        } else {
            if(x > 0){
                    
                    Debug.Log("DEBUG (X Relevant) -> X is positive (" + edge.U.Item.roomPrefab.name + ", " + edge.V.Item.roomPrefab.name + ") Values: X=" + x + " Y=" + y + " BOUNDS: end.x=" + end.x + " start.x=" + start.x + " end.y=" + end.y + "start.y=" + start.y);

                    // If x is positive, will connect the right exit of the start room with the left exit of the end room
                    start.x += edge.U.Item.roomPrefab.GetComponent<RoomDataScript>().doorPositionsCoordinates[1].x + 1; // +1 to avoid overlapping with the door and insted set the start point at the first hallway position (right of the door)
                    start.y += edge.U.Item.roomPrefab.GetComponent<RoomDataScript>().doorPositionsCoordinates[1].y;

                    end.x += edge.V.Item.roomPrefab.GetComponent<RoomDataScript>().doorPositionsCoordinates[3].x - 1; // -1 to avoid overlapping with the door and insted set the end point at the last hallway position (left of the door)
                    end.y += edge.V.Item.roomPrefab.GetComponent<RoomDataScript>().doorPositionsCoordinates[3].y;


                } else {
                    
                    Debug.Log("DEBUG (X Relevant) -> X is negative (" + edge.U.Item.roomPrefab.name + ", " + edge.V.Item.roomPrefab.name + ") Values: X=" + x + " Y=" + y + " BOUNDS: end.x=" + end.x + " start.x=" + start.x + " end.y=" + end.y + "start.y=" + start.y);
                    // If x is negative, will connect the left exit of the start room with the right exit of the end room

                    start.x += edge.U.Item.roomPrefab.GetComponent<RoomDataScript>().doorPositionsCoordinates[3].x - 1; // -1 to avoid overlapping with the door and insted set the start point at the last hallway position (left of the door)
                    start.y += edge.U.Item.roomPrefab.GetComponent<RoomDataScript>().doorPositionsCoordinates[3].y;

                    end.x += edge.V.Item.roomPrefab.GetComponent<RoomDataScript>().doorPositionsCoordinates[1].x + 1; // +1 to avoid overlapping with the door and insted set the end point at the first hallway position (right of the door)
                    end.y += edge.V.Item.roomPrefab.GetComponent<RoomDataScript>().doorPositionsCoordinates[1].y;

                }
        }

        return (start, end);

    }

    bool Position_is_within_bounds(int x, int y){
        return x >= -gridSize.x/2 && x < gridSize.x/2 && y >= -gridSize.y/2 && y < gridSize.y/2;
    }

    void PlaceHallways(){

        for(int i = -gridSize.x/2; i < gridSize.x/2; i++){
            for(int j = -gridSize.y/2; j < gridSize.y/2; j++){
                
                int connections = 0;
                bool[] connectedDirections = new bool[4];      // [up, right, down, left] or // [N, E, S, W]
                if (grid[i, j] == RoomType.hallway)
                {

                    if (Position_is_within_bounds(i - 1, j) && (grid[i - 1, j] == RoomType.hallway || grid[i - 1, j] == RoomType.E_door))
                    {
                        connectedDirections[3] = true;
                        connections++;
                    }

                    if (Position_is_within_bounds(i + 1, j) && (grid[i + 1, j] == RoomType.hallway || grid[i + 1, j] == RoomType.W_door))
                    {
                        connectedDirections[1] = true;
                        connections++;
                    }

                    if (Position_is_within_bounds(i, j - 1) && (grid[i, j - 1] == RoomType.hallway || grid[i, j - 1] == RoomType.S_door))
                    {
                        connectedDirections[0] = true;
                        connections++;
                    }

                    if (Position_is_within_bounds(i, j + 1) && (grid[i, j + 1] == RoomType.hallway || grid[i, j + 1] == RoomType.N_door))
                    {
                        connectedDirections[2] = true;
                        connections++;
                    }

                    GameObject hallway = null;

                    switch (connections)
                    {
                        case 2:
                            hallway = Place_Simple_Hallway(i, j, connectedDirections);
                            break;
                        case 3:
                            hallway = Place_T_Hallway(i, j, connectedDirections);
                            break;
                        case 4:
                            hallway = Place_Cross_Hallway(i, j);
                            break;
                        default:
                            Debug.LogError("ERROR -> Hallway not placed correctly, connections: " + connections);
                            break;
                    }

                    if (hallway != null)
                    {
                        // Sets the hallway as a child of root
                        hallway.transform.SetParent(root.transform);
                    }

                }



            }
        }
    }

    GameObject Place_Simple_Hallway(int x, int y, bool[] connections)
    {

        GameObject hallway;

        if (connections[0] && connections[2])
        {
            hallway = Instantiate(hallwayPrefab, new Vector3(x * 12, 0, y * -12), Quaternion.identity);
        }
        else if (connections[1] && connections[3])
        {
            //Rotates it 90 degrees
            hallway = Instantiate(hallwayPrefab, new Vector3(x * 12, 0, y * -12), Quaternion.Euler(0, 90, 0));
        }
        // Is a Corner
        else if (connections[0] && connections[3])
        {
            hallway = Instantiate(hallwayCornerPrefab, new Vector3(x * 12, 0, y * -12), Quaternion.Euler(0, 0, 0)); //
        }
        else if (connections[0] && connections[1])
        {
            hallway = Instantiate(hallwayCornerPrefab, new Vector3(x * 12, 0, y * -12), Quaternion.Euler(0, 90, 0));
        }
        else if (connections[1] && connections[2])
        {
            hallway = Instantiate(hallwayCornerPrefab, new Vector3(x * 12, 0, y * -12), Quaternion.Euler(0, 180, 0)); //
        }
        else
        {
            hallway = Instantiate(hallwayCornerPrefab, new Vector3(x * 12, 0, y * -12), Quaternion.Euler(0, 270, 0)); //
        }
        
        return hallway;
    }

    GameObject Place_T_Hallway(int x, int y, bool[] connections)
    {

        GameObject hallway;

        if (connections[0] && connections[1] && connections[3])
        {
            hallway = Instantiate(hallwayTJunctionPrefab, new Vector3(x * 12, 0, y * -12), Quaternion.identity);
        }
        else if (connections[0] && connections[1] && connections[2])
        {
            hallway = Instantiate(hallwayTJunctionPrefab, new Vector3(x * 12, 0, y * -12), Quaternion.Euler(0, 90, 0));
        }
        else if (connections[1] && connections[2] && connections[3])
        {
            hallway = Instantiate(hallwayTJunctionPrefab, new Vector3(x * 12, 0, y * -12), Quaternion.Euler(0, 180, 0));
        }
        else
        {
            hallway = Instantiate(hallwayTJunctionPrefab, new Vector3(x * 12, 0, y * -12), Quaternion.Euler(0, 270, 0));
        }
        
        return hallway;
    }

    GameObject Place_Cross_Hallway(int x, int y)
    {
        GameObject hallway;
        hallway = Instantiate(hallwayCrossJunctionPrefab, new Vector3(x * 12, 0, y * -12), Quaternion.identity);
        return hallway;
    }

    void CheckAndFixHallways(){

        // Goes through the grid and checks if there are any doors that are not connected to any hallway, if so, they will be connected to the nearest hallway

        for(int i = -gridSize.x/2; i < gridSize.x/2; i++){
            for(int j = -gridSize.y/2; j < gridSize.y/2; j++){
                if(grid[i, j] == RoomType.N_door && grid[i, j-1] == RoomType.none){

                    GoToNearestHallway(i, j-1, i, j+1);
                    
                } else if (grid[i, j] == RoomType.S_door && grid[i, j+1] == RoomType.none){

                    GoToNearestHallway(i, j+1, i, j-1);

                } else if (grid[i, j] == RoomType.W_door && grid[i-1, j] == RoomType.none){

                    GoToNearestHallway(i-1, j, i+1, j);

                } else if (grid[i, j] == RoomType.E_door && grid[i+1, j] == RoomType.none){

                    GoToNearestHallway(i+1, j, i-1, j);

                }
            }
        }
    }

    void GoToNearestHallway(int i, int j, int x, int y){

        int x_goal = 0;
        int y_goal = 0;
        List<Vector2Int> path = new List<Vector2Int>();
        // Will check the positions arround the room [center [x,y] and will connect [i,j] to the first hallway found

        //Checks the 8 positions around the room [x, y]
        bool exit = false;
        for(int k = -1; !exit && k <= 1; k++){
            for(int l = -1;exit && l <= 1; l++){
                if(grid[x+k, y+l] == RoomType.hallway){
                    x_goal = x+k;
                    y_goal = y+l;
                    exit = true;
                }
            }
        }

        if(FindPath(new Vector2Int(i, j), new Vector2Int(x_goal, y_goal), path)){
            AddPathToGrid(path);
        } else {
            Debug.LogError("ERROR -> Path not found (GoToNearestHallway)");
        }

        

    }

    void AddPathToGrid(List<Vector2Int> path){
        foreach(Vector2Int pos in path){
            grid[pos.x, pos.y] = RoomType.hallway;
        }
    }
}
