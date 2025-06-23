using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WidowBehaviours : MonoBehaviour, IBossBehaviours
{

    private BossReferences bossReferences; // Reference to the BossReferences component

    private bool isTargetingPlayer = true; // Flag to check if the boss is targeting the player    
    private bool isMovingToRandomWaypoint = false; // Flag to check if the boss is moving to a random waypoint
    private int attackIndex = 0; // Index of the attack to be performed

    private List<GameObject> spawnedOrbs = new List<GameObject>(); // List of spawned orbs

    private List<GameObject> spawnedSlimes = new List<GameObject>(); // List of spawned slimes

    [SerializeField]
    private GameObject[] slimesPrefabs; // Array of slime prefabs to be spawned

    private void Awake()
    {
        bossReferences = GetComponent<BossReferences>(); // Get the BossReferences component

    }

    private void Start()
    {
        if(slimesPrefabs.Length == 0)
        {
            Debug.LogError("No slimes prefabs assigned!"); // Log an error if no slimes prefabs are assigned
        }

    }

    public void AttackBehaviour()
    {

        if (attackIndex >= bossReferences.attackPrefabs.Length)
        {
            Debug.LogError("Invalid attack index: " + attackIndex); // Log an error if the attack index is invalid
            return;
        }

        if (bossReferences.GetCurrentState() == EnumBossesStates.AttackFollowUp)
        {
            Debug.LogWarning("Attack Index: " + attackIndex); // Log a warning if the attack index is in follow up state
        }

        // Depending on the level the boss is generated, it can have up tu 5 attacks
        switch (attackIndex)
        {
            case 0:
                Debug.Log("Target: " + bossReferences.target.gameObject.name); // Log the target name
                StopMoving(); // Stop the navMeshAgent
                bossReferences.animator.SetTrigger("Attack"); // Trigger the basic attack animation
                bossReferences.isAttacking = true; // Set the isAttacking flag to true
                StartCoroutine(WidowBasicAttackAnimationDuration()); // Start the coroutine to wait for the animation to finish
                // Attack 1
                break;
            case 1:
                // Attack 2
                StopMoving(); // Stop the navMeshAgent
                bossReferences.animator.SetTrigger("AttackJump"); // Trigger the basic attack animation
                bossReferences.isAttacking = true; // Set the isAttacking flag to true
                StartCoroutine(JumpAttackDeley());
                break;
            case 2:
                // Attack 3
                StopMoving(); // Stop the navMeshAgent
                bossReferences.animator.SetTrigger("AreaAttack"); // Trigger the basic attack animation
                bossReferences.isAttacking = true; // Set the isAttacking flag to true
                StartCoroutine(AreaAttackDelay());
                break;
            case 3:
                // Attack 4
                StopMoving(); // Stop the navMeshAgent
                bossReferences.animator.SetTrigger("Attack4_Spit"); // Trigger the basic attack animation
                bossReferences.isAttacking = true; // Set the isAttacking flag to true
                StartCoroutine(SpitAttackAnimation()); // Start the coroutine to wait for the animation to finish
                break;
            case 4:
                StopMoving(); // Stop the navMeshAgent
                bossReferences.animator.SetTrigger("AttackJump"); // Trigger the basic attack animation
                bossReferences.isAttacking = true; // Set the isAttacking flag to true
                StartCoroutine(SecondJumpAttackAnimation()); // Start the coroutine to wait for the animation to finish
                // Attack 5
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

    public void RemoveSlime(GameObject slime)
    {
        if (slime != null)
        {
            spawnedSlimes.Remove(slime); // Remove the slime from the list
            Destroy(slime); // Destroy the slime
        }
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

        if (isTargetingPlayer)
        {
            // Move towards the player
            bossReferences.target = bossReferences.player; // Set the target to the player
            Debug.Log("New target (Player):" + bossReferences.target.position); // Log the new target position
        }
        else if (isMovingToRandomWaypoint)
        {

            // Move towards a random waypoint
            List<Transform> waypoints = DiscardNearWaypoints(bossReferences.GetWaypoints()); // Get the waypoints and discard the ones too close to the player
            int randomIndex = Random.Range(0, waypoints.Count);
            bossReferences.target = waypoints[randomIndex]; // Set the target to a random waypoint
            //bossReferences.navMeshAgent.SetDestination(bossReferences.target.position); // Set the destination to the random waypoint
            Debug.Log("New target (Random Waypoint):" + bossReferences.target.position); // Log the new target position
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
            Debug.Log("New target (Furthest Waypoint from Player):" + bossReferences.target.position); // Log the new target position

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
        return distanceToTarget <= bossReferences.navMeshAgent.stoppingDistance + 1f;
    }

    public void UpdateAttackIndex()
    {
        // Update the attack index for the next attack
        attackIndex = (attackIndex + 1) % bossReferences.GetAttacksPerRotation(); // Loop through the attack index
    }


    //////////////////////// Coroutines to attack the player ////////////////////////

    public void WidowBasicAttack()
    {
        // Will instantiate the basic attack prefab
        Debug.Log("Widow Basic Attack");
        if (bossReferences.target.gameObject.name != "Player")
        {
            Debug.LogError("Target is not the player!"); // Log an error if the target is not the player
            return;
        }

        // Instantiates the basic attack prefab on the boss position (but substracts 2f to the y position, since the boss is too tall)
        GameObject attackPrefab = Instantiate(bossReferences.attackPrefabs[attackIndex], new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 2f, gameObject.transform.position.z), Quaternion.identity); // Instantiate the attack prefab
        UpdateAttackIndex(); // Update the attack index for the next attack
        Destroy(attackPrefab, 0.3f); // Destroy the attack prefab after it has been used

    }

    private IEnumerator WidowBasicAttackAnimationDuration()
    {
        // Wait for the animation to finish
        yield return null; // Wait for the next frame
        bossReferences.animator.ResetTrigger("Attack"); // Reset the attack trigger
        Debug.Log("Waiting for the basic attack animation to finish... " + bossReferences.animator.GetCurrentAnimatorStateInfo(0).length); // Log that the animation is being waited for
        Debug.Log("Current Animation State: " + bossReferences.animator.GetCurrentAnimatorStateInfo(0).IsName("Attack_Basic")); // Log the current animation state
        yield return new WaitForSeconds(bossReferences.animator.GetCurrentAnimatorStateInfo(0).length);
        if(bossReferences.GetCurrentState() == EnumBossesStates.Dead) yield break; // If the boss is dead, do not continue
        // Set the boss state to idle after the animation

        bossReferences.navMeshAgent.isStopped = true; // Stop the navMeshAgent
        bossReferences.navMeshAgent.ResetPath(); // Reset the path of the navMeshAgent
        bossReferences.SetCurrentState(EnumBossesStates.AttackFollowUp);
        bossReferences.isAttacking = false; // Set the isAttacking flag to false
    }

    public void WidowJumpAttack()
    {
        if (attackIndex != 1) return;
        // Will instantiate the jump attack prefab
        Debug.Log("Widow Jump Attack");
        if (bossReferences.target.gameObject.name != "Player")
        {
            Debug.LogError("Target is not the player!"); // Log an error if the target is not the player
            return;
        }

        // Instantiates the jump attack prefab on the boss position but at y=0
        GameObject attackPrefab = Instantiate(bossReferences.attackPrefabs[attackIndex], new Vector3(gameObject.transform.position.x, 0, gameObject.transform.position.z), Quaternion.identity); // Instantiate the attack prefab
        UpdateAttackIndex(); // Update the attack index for the next attack
        Destroy(attackPrefab, 1.7f); // Destroy the attack prefab after it has been used
    }

    private IEnumerator JumpAttackDeley()
    {

        // Waits a random time before landing, to make the attack less predictable Between 1.5 an 3.5 seconds
        //gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 10.0f, gameObject.transform.position.z); // Set the boss position to jump
        // Disables the rigidbody and collider of the boss since it will be jumping
        gameObject.GetComponent<Rigidbody>().isKinematic = false; // Set the Rigidbody to kinematic
        gameObject.GetComponent<Collider>().enabled = false; // Disable the collider

        float randomDelay = Random.Range(1.5f, 3.5f);

        // Wait for the jump attack animation to finish
        yield return new WaitForSeconds(randomDelay);
        gameObject.GetComponent<Rigidbody>().isKinematic = true; // Set the Rigidbody to non-kinematic
        gameObject.GetComponent<Collider>().enabled = true; // Enable the collider
        Debug.Log("Player position: " + bossReferences.player.position);
        gameObject.transform.position = new Vector3(bossReferences.player.position.x, gameObject.transform.position.y, bossReferences.player.position.z); // Set the boss position to jump
        bossReferences.animator.SetTrigger("AttackLand"); // Trigger the jump attack animation
        yield return null; // Wait for the next frame
        yield return new WaitForSeconds(bossReferences.animator.GetCurrentAnimatorStateInfo(0).length); // Wait for the animation to finish
        if(bossReferences.GetCurrentState() == EnumBossesStates.Dead) yield break; // If the boss is dead, do not continue
        isTargetingPlayer = false;

        // Randomly chooses to move to the furthest waypoint or to a random waypoint
        if (Random.Range(0, 2) == 0)
            isMovingToRandomWaypoint = true; // Move to a random waypoint
        else
            isMovingToRandomWaypoint = false; // Move to the fur

        MovingBehaviour(); // Call the MovingBehaviour method to select the new target
        bossReferences.SetCurrentState(EnumBossesStates.Moving); // Set the boss state to idle after the animation
        bossReferences.isAttacking = false; // Set the isAttacking flag to false
    }

    public void WidowAreaAttack()
    {
        // Will instantiate the area attack prefab
        Debug.Log("Widow Area Attack");
        if (bossReferences.target.gameObject.name == "Player")
        {
            Debug.LogError("Target is not the player!"); // Log an error if the target is not the player
            return;
        }
        // Instantiates between 3 and 5 attack prefabs arround the boss position
        int randomAmount = Random.Range(3, 5+1); // Randomly chooses between 3 and 5 attack prefabs

        // Spawns the attacks surrounding the boss, starting from the nort, east, south and west, Then will spawn the rest starting from the north-east, north-west, south-east and south-west
        for (int i = 0; i < randomAmount; i++)
        {

            // Randomly chooses a position around the boss
            Vector3 randomPosition = new Vector3(gameObject.transform.position.x + Random.Range(-5f, 5f), 1.2f, gameObject.transform.position.z + Random.Range(-5f, 5f)); // Random position around the boss
            GameObject attackPrefab = Instantiate(bossReferences.attackPrefabs[attackIndex], randomPosition, Quaternion.identity); // Instantiate the attack prefab
            attackPrefab.GetComponent<ElementalOrbMovement>().SetWidow(gameObject); // Set the widow reference in the attack prefab
            spawnedOrbs.Add(attackPrefab); // Add the attack prefab to the list of spawned orbs

        }

        UpdateAttackIndex(); // Update the attack index for the next attack

    }
    private IEnumerator AreaAttackDelay()
    {
        yield return null;
        // Wait for the area attack animation to finish
        yield return new WaitForSeconds(bossReferences.animator.GetCurrentAnimatorStateInfo(0).length);
        if(bossReferences.GetCurrentState() == EnumBossesStates.Dead) yield break; // If the boss is dead, do not continue

        if (bossReferences.GetAttacksPerRotation() > 3)
        {
            bossReferences.SetCurrentState(EnumBossesStates.Moving); // Set the boss state to moving after the animation
        }
        else
        {
            bossReferences.SetCurrentState(EnumBossesStates.Idle); // Set the boss state to idle after the animation
        }

        isMovingToRandomWaypoint = false; // Stop moving to a random waypoint
        isTargetingPlayer = true; // Start targeting the player again
        MovingBehaviour(); // Call the MovingBehaviour method to select the new target
        bossReferences.isAttacking = false; // Set the isAttacking flag to false
    }

    private IEnumerator SpitAttackAnimation()
    {
        yield return null; // Wait for the next frame
        yield return new WaitForSeconds(bossReferences.animator.GetCurrentAnimatorStateInfo(0).length); // Wait for the animation to finish
        if(bossReferences.GetCurrentState() == EnumBossesStates.Dead) yield break; // If the boss is dead, do not continue

        bossReferences.navMeshAgent.isStopped = true; // Stop the navMeshAgent
        bossReferences.navMeshAgent.ResetPath(); // Reset the path of the navMeshAgent
        if (bossReferences.GetAttacksPerRotation() > 4)
        {
            bossReferences.SetCurrentState(EnumBossesStates.AttackFollowUp); // Set the boss state to moving after the animation
        }
        else
        {
            bossReferences.SetCurrentState(EnumBossesStates.Idle); // Set the boss state to idle after the animation
        }
        bossReferences.isAttacking = false; // Set the isAttacking flag to false
    }

    private void RotateToPlayer(Transform attackPrefabTransform)
    {
        Vector3 dir = bossReferences.player.position - attackPrefabTransform.position;
        dir.y = 0f;                                 // rotate only on the y axis
        if (dir.sqrMagnitude < 0.001f) return;      // if the distance is too small, do nothing

        attackPrefabTransform.rotation = Quaternion.LookRotation(dir);
    }

    public void WidowSpitAttack()
    {
        // Will instantiate the spit attack prefab
        Debug.Log("Widow Spit Attack");
        if (bossReferences.target.gameObject.name != "Player")
        {
            Debug.LogError("Target is not the player!"); // Log an error if the target is not the player
            return;
        }

        // Instantiates the spit attack prefab on the boss position but at y=0
        GameObject attack = Instantiate(bossReferences.attackPrefabs[attackIndex], new Vector3(gameObject.transform.position.x, 1.2f, gameObject.transform.position.z), Quaternion.identity); // Instantiate the attack prefab
        RotateToPlayer(attack.transform); // Rotate the attack prefab to face the player
        UpdateAttackIndex(); // Update the attack index for the next attack
        //Destroy(attackPrefab, 1.7f); // Destroy the attack prefab after it has been used
    }

    private IEnumerator SecondJumpAttackAnimation()
    {

        // Waits a random time before landing, to make the attack less predictable Between 1.5 an 3.5 seconds
        //gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 10.0f, gameObject.transform.position.z); // Set the boss position to jump
        // Disables the rigidbody and collider of the boss since it will be jumping
        gameObject.GetComponent<Rigidbody>().isKinematic = false; // Set the Rigidbody to non-kinematic
        gameObject.GetComponent<Collider>().enabled = false; // Disable the collider

        float randomDelay = Random.Range(1.5f, 3.5f);

        // Wait for the jump attack animation to finish
        yield return new WaitForSeconds(randomDelay);
        gameObject.GetComponent<Rigidbody>().isKinematic = true; // Set the Rigidbody to kinematic
        gameObject.GetComponent<Collider>().enabled = true; // Enable the collider
        Debug.Log("Player position: " + bossReferences.player.position);

        bossReferences.animator.SetTrigger("Attack5_Landing"); // Trigger the jump attack animation

        // Wait for the animation to finish
        yield return null; // Wait for the next frame
        yield return new WaitForSeconds(bossReferences.animator.GetCurrentAnimatorStateInfo(0).length);
        if(bossReferences.GetCurrentState() == EnumBossesStates.Dead) yield break; // If the boss is dead, do not continue
        // Set the boss state to idle after the animation
        bossReferences.navMeshAgent.isStopped = true; // Stop the navMeshAgent
        bossReferences.navMeshAgent.ResetPath(); // Reset the path of the navMeshAgent
        bossReferences.SetCurrentState(EnumBossesStates.Idle);
        bossReferences.isAttacking = false; // Set the isAttacking flag to false
        isTargetingPlayer = true; // Start targeting the player again
        isMovingToRandomWaypoint = false; // Stop moving to a random waypoint
        MovingBehaviour(); // Call the MovingBehaviour method to select the new target

    }

    public void WidowLandingAreaAttack()
    {
        // Will instantiate the landing area attack prefab
        Debug.Log("Widow Landing Area Attack");
        if (bossReferences.target.gameObject.name != "Player")
        {
            Debug.LogError("Target is not the player!"); // Log an error if the target is not the player
            return;
        }

        // Instantiates the landing area attack prefab on the boss position but at y=0.2
        // The attack is a circle area of poison
        Instantiate(bossReferences.attackPrefabs[attackIndex], new Vector3(gameObject.transform.position.x, 0.2f, gameObject.transform.position.z), Quaternion.identity); // Instantiate the attack prefab
        UpdateAttackIndex(); // Update the attack index for the next attack
    }

    public void WidowSummon()
    {

        // Generates one of each slime prefab, each in a random waypoint position
        if (slimesPrefabs.Length == 0)
        {
            Debug.LogError("No slimes prefabs assigned!"); // Log an error if no slimes prefabs are assigned
            return;
        }

        List<Transform> spawn_waypoints = DiscardNearWaypoints(bossReferences.GetWaypoints()); // Get the waypoints and discard the ones too close to the player
        if (spawn_waypoints.Count == 0)
        {
            Debug.LogError("No waypoints available to spawn slimes!"); // Log an error if no waypoints are available
            return;
        }

        for (int i = 0; i < slimesPrefabs.Length; i++)
        {
            int randomIndex = Random.Range(0, spawn_waypoints.Count); // Get a random index from the waypoints list
            Transform randomWaypoint = spawn_waypoints[randomIndex]; // Get the random waypoint
            spawn_waypoints.RemoveAt(randomIndex); // Remove the random waypoint from the list to avoid spawning multiple slimes in the same position
            GameObject slime = Instantiate(slimesPrefabs[i], randomWaypoint.position, Quaternion.identity); // Instantiate the slime prefab at the random waypoint position
            spawnedSlimes.Add(slime); // Add the slime to the list of spawned slimes
            Debug.Log("Spawned slime: " + slime.name + " at position: " + randomWaypoint.position); // Log the spawned slime and its position
        }
        
    }


}
