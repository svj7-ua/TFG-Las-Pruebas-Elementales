using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{

    public int size_x;      //size of the room in x direction (width/columns)
    public int size_y;      //size of the room in y direction (height/rows)

    public bool isIrregular = false;
    public int[] irregularValues_x;
    public int[] irregularValues_y;

    public GameObject[] doors;
    public int[] doorPositions_x;
    public int[] doorPositions_y;

    



}
