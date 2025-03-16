using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid2D<T>{

    T[] data;

    public Vector2Int size {get; private set;}
    public Vector2Int origin {get; private set;}        // Will set the origin [0, 0] of the grid. Expected to be the center of the grid most of the times.
                                                        // It alsa acts as an offset for the grid. So, if the origin is [2, 2] on a 5x5, the grid will be [-2, -2] to [2, 2].
                                                        // Example:
                                                        // [-2,-2] [-1,-2] [0,-2] [1,-2] [2,-2]
                                                        // [-2,-1] [-1,-1] [0,-1] [1,-1] [2,-1]
                                                        // [-2, 0] [-1, 0] [0, 0] [1, 0] [2, 0]
                                                        // [-2, 1] [-1, 1] [0, 1] [1, 1] [2, 1]
                                                        // [-2, 2] [-1, 2] [0, 2] [1, 2] [2, 2]

    public Grid2D(Vector2Int size, Vector2Int origin){
        this.size = size;
        this.origin = origin;
        data = new T[size.x * size.y];
    }

    public int getIndex(Vector2Int position){
        return position.x + (size.x * position.y);
    }

    public bool isInBounds(Vector2Int position){
        return new RectInt(Vector2Int.zero, size).Contains(position + origin);
    }

    /* The following mehtods allow me to access the elements using the notation mapGrid[x, y] or mapGrid[pos] (pos would be a Vector2Int with the x and y coordinates)*/

    public T this[Vector2Int position]{
        get{
            return data[getIndex(position + origin)];       // This will return the value of the grid at the given position. The position is offset by the origin.
        }
        set{
            data[getIndex(position + origin)] = value;      // This will set the value of the grid at the given position. The position is offset by the origin.
        }
    }

    public T this[int x, int y]{
        get{
            return data[getIndex(new Vector2Int(x, y) + origin)];       // This will return the value of the grid at the given position. The position is offset by the origin.
        }
        set{
            data[getIndex(new Vector2Int(x, y) + origin)] = value;      // This will set the value of the grid at the given position. The position is offset by the origin.
        }
    }

}
