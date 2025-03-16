using System.Collections;
using System.Collections.Generic;
using Graphs;
using UnityEngine;

public class Prim
{


    public static List<Edge> CalculateMinimumSpanningTree(List<Edge> edges, Vertex start){

        HashSet<Vertex> visited = new HashSet<Vertex>();
        HashSet<Vertex> yet_to_visit = new HashSet<Vertex>();

        foreach (Edge edge in edges){
            yet_to_visit.Add(edge.U);
            yet_to_visit.Add(edge.V);
        }

        visited.Add(start);

        List<Edge> result = new List<Edge>();

        while(yet_to_visit.Count>0) {

            bool chosen = false;
            Edge chosenEdge = null;
            float minWeight = float.MaxValue;

            foreach (Edge edge in edges){
                int visitedVerticesCount = 0;

                if (!visited.Contains(edge.U)){
                    visitedVerticesCount++;
                }

                if (!visited.Contains(edge.V)){
                    visitedVerticesCount++;
                }

                if (visitedVerticesCount == 1){
                    float edgeDistance = edge.CalculateDistance();
                    if (edgeDistance < minWeight){
                        chosen = true;
                        chosenEdge = edge;
                        minWeight = edgeDistance;
                    }
                }
            }

            if (chosen){
                result.Add(chosenEdge);
                visited.Add(chosenEdge.U);
                visited.Add(chosenEdge.V);
                yet_to_visit.Remove(chosenEdge.U);
                yet_to_visit.Remove(chosenEdge.V);
            } else {
                break;
            }

        }

        return result;

    }

}
