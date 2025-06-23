using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnBossController : MonoBehaviour
{

    [SerializeField] private LayerMask layerMask = 10; // Layer mask of the player
    [SerializeField] private GameObject spawnText; // Boss prefab to spawn
    [SerializeField] private GameObject nextLevelText; // Reference to the pick up text pop up prefab so we can activate it when the player is in range of the item
    [SerializeField] private GameObject door; // Boss prefab to spawn
    private  BossRoomManagerScript bossRoomManagerScript; // Boss room manager script

    [SerializeField] private GameObject SummonCircle; // Boss prefab to spawn

    private bool bossSummoned = false; // Flag to check if the boss has been summoned
    private bool bossKilled = false; // Flag to check if the boss has been killed

    private bool isInTrigger = false; // Flag to check if the player is in the trigger    
    void Start()
    {

        bossRoomManagerScript = GetComponentInParent<BossRoomManagerScript>(); // Gets the boss room manager script
        SummonCircle.GetComponent<Animator>().enabled = false; // Enables the animator for the summon circle
        
        SummonCircle.GetComponent<SpriteRenderer>().sprite = bossRoomManagerScript.GetBossSummoningCircleSprite(); // Sets the sprite of the summon circle
        
        spawnText.SetActive(false); // Deactivates the spawn text
        if (bossRoomManagerScript == null)
        {
            Debug.LogError("Boss room manager script not found in the parent object.");
        }
    }

    void Update()
    {
        if (isInTrigger && Input.GetKeyDown(KeyCode.E)) {
            if (!bossSummoned){
                bossSummoned = true; // Sets the flag to true
                SummonCircle.GetComponent<Animator>().enabled = true;
                spawnText.SetActive(false);
                door.GetComponent<DoorController>().ToggleDoor();
                StartCoroutine(SummonCircleAnimation()); // Starts the summon circle animation
                GetComponent<Collider>().enabled = false;
            } else if (bossKilled){
                Debug.Log("Next level");
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (layerMask == (layerMask | (1 << other.gameObject.layer))){
            
            if(!bossSummoned)
                spawnText.SetActive(true); // Activates the spawn text
            if (bossKilled)
                nextLevelText.SetActive(true); // Activates the next level text
            
            isInTrigger = true; // Sets the flag to true
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (layerMask == (layerMask | (1 << other.gameObject.layer))){
            spawnText.SetActive(false); // Deactivates the spawn text
            isInTrigger = false; // Sets the flag to false
        }
    }

    private IEnumerator SummonCircleAnimation(){

        Debug.Log("Summoning boss"); // Debug message for summoning the boss
        SummonCircle.GetComponent<Animator>().SetInteger("Boss", bossRoomManagerScript.GetBossSummoningCircleAnimation());
        yield return null; // Wait for the next frame
        
        // Wait for the animation to finish
        AnimatorStateInfo info =  SummonCircle.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
        Debug.Log("Animation length: " + info.length);
        Debug.Log("Animation name: " + info.IsName("Lords Circle"));
        yield return new WaitForSeconds(info.length);
        bossRoomManagerScript.SpawnBoss(); // Spawns the boss
    }

   
}
