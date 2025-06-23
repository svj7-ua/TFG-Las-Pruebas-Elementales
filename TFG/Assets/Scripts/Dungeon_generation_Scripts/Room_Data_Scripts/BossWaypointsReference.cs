using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossWaypointsReference : MonoBehaviour
{
    private List<Transform> waypoints;

    // Start is called before the first frame update
    void Start()
    {
        waypoints = new List<Transform>();

        // Buscar el objeto hijo llamado "BossWaypoints"
        Transform bossWaypoints = transform.Find("BossWaypoints");

        if (bossWaypoints != null)
        {
            // Recorrer todos los hijos de BossWaypoints y guardar sus Transforms
            foreach (Transform child in bossWaypoints)
            {
                waypoints.Add(child);
            }
            Debug.Log("Waypoints encontrados: " + waypoints.Count);
        }
        else
        {
            Debug.LogWarning("No se encontró el objeto 'BossWaypoints' como hijo de este GameObject.");
        }
    }

    public List<Transform> GetWaypoints()
    {
        return waypoints; // Return the list of waypoints
    }
}
