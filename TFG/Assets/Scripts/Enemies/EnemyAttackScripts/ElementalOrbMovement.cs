using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class ElementalOrbMovement : MonoBehaviour
{

    private NavMeshAgent agent; // NavMeshAgent component for movement
    private Transform target; // Target object to move towards

    private float pathUpdateDelay = 0.2f; // Delay for path updates

    private float pathUpdateDeadline = 0f; // Timer for path updates

    private float launchForwardCooldown = 3.0f; // Cooldown for launching forward
    private float launchForwardDeadline = 0f; // Timer for launching forward

    private float launchForwardDuration = 3.0f; // Duration of the launch forward
    private bool canLaunchForward = true; // Flag to check if the cooldown can start

    private float velocityAdded = 10.0f; // Force applied when launching forward

    private GameObject magician;

    private GameObject widow;

    private GameObject lord;

    [SerializeField] LayerMask playerLayer; // Layer mask for the player
    [SerializeField] LayerMask attackLayer; // Layer mask for the attacks

    // Start is called before the first frame update
    void Start()
    {

        agent = GetComponent<NavMeshAgent>(); // Get the NavMeshAgent component
        target = GameObject.FindGameObjectWithTag("Player").transform; // Find the player object by tag
        if (target != null)
        {
            agent.SetDestination(target.transform.position); // Set the destination to the player's position
        }
        else
        {
            Debug.LogWarning("Player not found!"); // Log a warning if the player is not found
        }

        launchForwardDeadline = Time.time + launchForwardCooldown; // Set the launch forward deadline
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if(magician != null) CheckMagician(); // Check if the magician is still valid
        if(widow != null) CheckWidow(); // Check if the widow is still valid
        if(lord != null) CheckLord(); // Check if the lord is still valid

        UpdatePath(); // Update the path to the target
        if (canLaunchForward && Time.time >= launchForwardDeadline)
        {
            
            LaunchForward(); // Launch the object forward
        }

    }

    private void CheckMagician()
    {
        if (magician == null) Destroy(gameObject); // Destroy the object if the magician is null (Would happen if the magician is destroyed, but didn't delete the object)
        if (magician.activeSelf == false) Destroy(gameObject); // Destroy the object if the magician is not active (Would happen if the magician is destroyed, but didn't delete the object)
    }

    private void CheckWidow()
    {
        if (widow == null) Destroy(gameObject); // Destroy the object if the widow is null (Would happen if the widow is destroyed, but didn't delete the object)
        if (widow.activeSelf == false) Destroy(gameObject); // Destroy the object if the widow is not active (Would happen if the widow is destroyed, but didn't delete the object)
    }

    private void CheckLord()
    {
        if (lord == null) Destroy(gameObject); // Destroy the object if the lord is null (Would happen if the lord is destroyed, but didn't delete the object)
        if (lord.activeSelf == false) Destroy(gameObject); // Destroy the object if the lord is not active (Would happen if the lord is destroyed, but didn't delete the object)
    }

    void UpdatePath(){
        if (Time.time >= pathUpdateDeadline)
        {
            pathUpdateDeadline = Time.time + pathUpdateDelay;
            if(agent.enabled)    agent.SetDestination(target.position);
        }
    }

    void LaunchForward()
    {
        agent.speed = agent.speed + velocityAdded; // Increase the speed of the object
        StartCoroutine(IncrementVelocityDuration()); // Start the coroutine to decrease the speed after a duration
    }

    public void SetMagician(GameObject magician)
    {
        this.magician = magician; // Set the magician object
    }

    public void SetWidow(GameObject widow)
    {
        this.widow = widow; // Set the widow object
    }

    public void SetLord(GameObject lord)
    {
        this.lord = lord; // Set the lord object
    }      

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Elemental orb collided with " + other.gameObject.name); // Log the collision with the object
        if (playerLayer == (playerLayer | (1 << other.gameObject.layer)) || attackLayer == (attackLayer | (1 << other.gameObject.layer)))
        {
            Debug.Log("Elemental orb collided with " + other.gameObject.name); // Log the collision with the player or attacks
            // if the player collides with the object, the object will be destroyed but first it will be deleted from the MagicOrbs list from the magician
            if (magician != null)
            {
                magician.GetComponent<MagicianBehaviour>().RemoveOrb(gameObject); // Remove the object from the magician's list of orbs
            }
            else if (widow != null)
            {
                widow.GetComponent<WidowBehaviours>().RemoveOrb(gameObject); // Remove the object from the widow's list of orbs
            }
            else if (lord != null)
            {
                lord.GetComponent<LordBehaviours>().RemoveOrb(gameObject); // Remove the object from the lord's list of orbs
            }
            else
            {
                Debug.LogWarning("Magician not found!"); // Log a warning if the magician is not found
            }
        }
    }

    IEnumerator IncrementVelocityDuration(){

        yield return new WaitForSeconds(launchForwardDuration); // Wait for the duration of the launch forward
        agent.speed = agent.speed - velocityAdded; // Decrease the speed of the object
        launchForwardDeadline = Time.time + launchForwardCooldown; // Reset the launch forward deadline
    }
}
