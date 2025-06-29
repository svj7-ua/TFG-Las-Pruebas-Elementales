using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AncientGolemBehaviours : MonoBehaviour, IBossBehaviours
{

    private BossReferences bossReferences; // Reference to the BossReferences component

    private bool isTargetingPlayer = false; // Flag to check if the boss is targeting the player    
    private bool isMovingToRandomWaypoint = true; // Flag to check if the boss is moving to a random waypoint
    private int attackIndex = 0; // Index of the attack to be performed

    private List<GameObject> spawnedOrbs = new List<GameObject>(); // List of spawned orbs

    private bool spinAttackSpawned = false; // Flag to check if the spin attack has been spawned

    private GameObject spinAttackGameObject; // Reference to the spin attack prefab

    //private float maxAttackDuration = 3.0f;

    private void Awake()
    {
        bossReferences = GetComponent<BossReferences>(); // Get the BossReferences component

        // TODO: Will modify the maxAttackDuration depending on the level of the boss

    }

    void Start()
    {
        MovingBehaviour(); // Call the MovingBehaviour method to select the new target
    }

    void LateUpdate()
    {
        // Fixes the Y position of the boss to 2
        Vector3 position = gameObject.transform.position; // Get the position of the boss
        position.y = 2f; // Set the Y position to 2
        gameObject.transform.position = position; // Set the position of the boss
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
                Debug.Log("Lightning Surge"); // Log the attack index
                StopMoving(); // Stop the navMeshAgent
                bossReferences.animator.SetTrigger("Attack1_Area"); // Trigger the basic attack animation
                bossReferences.isAttacking = true; // Set the isAttacking flag to true
                StartCoroutine(AncientGolemLightningSurgeAnimation()); // Start the coroutine to wait for the animation to finish
                // Attack 1
                break;
            case 1:
                // Attack 2
                Debug.Log("Spin Attack"); // Log the attack index
                StopMoving(); // Stop the navMeshAgent
                bossReferences.animator.SetTrigger("Attack2_Spin"); // Trigger the basic attack animation
                bossReferences.isAttacking = true; // Set the isAttacking flag to true
                StartCoroutine(AncientGolemSpinAttackAnimation());
                break;
            case 2:
                // Attack 3
                Debug.Log("Smash Attack"); // Log the attack index
                StopMoving(); // Stop the navMeshAgent
                bossReferences.animator.SetTrigger("Attack3_Smash"); // Trigger the basic attack animation
                bossReferences.isAttacking = true; // Set the isAttacking flag to true
                StartCoroutine(AncientGolemSmashAttackAnimation()); // Start the coroutine to wait for the animation to finish
                break;
            case 3:
                // Attack 4
                Debug.Log("Orbital Strike"); // Log the attack index
                StopMoving(); // Stop the navMeshAgent
                bossReferences.animator.SetTrigger("Attack4_Ranged"); // Trigger the basic attack animation
                bossReferences.isAttacking = true; // Set the isAttacking flag to true
                StartCoroutine(AncientGolemOrbitalStrikeAnimation()); // Start the coroutine to wait for the animation to finish
                break;
            case 4:
                // Attack 5
                Debug.Log("Burst Attack"); // Log the attack index
                StopMoving(); // Stop the navMeshAgent
                bossReferences.animator.SetTrigger("Attack5_Burst"); // Trigger the basic attack animation
                bossReferences.isAttacking = true; // Set the isAttacking flag to true
                StartCoroutine(AncientGolemBurstAttackAnimation()); // Start the coroutine to wait for the animation to finish
                break;
            default:
                Debug.LogError("Invalid attack index: " + attackIndex); // Log an error if the attack index is invalid
                break;
        }
    }

    public void DeathBehaviour()
    {
        clearOrbs(); // Clear the orbs
    }

    public void RemoveOrb(GameObject orb)
    {
        if (orb != null)
        {
            spawnedOrbs.Remove(orb); // Remove the orb from the list
            Destroy(orb); // Destroy the orb
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

    private void StopMoving()
    {
        // Stop the navMeshAgent
        bossReferences.navMeshAgent.isStopped = true; // Stop the navMeshAgent
        bossReferences.navMeshAgent.ResetPath(); // Reset the path of the navMeshAgent
    }

    public void MovingBehaviour()
    {

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

        float distanceToTarget = Vector3.Distance(new Vector3(gameObject.transform.position.x, 0, gameObject.transform.position.y), new Vector3(bossReferences.target.transform.position.x, 0, bossReferences.target.transform.position.y)); // Calculate the distance to the target
        // Check if the boss is ready to attack
        // Debug.Log("Is boss ready to attack? " + (distanceToTarget <= bossReferences.navMeshAgent.stoppingDistance));
        // Debug.Log("Remaining distance: " + distanceToTarget);
        // Debug.Log("Stopping distance: " + bossReferences.navMeshAgent.stoppingDistance);
        // Debug.Log("Target position: " + bossReferences.target.position);
        // Debug.Log("Boss position: " + gameObject.transform.position);
        return distanceToTarget <= bossReferences.navMeshAgent.stoppingDistance + 1f;
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

    public void AncientGolemLightningSurge()
    {
        // Will instantiate the basic attack prefab
        Debug.Log("Ancient Golem Lightning Surge");
        if (bossReferences.target.gameObject.name == "Player")
        {
            Debug.LogError("Target is the player! Boss Should have moved to a Waypoint"); // Log an error if the target is not the player
            return;
        }
        Debug.Log("Attack index: " + attackIndex); // Log the attack index
        // Instantiates the basic attack prefab on the boss position (but substracts 2f to the y position, since the boss is too tall)
        GameObject attackPrefab = Instantiate(bossReferences.attackPrefabs[attackIndex], new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 1f, gameObject.transform.position.z), Quaternion.identity); // Instantiate the attack prefab
                                                                                                                                                                                                                                    //Adds the attack prefab as a child of the boss
        attackPrefab.transform.parent = gameObject.transform; // Set the parent of the attack prefab to the boss
        attackPrefab.transform.localPosition = new Vector3(0, 1, 0); // Set the local position of the attack prefab to the boss position

        // Sets a random time for the attack prefab to be destroyed between 3 and 6 seconds
        float randomDelay = Random.Range(3f, 6f);

        Destroy(attackPrefab, randomDelay); // Destroy the attack prefab after it has been used

    }

    private IEnumerator AncientGolemLightningSurgeAnimation()
    {
        Debug.Log("Ancient Golem Lightning Surge Animation");
        yield return null;
        // Wait for the animation to finish
        Debug.Log("Animation length: " + bossReferences.animator.GetCurrentAnimatorStateInfo(0).length); // Log the animation length
        yield return new WaitForSeconds(bossReferences.animator.GetCurrentAnimatorStateInfo(0).length);
        // Set the boss state to idle after the animation
        UpdateAttackIndex(); // Update the attack index for the next attack
        bossReferences.navMeshAgent.isStopped = true; // Stop the navMeshAgent
        bossReferences.navMeshAgent.ResetPath(); // Reset the path of the navMeshAgent

        isMovingToRandomWaypoint = false; // Stop moving to a random waypoint
        isTargetingPlayer = true; // Start targeting the player again
        MovingBehaviour(); // Call the MovingBehaviour method to select the new target

        bossReferences.SetCurrentState(EnumBossesStates.PursueAttack);
        bossReferences.isAttacking = false; // Set the isAttacking flag to false
    }

    public void AncientGolemSpinAttack()
    {

        if (!bossReferences.animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2"))
        {
            return;
        }

        // Will instantiate the jump attack prefab
        Debug.Log("Spin Attack");
        if (bossReferences.target.gameObject.name != "Player")
        {
            Debug.LogError("Target is not the player!"); // Log an error if the target is not the player
            return;
        }

        if (spinAttackSpawned)
        {
            Debug.Log("Spin attack already spawned!"); // Log if the spin attack has already been spawned
            return;
        }


        // Instantiates the jump attack prefab on the boss position but at y=0
        if (attackIndex != 1)
        {
            Debug.LogError("Attack index is not 1!"); // Log an error if the attack index is not 1
            return;
        }
        spinAttackSpawned = true; // Set the spin attack spawned flag to true
        spinAttackGameObject = Instantiate(bossReferences.attackPrefabs[attackIndex], new Vector3(gameObject.transform.position.x, 0, gameObject.transform.position.z), Quaternion.identity); // Instantiate the attack prefab

        // Sets the spin attack prefab as a child of the boss
        spinAttackGameObject.transform.parent = gameObject.transform; // Set the parent of the attack prefab to the boss
        spinAttackGameObject.transform.localPosition = new Vector3(0, 1, 0); // Set the local position of the attack prefab to the boss position

        // Sets a random time for the attack prefab to be destroyed between 3 and 6 seconds
        float randomDelay = Random.Range(3f, 6f);
        StartCoroutine(AncientGolemSpinAttackEndOfAnimation(randomDelay+0.1f)); // Start the coroutine to wait for the animation to finish (Adds an offset to avoid errors with the next attack)

    }

    private IEnumerator AncientGolemSpinAttackAnimation()
    {
        // Wait for the animation to finish, at least the time it takes to perform the attack
        yield return null;
        yield return new WaitForSeconds(bossReferences.animator.GetCurrentAnimatorStateInfo(0).length);
        // Then after the first loop of the animation, it starts moving pursuing the player

        isTargetingPlayer = true; // Start targeting the player again
        isMovingToRandomWaypoint = false; // Stop moving to a random waypoint

        Debug.Log("Calling MovingBehaviour From Spin Attack Animation");
        MovingBehaviour(); // Call the MovingBehaviour method to select the new target
        bossReferences.SetCurrentState(EnumBossesStates.PursueAttack); // Set the boss state to idle after the animation


    }

    private IEnumerator AncientGolemSpinAttackEndOfAnimation(float duration)
    {

        // Wait for the animation to finish
        yield return new WaitForSeconds(duration);
        // Set the boss state to idle after the animation

        if (spinAttackGameObject != null)
        {
            Destroy(spinAttackGameObject); // Destroy the spin attack prefab
            spinAttackSpawned = false; // Set the spin attack spawned flag to false
        }
        else
        {
            Debug.LogError("Spin attack prefab not found!"); // Log an error if the spin attack prefab is not found
        }

        UpdateAttackIndex(); // Update the attack index for the next attack
        bossReferences.animator.SetTrigger("Attack2_EndSpin");
        bossReferences.SetCurrentState(EnumBossesStates.Moving); // Set the boss state to idle after the animation
        isMovingToRandomWaypoint = false; // Stop moving to a random waypoint
        isTargetingPlayer = true; // Start targeting the player again

        Debug.Log("Calling MovingBehaviour From Spin Attack End Animation");
        MovingBehaviour(); // Call the MovingBehaviour method to select the new target
        bossReferences.isAttacking = false; // Set the isAttacking flag to false

    }


    public void AncientGolemSmashAttack()
    {
        // Will instantiate the area attack prefab
        Debug.LogWarning("Ancient Golem Smash Attack");
        if (bossReferences.target.gameObject.name != "Player")
        {
            Debug.LogError("Target is not the player!"); // Log an error if the target is not the player
            return;
        }

        // Instantiates the area attack prefab on the boss position but at y=1
        GameObject attackPrefab = Instantiate(bossReferences.attackPrefabs[attackIndex], new Vector3(gameObject.transform.position.x, 0.5f, gameObject.transform.position.z), Quaternion.identity); // Instantiate the attack prefab

        Debug.Log("UpdateAttackIndex From Smash: " + attackIndex); // Log the attack index
        UpdateAttackIndex(); // Update the attack index for the next attack


        isMovingToRandomWaypoint = false; // Stop moving to a random waypoint
        isTargetingPlayer = false; // Start targeting the player again
        


        Destroy(attackPrefab, 0.3f); // Destroy the attack prefab after .3 seconds
                                                                                                                                                                                                                                  

    }
    private IEnumerator AncientGolemSmashAttackAnimation()
    {
        yield return null;
        // Wait for the area attack animation to finish
        bossReferences.animator.ResetTrigger("Attack3_Smash");
        yield return new WaitForSeconds(bossReferences.animator.GetCurrentAnimatorStateInfo(0).length);
        Debug.Log("Calling MovingBehaviour From Smash Attack Animation");
        MovingBehaviour(); // Call the MovingBehaviour method to select the new target  
        if (bossReferences.GetAttacksPerRotation() <= 3)
        {
            bossReferences.SetCurrentState(EnumBossesStates.Idle); // Set the boss state to idle after the animation
        }
        else
        {
            bossReferences.SetCurrentState(EnumBossesStates.Moving); // Set the boss state to idle after the animation
        }

        bossReferences.isAttacking = false; // Set the isAttacking flag to false
    }

    public void AncientGolemOrbitalStrike()
    {
        // Will instantiate the area attack prefab
        Debug.Log("Ancient Golem Orbital Strike");
        if (bossReferences.target.gameObject.name == "Player")
        {
            Debug.LogError("Target is not a Waypoint!"); // Log an error if the target is not the player
            return;
        }

        // Instantiates the orbital strike prefab on the player position but at y=0
        GameObject attackPrefab = Instantiate(bossReferences.attackPrefabs[attackIndex], new Vector3(bossReferences.player.position.x, 0, bossReferences.player.position.z), Quaternion.identity); // Instantiate the attack prefab
        Destroy(attackPrefab, 4.85f); // Destroy the attack prefab after its duration, which is 4.85 seconds
    }

    private IEnumerator AncientGolemOrbitalStrikeAnimation()
    {
        yield return null;
        // Wait for the area attack animation to finish
        yield return new WaitForSeconds(bossReferences.animator.GetCurrentAnimatorStateInfo(0).length);
        UpdateAttackIndex(); // Update the attack index for the next attack
        if(bossReferences.GetAttacksPerRotation() <= 4)
        {
            bossReferences.SetCurrentState(EnumBossesStates.Idle); // Set the boss state to idle after the animation
        }
        else
        {
            bossReferences.SetCurrentState(EnumBossesStates.Moving); // Set the boss state to idle after the animation
        }
        isMovingToRandomWaypoint = true; // Stop moving to a random waypoint
        isTargetingPlayer = false; // Start targeting the player again

        Debug.Log("Calling MovingBehaviour From Orbital Strike Animation");
        MovingBehaviour(); // Call the MovingBehaviour method to select the new target
        bossReferences.isAttacking = false; // Set the isAttacking flag to false
    }

    public void AncientGolemBurstAttack()
    {
        // Will instantiate the area attack prefab
        Debug.Log("Ancient Golem Burst Attack");
        if (bossReferences.target.gameObject.name == "Player")
        {
            Debug.LogError("Target is not a Waypoint!"); // Log an error if the target is not the player
            return;
        }

        // Instantiates the area attack prefab on the boss position but at y=1
        GameObject attackPrefab = Instantiate(bossReferences.attackPrefabs[attackIndex], new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z), Quaternion.identity); // Instantiate the attack prefab
        RotateToPlayer(attackPrefab.transform); // Rotate the attack prefab to face the player
        //Debug.LogError("Attack prefab position: " + attackPrefab.transform.position); // Log the attack prefab position                                                                                                                                                                                                                      

    }

    private void RotateToPlayer(Transform attackPrefabTransform)
    {
        Vector3 dir = bossReferences.player.position - attackPrefabTransform.position;
        dir.y = 0f;                                 // rotate only on the y axis
        if (dir.sqrMagnitude < 0.001f) return;      // if the distance is too small, do nothing

        attackPrefabTransform.rotation = Quaternion.LookRotation(dir);
    }

    private IEnumerator AncientGolemBurstAttackAnimation()
    {
        yield return null;
        // Wait for the area attack animation to finish
        yield return new WaitForSeconds(bossReferences.animator.GetCurrentAnimatorStateInfo(0).length);
        UpdateAttackIndex(); // Update the attack index for the next attack
        bossReferences.SetCurrentState(EnumBossesStates.Idle); // Set the boss state to idle after the animation
        isMovingToRandomWaypoint = true; // Stop moving to a random waypoint
        isTargetingPlayer = false; // Start targeting the player again

        Debug.Log("Calling MovingBehaviour From Burst Attack Animation");
        MovingBehaviour(); // Call the MovingBehaviour method to select the new target
        bossReferences.isAttacking = false; // Set the isAttacking flag to false
    }


}
