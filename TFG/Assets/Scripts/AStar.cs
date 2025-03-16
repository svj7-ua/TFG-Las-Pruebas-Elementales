using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Graphs;

public class AStar
{


/*     private List<Vector2Int> CalculatePath(Grid2D<RoomType> grid, Vector2Int start, Vector2Int end)
    {
        List<Vector2Int> path = new List<Vector2Int>();

        List<Vector2Int> openSet = new List<Vector2Int>();
        List<Vector2Int> closedSet = new List<Vector2Int>();

        openSet.Add(start);

        while (openSet.Count > 0)
        {
            Vector2Int current = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i] == current)
                {
                    current = openSet[i];
                }
            }

            openSet.Remove(current);
            closedSet.Add(current);

            if (current == end)
            {
                Vector2Int temp = current;
                path.Add(temp);
                while (temp != start)
                {
                    temp = grid[temp].parent;
                    path.Add(temp);
                }
                path.Reverse();
                break;
            }

            foreach (Vector2Int neighbour in grid[current].neighbours)
            {
                if (closedSet.Contains(neighbour) || grid[neighbour].type == RoomType.none)
                {
                    continue;
                }

                int newMovementCostToNeighbour = grid[current].gCost + GetDistance(current, neighbour);
                if (newMovementCostToNeighbour < grid[neighbour].gCost || !openSet.Contains(neighbour))
                {
                    grid[neighbour].gCost = newMovementCostToNeighbour;
                    grid[neighbour].hCost = GetDistance(neighbour, end);
                    grid[neighbour].parent = current;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }
        }

        return path;
    } */

}
