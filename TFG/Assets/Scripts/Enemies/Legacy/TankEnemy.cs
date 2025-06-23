using UnityEngine;

public class TankEnemy : EnemyBehaviour
{
    private string enemy_name = "SimpleEnemie_"; // Name of the enemy

    private float attackCooldown; // Cooldown duration for attacks
    private float lastAttackTime = 0f; // Time of the last attack
    

    public TankEnemy(EnemyReferences enemyReferences, Transform target, Transform enemy, float attackCD=1f, int numberID=0) : base(enemyReferences, target, enemy)
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

    public override Quaternion AttackDirection(Transform origin, Transform target)
    {
        // Debug.Log("AttackDirection called in TankEnemy"); // Debug log to check if the method is called
        // Debug.Log("Origin: " + origin.position + " Target: " + target.position); // Log the origin position
        // Debug.Log("Rotation: " + Quaternion.LookRotation((origin.position - target.position).normalized)); // Log the rotation

        // Vector3 direction = (origin.position - target.position).normalized;
        // return Quaternion.LookRotation(direction);
        Debug.Log("AttackDirection called in TankEnemy");
        Debug.Log("Origin: " + origin.position + " Target: " + target.position);

        // Calcula la dirección en el plano XZ ignorando el eje Y
        Vector3 flatDirection = target.position - origin.position;
        flatDirection.y = 0f; // Elimina la componente vertical

        if (flatDirection == Vector3.zero)
        {
            // Evita errores si están exactamente en la misma posición
            return Quaternion.identity;
        }

        Quaternion rotation = Quaternion.Euler(0f, Mathf.Atan2(flatDirection.x, flatDirection.z) * Mathf.Rad2Deg, 0f);

        //Invierte la rotación en el eje Y para que el ataque vaya hacia el objetivo
        rotation = Quaternion.Euler(0f, rotation.eulerAngles.y - 90f, 0f);

        Debug.Log("Flat Rotation: " + rotation);
        return rotation;
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