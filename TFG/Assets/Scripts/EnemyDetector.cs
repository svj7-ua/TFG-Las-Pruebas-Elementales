using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetector : MonoBehaviour
{

    List<GameObject> enemiesInRange = new List<GameObject>();
    [SerializeField] LayerMask layerMask;

    public GameObject GetClosestEnemy ()
    {
        if(enemiesInRange.Count > 0)
        {
            GameObject bestTarget = null;
            float closestDistanceSqr = Mathf.Infinity;
            Vector3 currentPosition = transform.position;

            foreach (GameObject closestEnemy in enemiesInRange)
            {
                Vector3 directionToTarget = closestEnemy.transform.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;

                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    bestTarget = closestEnemy;
                }
            }

            return bestTarget;
        }
        else 
        { 
            return null; 
        }        
    }

    public List<GameObject> GetEnemiesInRange ()
    {
        return enemiesInRange;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(layerMask == (layerMask | (1 << other.gameObject.layer)))
        {            
            if(enemiesInRange.Count == 0) 
            {
                enemiesInRange.Add(other.gameObject);
            }
            else if (!enemiesInRange.Contains(other.gameObject))
            {
                enemiesInRange.Add(other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (layerMask == (layerMask | (1 << other.gameObject.layer)))
        {
            if (enemiesInRange.Count > 0)
            {
                enemiesInRange.Remove(other.gameObject);
            }
        }
    }

}
