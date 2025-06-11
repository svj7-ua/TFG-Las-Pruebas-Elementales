using System.ComponentModel;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class MagicianBehaviour : MonoBehaviour
{

    public Transform target;

    private EnemyReferences enemyReferences;

    [Header("Attacks Settings")]
    [SerializeField]
    private float summonOrbCooldown = 10f; // Cooldown duration for attacks
    [SerializeField]
    private float healingCooldown = 8f; // Cooldown duration for ranged attacks

    [SerializeField]
    private float attack_yOffset = 1f; // Y offset for the attack prefab instantiation

    private bool healingReady = true; // Flag to check if the ranged attack is ready
    private bool orbSpawnReady = true; // Flag to check if the melee attack

    private bool inStunnedCooldown = false; // Flag to check if the enemy is in stunned cooldown

    private Hurtbox hurtbox; // Reference to the Hurtbox component

    private List<GameObject> spawnedOrbs = new List<GameObject>(); // List to keep track of spawned orbs

    private Health health;
    
    [SerializeField]
    private int maxOrbs = 3; // Maximum number of orbs that can be spawned

    [SerializeField]
    [Range(0, 125)]
    private float healingAmmount = 25f; // Amount of health to heal

    private void Awake()
    {
        enemyReferences = GetComponent<EnemyReferences>();
        hurtbox = GetComponent<Hurtbox>(); // Get the Hurtbox component
        target = GameObject.FindGameObjectWithTag("Player").transform; // Assuming the target is tagged as "Player"
        health = GetComponent<Health>(); // Get the Health component
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (health.currentHealth <= 0) clearOrbs(); // Clear orbs if the enemy is dead
        if (target == null) return;
        if (hurtbox.IsStunned())
        {

            if(inStunnedCooldown) {
                StopCoroutine("StartStunCooldownHealing"); // Stop the stun cooldown coroutine if the enemy is already in stunned cooldown
                StopCoroutine("StartStunCooldownOrb"); // Stop the stun cooldown coroutine if the enemy is already in stunned cooldown
            }
            StartCoroutine(StartStunCooldownHealing(1.5f));
            StartCoroutine(StartStunCooldownOrb(1.5f));
            return;
        }
        if (hurtbox.IsTrapped()) return;

        float distance = Vector3.Distance(transform.position, target.position);

        // Si el jugador está demasiado cerca, huir
        if (distance <= 8.0f)
        {
            FleeFromTarget();
        }

        if(health.currentHealth <= health.maxHealth/2){
            Heal(); // Heal if the enemy is below 50% health
        }

        Attack(); // Attack if the enemy is not healing

        //Attack(); // Lógica de ataque
        enemyReferences.animator.SetFloat("Movement", enemyReferences.navMeshAgent.velocity.magnitude);
    }

    private void FleeFromTarget()
    {
        if (!enemyReferences.navMeshAgent.enabled) return;

        Vector3 directionAway = (transform.position - target.position).normalized;
        Vector3 bestDestination = Vector3.zero;
        float maxDistanceToPlayer = 0f;

        List<Vector3> candidatePoints = new List<Vector3>();

        // Primer intento: huida directa
        candidatePoints.Add(target.position + directionAway * 12.0f);
        if (TrySetDestination(candidatePoints[0]))
        {
            return; // Si se encuentra un destino válido, salir
        }

        // Intento 2: rodear al jugador en círculo, generando puntos candidatos
        int attempts = 12; // más intentos = más ángulos
        float angleStep = 360f / attempts;

        for (int i = 0; i < attempts; i++)
        {
            float angle = i * angleStep;
            Vector3 rotatedDir = Quaternion.Euler(0, angle, 0) * directionAway;
            Vector3 candidate = target.position + rotatedDir.normalized * 12.0f;
            candidatePoints.Add(candidate);
        }

        // Evaluar los puntos en orden y quedarnos con el más lejano al jugador
        foreach (Vector3 candidate in candidatePoints)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(candidate, out hit, 2.0f, NavMesh.AllAreas))
            {
                float distanceFromPlayer = Vector3.Distance(hit.position, target.position);
                if (distanceFromPlayer > maxDistanceToPlayer)
                {
                    bestDestination = hit.position;
                    maxDistanceToPlayer = distanceFromPlayer;
                }
            }
        }

        // Si se encontró un buen destino, moverse
        if (maxDistanceToPlayer > 0f)
        {
            enemyReferences.navMeshAgent.SetDestination(bestDestination);
        }


    }
    private bool TrySetDestination(Vector3 position)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(position, out hit, 2.0f, NavMesh.AllAreas))
        {
            enemyReferences.navMeshAgent.SetDestination(hit.position);
            return true;
        }
        return false;
    }

    private void Heal(){

        if(hurtbox.IsStunned()) return; // Exit if the enemy is stunned

        if(healingReady)
        {
            healingReady = false; // Set the healing flag to false to prevent immediate re-heal
            enemyReferences.animator.SetTrigger("Heal"); // Trigger the heal animation
            StartCoroutine(HealingCooldown(healingCooldown)); // Start the cooldown for the healing
            StartCoroutine(SummonOrbCooldown(summonOrbCooldown/2)); // Start the cooldown for the orb spawn (half the time, since the magician is healing)
        }

    }

    private void Attack()
    {
        if (hurtbox.IsStunned()) return; // Exit if the enemy is stunned

        if (enemyReferences.attackPrefabs.Length == 0){
            Debug.LogWarning("No attack prefabs assigned in the EnemyReferences component.");
            return; // Check if there are attack prefabs assigned
        }

        if (spawnedOrbs.Count >= maxOrbs) // Check if the maximum number of orbs is reached
        {
            return; // Exit if the maximum number of orbs is reached
        }

        // If the magician can spawn an orb, it will spawn around himself in a circle in a random position

        if (orbSpawnReady)
        {
            orbSpawnReady = false; // Set the orb spawn flag to false to prevent immediate
            enemyReferences.animator.SetTrigger("Attack"); // Trigger the attack animation
            StartCoroutine(SummonOrbCooldown(summonOrbCooldown)); // Start the cooldown for the orb spawn

        }


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

    public void SummonOrbSpell()
    {
        if(!enemyReferences.animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            return; // Exit the coroutine if the animation is not playing
        }
        int randomIndex = Random.Range(0, enemyReferences.attackPrefabs.Length); // Get a random index from the attack prefabs array
        Vector3 position = new Vector3(transform.position.x+Random.Range(-1f, 1f), attack_yOffset, transform.position.z+Random.Range(-1f, 1f)); // Random position around the magician
        GameObject attackPrefab = Instantiate(enemyReferences.attackPrefabs[randomIndex], position, Quaternion.identity); // Instantiate the attack prefab at the specified position and rotation
        attackPrefab.GetComponent<ElementalOrbMovement>().SetMagician(gameObject); // Set the magician reference in the attack prefab
        spawnedOrbs.Add(attackPrefab); // Add the spawned orb to the list
    }

    public void HealingSpell()
    {


        if (!enemyReferences.animator.GetCurrentAnimatorStateInfo(0).IsName("Heal"))
        {
            return; // Exit the coroutine if the animation is not playing
        }
        health.Heal(healingAmmount); // Heal the enemy by 25 health points
    }

    private IEnumerator SummonOrbCooldown(float cooldownTime)
    {
        yield return new WaitForSeconds(cooldownTime); // Wait for the cooldown time
        orbSpawnReady = true; // Set the melee attack flag to true after cooldown
    }

    private IEnumerator HealingCooldown(float cooldownTime)
    {
        yield return new WaitForSeconds(cooldownTime); // Wait for the cooldown time
        healingReady = true; // Set the ranged attack flag to true after cooldown
    }




    // This method adds a small cooldown to the enemy attacks when it is stunned, so it doesn't spam the attack animation just after being unstunned
    private IEnumerator StartStunCooldownOrb(float stunDuration)
    {

        if(!orbSpawnReady) yield break; // Exit the coroutine if the enemy is already in stunned cooldown
        orbSpawnReady = false; // Set the stunned cooldown flag to true

        inStunnedCooldown = true; // Set the stunned cooldown flag to true
        yield return new WaitForSeconds(stunDuration); // Wait for the stun duration
        inStunnedCooldown = false; // Set the stunned cooldown flag to false after the stun duration
        orbSpawnReady = true; // Set the melee attack flag to true after cooldown

    }

        private IEnumerator StartStunCooldownHealing(float stunDuration)
    {

        if(!healingReady) yield break; // Exit the coroutine if the enemy is already in stunned cooldown
        healingReady = false; // Set the stunned cooldown flag to true

        inStunnedCooldown = true; // Set the stunned cooldown flag to true
        yield return new WaitForSeconds(stunDuration); // Wait for the stun duration
        inStunnedCooldown = false; // Set the stunned cooldown flag to false after the stun duration
        healingReady = true; // Set the melee attack flag to true after cooldown
        inStunnedCooldown = false; // Set the stunned cooldown flag to false after the stun duration

    }
}