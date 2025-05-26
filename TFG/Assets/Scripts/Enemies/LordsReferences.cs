using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LordsReferences : MonoBehaviour
{

    [SerializeField]
    private GameObject lordOfPoison;
    [SerializeField]
    private GameObject lordOfFire;



    public GameObject GetLordOfPoison()
    {
        return lordOfPoison;
    }

    public GameObject GetLordOfFire()
    {
        return lordOfFire;
    }

    public bool IsLordOfPoisonAlive()
    {
        // Returns if the Lord of poison is active in the scene
        return lordOfPoison != null && lordOfPoison.activeInHierarchy;
    }

    public bool IsLordOfFireAlive()
    {
        // Returns if the Lord of fire is active in the scene
        return lordOfFire != null && lordOfFire.activeInHierarchy;
    }

    // TODO: Methods used to determine if one of the lords is waiting to sync with the other lord

}
