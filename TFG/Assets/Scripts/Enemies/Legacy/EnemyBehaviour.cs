using UnityEngine;

public abstract class EnemyBehaviour
{

    public Transform target;

    public Transform enemy;

    public EnemyReferences enemyReferences;

    public float stoppingDistance;

    protected bool attackIsReady = true; // Flag to check if the attack is ready
    public float pathUpdateDeadline = 0f; // Delay between path updates in seconds

    public EnemyBehaviour(EnemyReferences enemyReferences, Transform target, Transform enemy)
    {
        this.enemyReferences = enemyReferences;
        this.target = target;
        this.enemy = enemy;
        stoppingDistance = enemyReferences.navMeshAgent.stoppingDistance;
    }

    public abstract void Tick();

    public abstract void CanAttack();

    public abstract void UpdateCooldown();

    public abstract Quaternion AttackDirection(Transform origin, Transform Target);

    public bool IsAttackReady()
    {
        return attackIsReady; // Return the attack ready flag
    }

    public abstract void StartAttackCooldown();

    public void AttackAnimation(){
        enemyReferences.animator.SetTrigger("Attack"); // Trigger the attack animation
    }

    public void UpdatePath()
    {

        if (Time.time >= pathUpdateDeadline)
        {
            pathUpdateDeadline = Time.time + enemyReferences.pathUpdateDelay;
            if(enemyReferences.navMeshAgent.enabled)    enemyReferences.navMeshAgent.SetDestination(target.position);
        }

    }
    
}