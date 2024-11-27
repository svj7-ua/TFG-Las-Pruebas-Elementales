using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class MapGenerator : MonoBehaviour
{

    public int map_size = 25;
    public GameObject tilePrefab;
    public int Offset = 12;

    private int[,] map;

    public GameObject startRoom;
    public GameObject bossRoom;
    public GameObject shopRoom;
    public GameObject[] rooms;
    public GameObject[] hub_rooms;
    public GameObject[] reward_rooms;
    public GameObject[] corridors;

    private Random rng;
    private int map_center;


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("MapGenerator Start");

        rng = new Random();

        map = new int[map_size, map_size];
        map_center = map_size / 2+1;

        GenerateMap();
    }

    // Update is called once per frame
    void Update()
    {
        
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
        GenerateEast(shopRoom);

        

    }

    private void GenerateEast(GameObject branch_end_room){

        bool branch_closed = false;

        int x = map_size/2 + 1;
        int z = map_size/2 + 1;

        // Generates the East of the dungeon

        // Generates a room near the start room in the East
        x = 2 + rng.Next(3) + 1;
        
        // Z value can be positive or negative
        z = rng.Next(3) - rng.Next(3);

        //TODO: RANDOMIZE THE SELECTION OF THE ROOM

        // Instantiates the room prefab at the position x, z
        Instantiate(rooms[0], new Vector3((x - map_center)*Offset, 0, (map_center - z)*Offset), Quaternion.identity);

        // Add the room to the map
        AddToArrayMap(rooms[0], x, z);

        // Generates a HUB room near the last room in the East
        x = x + rooms[0].GetComponent<Room>().size_x + 2;
        
        Instantiate(hub_rooms[0], new Vector3((x - map_center)*Offset, 0, (map_center - z) * Offset), Quaternion.identity);

        AddToArrayMap(hub_rooms[0], x, z);

        x = x + hub_rooms[0].GetComponent<Room>().size_x + rng.Next(3) + 1;
        z = z + rng.Next(3) - rng.Next(3);

        float room_probability = 0.9f;
        bool room_generated = false;

        while(!branch_closed){
            // Generates rooms until the branch is closed
            // Room probability starts at 0.9 (0.1 chance of closing the branch)
            // Every room generated, the probability of closing the branch increases by 0.3

            if(rng.NextDouble() > room_probability){
                branch_closed = true;
                // Create the end room
                Instantiate(branch_end_room, new Vector3((x - map_center)*Offset, 0, (map_center - z)*Offset), Quaternion.identity);

                AddToArrayMap(branch_end_room, x, z);
            } else {
                if (room_generated){
                    // Generates a HUB room
                    Instantiate(hub_rooms[0], new Vector3((x - map_center)*Offset, 0, (map_center - z)*Offset), Quaternion.identity);
                    AddToArrayMap(hub_rooms[0], x, z);

                    x = x + hub_rooms[0].GetComponent<Room>().size_x + rng.Next(3) + 1;
                    z = z + rng.Next(3) - rng.Next(3);

                    room_generated = false;
                } else {
                    // Generate a room
                    Instantiate(rooms[0], new Vector3((x - map_center)*Offset, 0, (map_center - z)*Offset), Quaternion.identity);
                    AddToArrayMap(rooms[0], x, z);

                    x = x + rooms[0].GetComponent<Room>().size_x + rng.Next(3) + 1;
                    z = z + rng.Next(3) - rng.Next(3);

                    room_generated = true;
                }
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

        // Add the room to the map
        for(int i = 0; i < size_y; i++){
            for(int j = 0; j < size_x; j++){
                Debug.Log("Adding to map: " + (y + i) + ", " + (x + j));
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
