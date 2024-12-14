using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class MapGenerator : MonoBehaviour
{

    public int map_size;
    public int random_lateral_deviation;
    public int random_deviation;
    public GameObject tilePrefab;
    public int Offset = 12;

    private int[,] map;

    public GameObject startRoom;
    public GameObject bossRoom;
    public GameObject shopRoom;
    public GameObject[] rooms_east_west;
    public GameObject[] rooms_North_South;

    public GameObject[] REMOVE__rooms;
    public GameObject[] hub_rooms;
    public GameObject[] reward_rooms;
    public GameObject[] corridors;

    public GameObject roomDebug;
    public GameObject corridorDebug;
    public GameObject doorDebug;

    private Random rng;
    private int map_center;

    public double room_probability_decrement = 0.3;


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("MapGenerator Start");

        rng = new Random();

        map = new int[map_size, map_size];
        map_center = map_size / 2+1;

        GenerateMap();
    }

    public void GenerateMap(){
        Debug.Log("Generating map");

        // The map will contain the start room at the center of the map, for a map of 25x25, the start room will be at 12,12
        // Then will generate the boss room and the 3 shop rooms, each one in a different cuadrant of the map
        // Finally will generate the rest of the rooms and connect them with corridors

        // Generate the start room

        // Instantiates the start room prefab at the center of the map
        Instantiate(startRoom, new Vector3(-Offset, 0, Offset), Quaternion.identity);
        
        // Add the start room to the center of the map
        AddToArrayMap(startRoom, map_size/2, map_size/2);       //If map_size is 25, the start room will be at 12,12, with its center at 13,13 which is 0,0 in the scene

        // Generates an array, numbers from 0 to 3 to determine the position of the shop rooms and the boss room.

        int[] positions = new int[4];
        for(int i = 0; i < 4; i++){
            positions[i] = i;
        }
        ShuffleArray(positions);

        //TODO: CAMBIAR PARA QUE HAYA UNA LISTA CON LAS TIENDAS Y JEFE
        //GenerateEastWest(shopRoom, 1);      // East
        //GenerateEastWest(shopRoom, -1);     // West

        GenerateNorthSouth(shopRoom, 1);    // North
        //GenerateNorthSouth(shopRoom, -1);   // South

        DisplayMapDebug();

    }

    private void DisplayMapDebug(){
        for(int i = 0; i < map_size; i++){
            for(int j = 0; j < map_size; j++){
                //instantiates a roobDebug prefab at the position i, j if the value of the map is 1
                //instantiates a corridorDebug prefab at the position i, j if the value of the map is -1
                //instantiates a doorDebug prefab at the position i, j if the value of the map is 2

                if(map[i,j] == 1){
                    Instantiate(roomDebug, new Vector3((j - map_center), -10, (i - map_center)), Quaternion.identity);
                } else if(map[i,j] == -1){
                    Instantiate(corridorDebug, new Vector3((j - map_center), -10, (i - map_center)), Quaternion.identity);
                } else if(map[i,j] == 2){
                    Instantiate(doorDebug, new Vector3((j - map_center), -10, (i - map_center)), Quaternion.identity);
                }
            }
        }
    }

    // Generates the East and West branch of the dungeon
    // branch_end_room --> the room that will be at the end of the branch
    // direction --> 1 for East, -1 for West
    private void GenerateEastWest(GameObject branch_end_room, int direction){

        bool branch_closed = false;

        int x = map_size/2 + 1;
        int z = map_size/2 + 1;

        // Generates the East of the dungeon

        // Generates a room near the start room in the East/West
        x = x + direction * (2 + rng.Next(random_deviation) + 1);
        
        // Z value can be positive or negative
        z = z + rng.Next(random_lateral_deviation) - rng.Next(random_lateral_deviation);

        //TODO: RANDOMIZE THE SELECTION OF THE ROOM

        if(direction < 0){
            // If the direction is negative, the room will be generated in the West taking into account the pivot point of the room (That is in the top left corner)
            x = x - REMOVE__rooms[0].GetComponent<Room>().size_x;
        }

        // Instantiates the room prefab at the position x, z
        Instantiate(REMOVE__rooms[0], new Vector3((x - map_center)*Offset, 0, (z - map_center)*Offset), Quaternion.identity);

        // Add the room to the map
        AddToArrayMap(REMOVE__rooms[0], x, z);

        // Generates a HUB room near the last room in the East/West
        x = x + direction * (REMOVE__rooms[0].GetComponent<Room>().size_x + 2);
        if(direction < 0){
            // If the direction is negative, the room will be generated in the West taking into account the pivot point of the room (That is in the top left corner)
            x = x - REMOVE__rooms[0].GetComponent<Room>().size_x;
        }
        
        Instantiate(hub_rooms[0], new Vector3((x - map_center)*Offset, 0, (map_center - z) * Offset), Quaternion.identity);

        AddToArrayMap(hub_rooms[0], x, z);

        x = x + direction * (hub_rooms[0].GetComponent<Room>().size_x + rng.Next(random_deviation) + 1);
        if(direction < 0){
            // If the direction is negative, the room will be generated in the West taking into account the pivot point of the room (That is in the top left corner)
            x = x - REMOVE__rooms[0].GetComponent<Room>().size_x;
        }
        z = z + rng.Next(random_lateral_deviation) - rng.Next(random_lateral_deviation);

        double room_probability = 0.9;
        bool room_generated = false;

        while(!branch_closed){
            // Generates rooms until the branch is closed
            // Room probability starts at 0.9 (0.1 chance of closing the branch)
            // Every room generated, the probability of closing the branch increases by 0.3

            if(rng.NextDouble() > room_probability){
                branch_closed = true;
                // Create the end room
                Instantiate(branch_end_room, new Vector3((x - map_center)*Offset, 0, (z - map_center)*Offset), Quaternion.identity);

                AddToArrayMap(branch_end_room, x, z);
            } else {
                if (room_generated){
                    // Generates a HUB room
                    Instantiate(hub_rooms[0], new Vector3((x - map_center)*Offset, 0, (z - map_center)*Offset), Quaternion.identity);
                    AddToArrayMap(hub_rooms[0], x, z);

                    x = x + direction * ( hub_rooms[0].GetComponent<Room>().size_x + rng.Next(random_deviation) + 1 );

                    room_generated = false;
                } else {
                    // Generate a room
                    Instantiate(REMOVE__rooms[0], new Vector3((x - map_center)*Offset, 0, (z - map_center)*Offset), Quaternion.identity);
                    AddToArrayMap(REMOVE__rooms[0], x, z);
                    x = x + direction * (REMOVE__rooms[0].GetComponent<Room>().size_x + rng.Next(random_deviation) + 1);
                    room_generated = true;
                }

                if(direction < 0){
                    // If the direction is negative, the room will be generated in the West taking into account the pivot point of the room (That is in the top left corner)
                    x = x - REMOVE__rooms[0].GetComponent<Room>().size_x;
                }
                z = z + rng.Next(random_lateral_deviation) - rng.Next(random_lateral_deviation);
                room_probability = room_probability - room_probability_decrement;
            }

        }


    }

    // Generates the North and South branch of the dungeon
    // branch_end_room --> the room that will be at the end of the branch
    // direction --> 1 for North, -1 for South
    private void GenerateNorthSouth(GameObject branch_end_room, int direction){

        bool branch_closed = false;

        int x = map_size/2 + 1;
        int z = map_size/2 + 1;

        // Generates the East of the dungeon

        // Generates a room near the start room in the East

        z = z + direction * ( 2 + rng.Next(random_deviation) + 1);
        if(direction > 0){
            // If the direction is positive, the room will be generated in the North taking into account the pivot point of the room (That is in the top left corner)
            z = z + REMOVE__rooms[0].GetComponent<Room>().size_y;
        }
        x = x + rng.Next(random_lateral_deviation) - rng.Next(random_lateral_deviation);  
        
        // X value can be positive or negative
        

        //TODO: RANDOMIZE THE SELECTION OF THE ROOM

        // Instantiates the room prefab at the position x, z
        Instantiate(REMOVE__rooms[0], new Vector3((x - map_center)*Offset, 0, (z - map_center)*Offset), Quaternion.identity);

        // Add the room to the map
        AddToArrayMap(REMOVE__rooms[0], x, z);

        // Generates a HUB room near the last room in the East
        z = z + direction * (REMOVE__rooms[0].GetComponent<Room>().size_y + 2);
        if(direction > 0){
            // If the direction is positive, the room will be generated in the North taking into account the pivot point of the room (That is in the top left corner)
            z = z + REMOVE__rooms[0].GetComponent<Room>().size_y;
        }
        x = x + rng.Next(random_lateral_deviation) - rng.Next(random_lateral_deviation);        
        
        Instantiate(hub_rooms[0], new Vector3((x - map_center)*Offset, 0, (z - map_center) * Offset), Quaternion.identity);

        AddToArrayMap(hub_rooms[0], x, z);

        z = z + direction * (hub_rooms[0].GetComponent<Room>().size_y + rng.Next(random_deviation) + 1);
        if(direction > 0){
            // If the direction is positive, the room will be generated in the North taking into account the pivot point of the room (That is in the top left corner)
            z = z + REMOVE__rooms[0].GetComponent<Room>().size_y;
        }    
        x = x + rng.Next(random_lateral_deviation) - rng.Next(random_lateral_deviation);

        double room_probability = 0.9;
        bool room_generated = false;

        while(!branch_closed){
            // Generates rooms until the branch is closed
            // Room probability starts at 0.9 (0.1 chance of closing the branch)
            // Every room generated, the probability of closing the branch increases by 0.3

            if(rng.NextDouble() > room_probability){
                branch_closed = true;
                // Create the end room
                Instantiate(branch_end_room, new Vector3((x - map_center)*Offset, 0, (z - map_center)*Offset), Quaternion.identity);

                AddToArrayMap(branch_end_room, x, z);
            } else {
                if (room_generated){
                    // Generates a HUB room
                    Instantiate(hub_rooms[0], new Vector3((x - map_center)*Offset, 0, (z - map_center)*Offset), Quaternion.identity);
                    AddToArrayMap(hub_rooms[0], x, z);

                    z = z + direction * (hub_rooms[0].GetComponent<Room>().size_y + rng.Next(random_deviation) + 1);

                    room_generated = false;
                } else {
                    // Generate a room
                    Instantiate(REMOVE__rooms[0], new Vector3((x - map_center)*Offset, 0, (z - map_center)*Offset), Quaternion.identity);
                    AddToArrayMap(REMOVE__rooms[0], x, z);
                    z = z + direction * (hub_rooms[0].GetComponent<Room>().size_y + rng.Next(random_deviation) + 1);
                    room_generated = true;
                }

                
                if(direction > 0){
                    // If the direction is positive, the room will be generated in the North taking into account the pivot point of the room (That is in the top left corner)
                    z = z + REMOVE__rooms[0].GetComponent<Room>().size_y;
                } 
                room_probability = room_probability - room_probability_decrement; 
                x = x + rng.Next(random_lateral_deviation) - rng.Next(random_lateral_deviation);
            }

        }


    }

    private void ShuffleArray(int[] array){
        for (int i = array.Length - 1; i > 0; i--)
        {
            int randomIndex = rng.Next(i + 1);
            // Intercambiar los elementos
            (array[randomIndex], array[i]) = (array[i], array[randomIndex]);
        }
    }

    private void AddToArrayMap(GameObject room, int x, int y){
        // Get the size of the room
        int size_x = room.GetComponent<Room>().size_x;
        int size_y = room.GetComponent<Room>().size_y;
        Debug.Log("Adding to map: " + y + ", " + x);
        // Add the room to the map
        for(int i = 0; i < size_y; i++){
            for(int j = 0; j < size_x; j++){
                
                map[y + i, x + j] = 1;
            }
        }

        // If the room is irregular (not square or rectangular), change the values of the map to 0 for the irregular parts
        if(room.GetComponent<Room>().isIrregular){
            
            int[] irregularValues_x = room.GetComponent<Room>().irregularValues_x;
            int[] irregularValues_y = room.GetComponent<Room>().irregularValues_y;

            for(int i = 0; i < irregularValues_x.Length; i++){
                map[y + irregularValues_x[i], x + irregularValues_y[i]] = 0;
            }

        }

        // Add the doors to the map (represented by 2)

        GameObject[] doors = room.GetComponent<Room>().doors;
        int[] doorPositions_x = room.GetComponent<Room>().doorPositions_x;
        int[] doorPositions_y = room.GetComponent<Room>().doorPositions_y;

        for(int i = 0; i < doors.Length; i++){
            map[y + doorPositions_x[i], x + doorPositions_y[i]] = 2;
        }
    }

}
