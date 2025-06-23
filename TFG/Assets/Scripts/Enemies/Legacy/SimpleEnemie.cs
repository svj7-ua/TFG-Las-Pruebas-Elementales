using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Only tries to persue the enemie
public class SimpleEnemie : EnemyBehaviour
{
    private string enemy_name = "SimpleEnemie_"; // Name of the enemy

    private float attackCooldown; // Cooldown duration for attacks
    private float lastAttackTime = 0f; // Time of the last attack
    

    public SimpleEnemie(EnemyReferences enemyReferences, Transform target, Transform enemy, float attackCD=1f, int numberID=0) : base(enemyReferences, target, enemy)
    {

        enemy_name += numberID; // Name of the enemy
        attackCooldown = attackCD; // Set the attack cooldown
    }

    public override void Tick(){
        
                
        if (target != null)
        {

            bool isTargetInRange = Vector3.Distance(enemy.position, target.position) <= stoppingDistance;
            if (!isTargetInRange)
            {
                enemyReferences.canAttack = false; // Set the canAttack flag to false to prevent immediate re-attack
                UpdatePath();
            }
            else
            {
                CanAttack(); // Call the Attack method when in range
            }

        }

    }

    public override void CanAttack()
    {

        //enemyReferences.SetAttacking(); // Set the attacking flag to true
        enemyReferences.canAttack = true; // Set the canAttack flag to false to prevent immediate re-attack


    }

    public override Quaternion AttackDirection(Transform origin, Transform Target)
    {

        return Quaternion.identity; // Return the identity quaternion (no rotation)

    }


    public override void StartAttackCooldown()
    {
        lastAttackTime = Time.time; // Set the last attack time to the current time
        attackIsReady = false; // Set the canAttack flag to false to prevent immediate
    }

    public override void UpdateCooldown()
    {
        if (Time.time >= lastAttackTime + attackCooldown) // Check if the cooldown time has passed
        {
            attackIsReady = true; // Set the attack ready flag to true
        }
    }

}
