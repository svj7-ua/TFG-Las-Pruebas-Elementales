using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BossGeneralBehaviour : MonoBehaviour
{
    private BossReferences bossReferences;

    private float pathUpdateDeadline = 0f; // Delay between path updates in seconds

    //private float stateCheckDelay = 0.2f; // Delay between state checks in seconds
    //private float stateCheckDeadline = 0f; // Deadline for the next state check

    private EnumBossesStates lastState = EnumBossesStates.None; // Current state of the boss

    private IBossBehaviours bossBehaviour; // Reference to the boss behaviour interface

    [Header("Attacks Settings")]
    private Hurtbox hurtbox; // Reference to the Hurtbox component

    //private bool checkHitAnimation = false; // Flag to check if the hit animation is playing

    private void Awake()
    {
        bossReferences = GetComponent<BossReferences>();
        hurtbox = GetComponent<Hurtbox>(); // Get the Hurtbox component
        bossReferences.target = GameObject.FindGameObjectWithTag("Player").transform;
        bossReferences.player = bossReferences.target; // Set the player reference
        bossBehaviour = GetComponent<IBossBehaviours>(); // Get the IBossBehaviours component

    }

    // Start is called before the first frame update
    void Start()
    {
        hurtbox.SetBossReferences(bossReferences); // Set the boss references in the Hurtbox component
        if(bossBehaviour == null)
        {
            //Debug.LogError("IBossBehaviours component not found!"); // Log an error if the IBossBehaviours component is not found
        }

    }

    // Update is called once per frame
    void Update()
    {

        if (bossReferences.GetCurrentState() == EnumBossesStates.Dead) // Check if the boss is dead
        {
            if (lastState != EnumBossesStates.Dead) StopBehaviour(); // Stop the boss behaviour
            lastState = EnumBossesStates.Dead; // Update the last state to Dead
            return; // Exit the update method if the boss is dead
        }

        if (bossReferences.isAttacking == false && lastState != bossReferences.GetCurrentState()) // Check if the state has changed
        {
            Debug.LogWarning("State changeg! From: " + lastState.ToString() + " To: " + bossReferences.GetCurrentState().ToString()); // Log the state change
            switch (bossReferences.GetCurrentState())
            {
                case EnumBossesStates.Dead:
                    StopBehaviour(); // Stop the boss behaviour

                    break;
                case EnumBossesStates.Idle:
                    IdleBehaviour(); // Call the idle behaviour
                    break;
                case EnumBossesStates.Stunned:
                    // NOTE: This may not be needed as it's handled in the Hurtbox
                    bossReferences.target = bossReferences.player; // Set the target to the player
                    return;
                case EnumBossesStates.Moving:
                    break;
                case EnumBossesStates.Attack:
                    bossBehaviour.AttackBehaviour(); // Call the attack behaviour
                    break;
                case EnumBossesStates.AttackFollowUp:
                    // Attack follow up acts when and attack if followed by another attack, without moving.
                    bossBehaviour.AttackBehaviour(); // Call the attack behaviour
                    break;
                case EnumBossesStates.PursueAttack:
                    // Pursue attack acts when the boss is attacking and the player is out of range.
                    bossBehaviour.AttackBehaviour(); // Call the pursue attack behaviour
                    break;
                case EnumBossesStates.None:
                    //Debug.LogWarning("State is None!"); // Log a warning if the state is None
                    bossReferences.SetCurrentState(EnumBossesStates.Idle); // Set the state to Idle
                    break;
                default:
                    //Debug.LogWarning("State not handled! Check State transitions. Current State: " + bossReferences.GetCurrentState().ToString()); // Log a warning if the state is not handled
                    bossReferences.SetCurrentState(EnumBossesStates.Idle); // Set the state to Idle
                    break;
            }

            lastState = bossReferences.GetCurrentState(); // Update the last state
        }

        if (bossReferences.animator.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
            return;
        
        if (bossReferences.GetCurrentState() == EnumBossesStates.Moving || bossReferences.GetCurrentState() == EnumBossesStates.PursueAttack) // Check if the boss is not dead
        {
            UpdatePath(); // Update the path to the target
        }

        if(bossReferences.GetCurrentState() == EnumBossesStates.Moving && bossBehaviour.IsBossReadyToAttack()){
            bossReferences.SetCurrentState(EnumBossesStates.Attack); // Set the state to Attack
        }

        bossReferences.animator.SetFloat("Movement", bossReferences.navMeshAgent.velocity.magnitude); // Set the movement speed in the animator
        
        // Checks if the boss is going to the left, if so it will flip the sprite
        if (bossReferences.navMeshAgent.velocity.x < 0)
        {
            bossReferences.spriteRenderer.flipX = true; // Flip the sprite to the left   
        }
        else if (bossReferences.navMeshAgent.velocity.x > 0)
        {
            bossReferences.spriteRenderer.flipX = false; // Flip the sprite to the right
        }

    }

    public void BossDeath()
    {
        gameObject.SetActive(false); // Deactivate the boss game object
        bossReferences.OpenPortal(); // Open the portal if both bosses are dead
    }

    private void StopBehaviour()
    {
        bossReferences.navMeshAgent.isStopped = true; // Stop the navMeshAgent
        gameObject.GetComponent<Rigidbody>().isKinematic = false; // Set the Rigidbody to kinematic
        gameObject.GetComponent<Collider>().enabled = false; // Disable the collider
        bossReferences.navMeshAgent.ResetPath(); // Reset the path of the navMeshAgent
        //bossReferences.navMeshAgent.enabled = false; // Disable the navMeshAgent
        bossReferences.animator.SetTrigger("Dead"); // Trigger the death animation
    }

    private void UpdatePath()
    {
        if (Time.time >= pathUpdateDeadline) // Check if the path update deadline has passed
        {
            pathUpdateDeadline = Time.time + bossReferences.pathUpdateDelay; // Set the new path update deadline
            if (bossReferences.navMeshAgent.enabled) // Check if the navMeshAgent is enabled
            {
                bossReferences.navMeshAgent.SetDestination(bossReferences.target.position); // Set the destination to the target position
            }
        }
    }

    public void SetCurrentState(EnumBossesStates newState)
    {
        bossReferences.SetCurrentState(newState); // Set the current state of the boss
    }

    private void IdleBehaviour()
    {
        // Sets the boss navMeshAgent to not move
        bossReferences.navMeshAgent.isStopped = true; // Stop the navMeshAgent
        bossReferences.navMeshAgent.ResetPath(); // Reset the path of the navMeshAgent
        StartCoroutine(WaitToChangeBehaviour(6.0f, EnumBossesStates.Moving)); // Wait for 2 seconds and then change to moving state
    }

    IEnumerator WaitToChangeBehaviour(float time, EnumBossesStates newState)
    {
        yield return new WaitForSeconds(time); // Wait for the specified time
        if(bossReferences.GetCurrentState() == EnumBossesStates.Idle)
            bossReferences.SetCurrentState(newState); // Set the current state to the new state
    }

}
