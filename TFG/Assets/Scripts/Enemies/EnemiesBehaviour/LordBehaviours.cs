using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LordBehaviours : MonoBehaviour, IBossBehaviours
{

    private BossReferences bossReferences; // Reference to the BossReferences component

    private bool isTargetingPlayer = true; // Flag to check if the boss is targeting the player    
    private bool isMovingToRandomWaypoint = false; // Flag to check if the boss is moving to a random waypoint
    private int attackIndex = 0; // Index of the attack to be performed

    private List<GameObject> spawnedOrbs = new List<GameObject>(); // List of spawned orbs
    private List<GameObject> spawnedSlimes = new List<GameObject>(); // List of spawned slimes
    private float maxAttackDuration = 3.0f;

    [Header("Attack Settings")]
    [SerializeField]
    [Range(0.1f, 1f)]
    private float poisonSurgeGenerationRate = 0.2f; // Rate at which poison surges are generated

    [Header("Debug Settings")]
    [SerializeField]
    [Tooltip("Set to true to enable debug mode, false to disable it")]
    private bool DEBUG_MODE = false; // Debug mode flag

    private void Awake()
    {
        bossReferences = GetComponent<BossReferences>(); // Get the BossReferences component

        StartSetUp(); // Call the StartSetUp method to set up the boss behaviour

    }

    private void StartSetUp()
    {
        // The Lord of Poison has a different approach to targeting the player
        if (bossReferences.GetBossType() == EnumBosses.LordOfPoison)
        {
            isTargetingPlayer = false;
            isMovingToRandomWaypoint = false; // Set the flag to move to the furthest waypoint from the player
        }
        else
        {
            isTargetingPlayer = true; // Set the flag to target the player
            isMovingToRandomWaypoint = false; // Set the flag to move towards the player
        }
    }

    void Start()
    {
        MovingBehaviour(); // Call the MovingBehaviour method to select the new target
    }

    void LateUpdate()
    {
        // Fixes the Y position of the boss to 1.4f

        // It has to fix the position of its paren, since that object is the one that has the NavMeshAgent component
        transform.position = new Vector3(transform.position.x, 2.8f, transform.position.z); // Fix the Y position of the parent object

    }

    public void AttackBehaviour()
    {

        if (attackIndex >= bossReferences.attackPrefabs.Length)
        {
            Debug.LogError("Invalid attack index: " + attackIndex); // Log an error if the attack index is invalid
            return;
        }

        // Depending on the level the boss is generated, it can have up tu 5 attacks
        switch (attackIndex)
        {
            case 0:
                Debug.Log(bossReferences.GetBossType().ToString() + " Basic Attack"); // Log the attack index
                StopMoving(); // Stop the navMeshAgent
                bossReferences.animator.SetTrigger("Attack1"); // Trigger the basic attack animation
                bossReferences.isAttacking = true; // Set the isAttacking flag to true
                StartCoroutine(LordBasicAttackAnimation()); // Start the coroutine to wait for the animation to finish
                // Attack 1
                break;
            case 1:
                // Attack 2
                Debug.Log(bossReferences.GetBossType().ToString() + " Burst Attack"); // Log the attack index
                StopMoving(); // Stop the navMeshAgent
                bossReferences.animator.SetTrigger("Attack2_Burst"); // Trigger the basic attack animation
                bossReferences.isAttacking = true; // Set the isAttacking flag to true
                StartCoroutine(LordBurstAttackAnimation());
                break;
            case 2:
                // Attack 3
                Debug.Log(bossReferences.GetBossType().ToString() + " Spear Attack"); // Log the attack index
                StopMoving(); // Stop the navMeshAgent
                bossReferences.animator.SetTrigger("Attack3_Spear"); // Trigger the basic attack animation
                bossReferences.isAttacking = true; // Set the isAttacking flag to true
                StartCoroutine(LordSpearAttackAnimation()); // Start the coroutine to wait for the animation to finish
                break;
            case 3:
                // Attack 4
                Debug.Log(bossReferences.GetBossType().ToString() + " Spell Cast Attack"); // Log the attack index
                StopMoving(); // Stop the navMeshAgent
                bossReferences.animator.SetTrigger("Attack4"); // Trigger the basic attack animation
                bossReferences.isAttacking = true; // Set the isAttacking flag to true
                StartCoroutine(LordSpellCastAnimation()); // Start the coroutine to wait for the animation to finish
                break;
            case 4:
                // Attack 5
                Debug.Log(bossReferences.GetBossType().ToString() + " Summon Attack"); // Log the attack index
                StopMoving(); // Stop the navMeshAgent
                bossReferences.animator.SetTrigger("Attack5_Summon"); // Trigger the basic attack animation
                bossReferences.isAttacking = true; // Set the isAttacking flag to true

                StartCoroutine(LordSummonAnimation()); // Start the coroutine to wait for the animation to finish
                break;
            default:
                Debug.LogError("Invalid attack index: " + attackIndex); // Log an error if the attack index is invalid
                break;
        }
    }

    public void DeathBehaviour()
    {
        clearOrbs(); // Clear the orbs
        ClearSlimes(); // Clear the slimes
    }

    public void RemoveOrb(GameObject orb)
    {
        if (orb != null)
        {
            spawnedOrbs.Remove(orb); // Remove the orb from the list
            Destroy(orb); // Destroy the orb
        }

    }

    public void RemoveSlime(GameObject slime)
    {
        if (slime != null)
        {
            spawnedSlimes.Remove(slime); // Remove the slime from the list
            Destroy(slime); // Destroy the slime
        }
    }

    void clearOrbs()
    {
        foreach (GameObject orb in spawnedOrbs)
        {
            if (orb != null)
            {
                Destroy(orb); // Destroy the orb
            }
        }
        spawnedOrbs.Clear(); // Clear the list of spawned orbs
    }

    void ClearSlimes()
    {
        foreach (GameObject slime in spawnedSlimes)
        {
            if (slime != null)
            {
                Destroy(slime); // Destroy the slime
            }
        }
        spawnedSlimes.Clear(); // Clear the list of spawned slimes
    }

    private void StopMoving()
    {
        // Stop the navMeshAgent
        bossReferences.navMeshAgent.isStopped = true; // Stop the navMeshAgent
        bossReferences.navMeshAgent.ResetPath(); // Reset the path of the navMeshAgent
    }

    public void MovingBehaviour()
    {

        Debug.Log("MovingBehaviour called From " + bossReferences.GetBossType().ToString() + " TargetsPlayer: " + isTargetingPlayer + " RandomWaypoint: " + isMovingToRandomWaypoint); // Log the MovingBehaviour call

        if (isTargetingPlayer)
        {
            // Move towards the player
            bossReferences.target = bossReferences.player; // Set the target to the player
            Debug.LogWarning("New target (Player):" + bossReferences.target.position); // Log the new target position
        }
        else if (isMovingToRandomWaypoint)
        {

            // Move towards a random waypoint
            List<Transform> waypoints = DiscardNearWaypoints(bossReferences.GetWaypoints()); // Get the waypoints and discard the ones too close to the player
            int randomIndex = Random.Range(0, waypoints.Count);
            bossReferences.target = waypoints[randomIndex]; // Set the target to a random waypoint
            //bossReferences.navMeshAgent.SetDestination(bossReferences.target.position); // Set the destination to the random waypoint
            Debug.LogWarning("New target (Random Waypoint):" + bossReferences.target.position); // Log the new target position
        }
        else
        {
            // Move towards the next waypoint
            List<Transform> waypoints = DiscardNearWaypoints(bossReferences.GetWaypoints()); // Get the waypoints and discard the ones too close to the player

            // Calculates the furthest waypoint from the player
            Transform furthestWaypoint = waypoints[0];

            for (int i = 1; i < waypoints.Count; i++)
            {
                if (Vector3.Distance(bossReferences.player.position, waypoints[i].position) > Vector3.Distance(bossReferences.player.position, furthestWaypoint.position))
                {
                    furthestWaypoint = waypoints[i];
                }
            }

            bossReferences.target = furthestWaypoint; // Set the target to the furthest waypoint
            Debug.LogWarning("New target (Furthest Waypoint from Player):" + bossReferences.target.position); // Log the new target position

        }

    }

    private List<Transform> DiscardNearWaypoints(List<Transform> waypoints)
    {
        // Discard waypoints that are too close to the player
        List<Transform> farWaypoints = new List<Transform>();
        foreach (Transform waypoint in waypoints)
        {
            if (Vector3.Distance(bossReferences.player.position, waypoint.position) > 8f) // Adjust the distance threshold as needed
            {
                farWaypoints.Add(waypoint);
            }
        }
        return farWaypoints;
    }

    public bool IsBossReadyToAttack()
    {

        float distanceToTarget = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.y), new Vector3(bossReferences.target.transform.position.x, 0, bossReferences.target.transform.position.y)); // Calculate the distance to the target
        // Check if the boss is ready to attack
        // Debug.Log("Is boss ready to attack? " + (distanceToTarget <= bossReferences.navMeshAgent.stoppingDistance));
        // Debug.Log("Remaining distance: " + distanceToTarget);
        // Debug.Log("Stopping distance: " + bossReferences.navMeshAgent.stoppingDistance);
        // Debug.Log("Target position: " + bossReferences.target.position);
        // Debug.Log("Boss position: " + gameObject.transform.position);
        return distanceToTarget <= bossReferences.navMeshAgent.stoppingDistance + 1.5f;
    }

    public void UpdateAttackIndex()
    {
        // Update the attack index for the next attack
        Debug.Log("Attack index (Before): " + attackIndex); // Log the attack index
        attackIndex = (attackIndex + 1) % bossReferences.GetAttacksPerRotation(); // Loop through the attack index
        Debug.Log("Attack index (After): " + attackIndex); // Log the attack index
        //Debug.LogError("-----------------------------------------");
    }


    //////////////////////// Coroutines to attack the player ////////////////////////


    private IEnumerator LordBasicAttackAnimation()
    {
        yield return null;
        yield return new WaitForSeconds(maxAttackDuration); // Wait for the attack duration
        bossReferences.isAttacking = false; // Set the isAttacking flag to false

        if (bossReferences.GetBossType() == EnumBosses.LordOfPoison)
        {
            isTargetingPlayer = false;
            isMovingToRandomWaypoint = true; // Set the flag to move to a random waypoint
        }
        bossReferences.SetCurrentState(EnumBossesStates.Moving); // Set the state to Moving
        UpdateAttackIndex(); // Update the attack index for the next attack
        MovingBehaviour(); // Call the MovingBehaviour method to select the new target
    }

    public void LaunchSlashAttack()
    {

        if (DEBUG_MODE)
        {
            if (bossReferences.GetBossType() == EnumBosses.LordOfPoison)
            {
                Debug.Log("Launching Slash Attack (Lord of Poison)"); // Log the attack
                if (bossReferences.target.gameObject.name == "Player")
                    Debug.LogError("Target is the player, this should not happen!"); // Log an error if the target is the player
            }
            else
            {
                Debug.Log("Launching Slash Attack (Lord of Fire)"); // Log the attack
                if (bossReferences.target.gameObject.name != "Player")
                    Debug.LogError("Target is not the player, this should not happen!"); // Log an error if the target is not the player
            }
        }

        GameObject attackPrefab = Instantiate(bossReferences.attackPrefabs[attackIndex], new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z), Quaternion.identity); // Instantiate the attack prefab
        RotateToPlayer(attackPrefab.transform); // Rotate the attack prefab to face the player

    }

    private void RotateToPlayer(Transform attackPrefabTransform)
    {
        Vector3 dir = bossReferences.player.position - attackPrefabTransform.position;
        dir.y = 0f;                                 // rotate only on the y axis
        if (dir.sqrMagnitude < 0.001f) return;      // if the distance is too small, do nothing

        attackPrefabTransform.rotation = Quaternion.LookRotation(dir);
    }


    private IEnumerator LordBurstAttackAnimation()
    {
        yield return null;
        yield return new WaitForSeconds(maxAttackDuration); // Wait for the attack duration
        bossReferences.isAttacking = false; // Set the isAttacking flag to false

        isTargetingPlayer = true;
        isMovingToRandomWaypoint = false; // Set the flag to move towards the player
        bossReferences.SetCurrentState(EnumBossesStates.Moving); // Set the state to Moving
        UpdateAttackIndex(); // Update the attack index for the next attack
        MovingBehaviour(); // Call the MovingBehaviour method to select the new target
    }

    public void LordOfFireBurstAttack()
    {
        if (DEBUG_MODE)
        {
            Debug.Log("Launching Burst Attack (Lord of Fire)"); // Log the attack
            if (bossReferences.target.gameObject.name != "Player")
                Debug.LogError("Target is not the player, this should not happen!"); // Log an error if the target is not the player
        }

        GameObject attackPrefab = Instantiate(bossReferences.attackPrefabs[attackIndex], new Vector3(gameObject.transform.position.x, 0.1f, gameObject.transform.position.z), Quaternion.identity); // Instantiate the attack prefab

        float randomDuration = Random.Range(4f, 6f); // Random duration for the attack
        Destroy(attackPrefab, randomDuration); // Destroy the attack prefab after 2 seconds
    }

    public void LordOfPoisonBurstAttack()
    {
        if (DEBUG_MODE)
        {
            Debug.Log("Launching Burst Attack (Lord of Poison)"); // Log the attack
            if (bossReferences.target.gameObject.name == "Player")
                Debug.LogError("Target is the player, this should not happen!"); // Log an error if the target is the player
        }

        // Checks for all waypoints and instantiates a poison surge at each one (if the random chance is met)
        List<Transform> waypoints = DiscardNearWaypoints(bossReferences.GetWaypoints()); // Get the waypoints and discard the ones too close to the player
        foreach (Transform waypoint in waypoints)
        {
            if (Random.value < poisonSurgeGenerationRate) // Check if the random chance is met
            {
                Instantiate(bossReferences.attackPrefabs[attackIndex], new Vector3(waypoint.position.x, 0.2f, waypoint.position.z), Quaternion.identity); // Instantiate the attack prefab
            }
        }

    }

    private IEnumerator LordSpearAttackAnimation()
    {
        yield return null;
        yield return new WaitForSeconds(maxAttackDuration); // Wait for the attack duration
        bossReferences.isAttacking = false; // Set the isAttacking flag to false

        if (bossReferences.GetAttacksPerRotation() > 3)
        {

            isTargetingPlayer = false;
            if (bossReferences.GetBossType() == EnumBosses.LordOfPoison)
            {
                isMovingToRandomWaypoint = false; // Set the flag to move to a random waypoint
            }
            else
            {
                isMovingToRandomWaypoint = true; // Set the flag to move towards the player
            }

            bossReferences.SetCurrentState(EnumBossesStates.Moving); // Set the state to Moving
        }
        else
        {
            StartSetUp(); // Call the StartSetUp method to set up the boss behaviour
            bossReferences.SetCurrentState(EnumBossesStates.Idle); // Set the state to Idle
        }
        UpdateAttackIndex(); // Update the attack index for the next attack
        MovingBehaviour(); // Call the MovingBehaviour method to select the new target
    }

    public void LordsSpearAttack()
    {
        if (DEBUG_MODE)
        {
            if (bossReferences.GetBossType() == EnumBosses.LordOfPoison)
            {
                Debug.Log("Launching Spear Attack (Lord of Poison)"); // Log the attack
                if (bossReferences.target.gameObject.name != "Player")
                    Debug.LogError("Target is not player, this should not happen!"); // Log an error if the target is the player
            }
            else
            {
                Debug.Log("Launching Spear Attack (Lord of Fire)"); // Log the attack
                if (bossReferences.target.gameObject.name != "Player")
                    Debug.LogError("Target is not the player, this should not happen!"); // Log an error if the target is not the player
            }
        }

        GameObject attackPrefab = Instantiate(bossReferences.attackPrefabs[attackIndex], new Vector3(gameObject.transform.position.x, 0, gameObject.transform.position.z), Quaternion.identity); // Instantiate the attack prefab
        RotateToPlayer(attackPrefab.transform); // Rotate the attack prefab to face the player
        Destroy(attackPrefab, 0.4f); // Destroy the attack prefab after the attack duration

    }

    private IEnumerator LordSpellCastAnimation()
    {
        yield return null;
        yield return new WaitForSeconds(maxAttackDuration); // Wait for the attack duration
        bossReferences.isAttacking = false; // Set the isAttacking flag to false

        if (bossReferences.GetAttacksPerRotation() > 4)
        {

            isTargetingPlayer = false;
            isMovingToRandomWaypoint = true; // Set the flag to move to a random waypoint

            bossReferences.SetCurrentState(EnumBossesStates.Moving); // Set the state to Moving
        }
        else
        {
            StartSetUp(); // Call the StartSetUp method to set up the boss behaviour
            bossReferences.SetCurrentState(EnumBossesStates.Idle); // Set the state to Idle
        }
        UpdateAttackIndex(); // Update the attack index for the next attack
        MovingBehaviour(); // Call the MovingBehaviour method to select the new target
    }

    public void LordsSpellCastAttack()
    {
        if (DEBUG_MODE)
        {
            if (bossReferences.GetBossType() == EnumBosses.LordOfPoison)
            {
                Debug.Log("Launching Spell Cast Attack (Lord of Poison)"); // Log the attack
                if (bossReferences.target.gameObject.name == "Player")
                    Debug.LogError("Target is player, this should not happen!"); // Log an error if the target is not the player
            }
            else
            {
                Debug.Log("Launching Spell Cast Attack (Lord of Fire)"); // Log the attack
                if (bossReferences.target.gameObject.name == "Player")
                    Debug.LogError("Target is the player, this should not happen!"); // Log an error if the target is not the player
            }
        }

        // Sumons a random amount of orbs on the boss position, between 1 and 3
        int randomAmount = Random.Range(1, 4); // Random amount of orbs to summon

        for (int i = 0; i < randomAmount; i++)
        {
            GameObject orb = Instantiate(bossReferences.attackPrefabs[attackIndex], new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z), Quaternion.identity); // Instantiate the attack prefab
            spawnedOrbs.Add(orb); // Add the orb to the list of spawned orbs
            orb.GetComponent<ElementalOrbMovement>().SetLord(gameObject); // Set the lord object in the orb script
        }

    }

    private IEnumerator LordSummonAnimation()
    {
        yield return null;
        yield return new WaitForSeconds(maxAttackDuration); // Wait for the attack duration
        bossReferences.isAttacking = false; // Set the isAttacking flag to false

        StartSetUp();

        bossReferences.SetCurrentState(EnumBossesStates.Idle); // Set the state to Moving
        UpdateAttackIndex(); // Update the attack index for the next attack
        MovingBehaviour(); // Call the MovingBehaviour method to select the new target
    }
    
    public void LordsSummonAttack()
    {
        if (DEBUG_MODE)
        {
            if (bossReferences.GetBossType() == EnumBosses.LordOfPoison)
            {
                Debug.Log("Launching Spell Cast Attack (Lord of Poison)"); // Log the attack
                if (bossReferences.target.gameObject.name == "Player")
                    Debug.LogError("Target is player, this should not happen!"); // Log an error if the target is not the player
            }
            else
            {
                Debug.Log("Launching Spell Cast Attack (Lord of Fire)"); // Log the attack
                if (bossReferences.target.gameObject.name == "Player")
                    Debug.LogError("Target is the player, this should not happen!"); // Log an error if the target is not the player
            }
        }

        // Sumons a random amount of slimes on the boss position, between 1 and 2 
        int randomAmount = Random.Range(1, 3); // Random amount of orbs to summon
        float xOffset = -3f;
        for (int i = 0; i < randomAmount; i++)
        {
            GameObject slime = Instantiate(bossReferences.attackPrefabs[attackIndex], new Vector3(gameObject.transform.position.x+xOffset, 1.2f, gameObject.transform.position.z), Quaternion.identity); // Instantiate the attack prefab
            spawnedSlimes.Add(slime); // Add the orb to the list of spawned orbs
            slime.GetComponent<SlimeBehaviour>().SetLord(gameObject); // Set the lord object in the slime script
            xOffset += 6f; // Increment the x offset for the next slime
        }
    }



}
