using UnityEngine;
public class EnemyBehaviourFactory
{

    public static EnemyBehaviour CreateEnemyBehaviour(EnemyReferences enemyReferences, Transform target, Transform enemy, int numberID = 0, EnumEnemyBehaviours enemyBehaviourType = EnumEnemyBehaviours.SimpleEnemie, float attackCD = 1f)
    {
        switch (enemyBehaviourType)
        {
            case EnumEnemyBehaviours.SimpleEnemie:
                return new SimpleEnemie(enemyReferences, target, enemy, attackCD, numberID);
            // Add more cases for different enemy behaviours here
            default:
                Debug.LogWarning("Enemy behaviour type not recognized. Defaulting to SimpleEnemie.");
                return new SimpleEnemie(enemyReferences, target, enemy, attackCD,numberID);
        }
    }


    
}