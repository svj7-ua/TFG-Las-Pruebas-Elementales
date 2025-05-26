using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBossBehaviours
{
    void AttackBehaviour(); // Method to handle attack behaviour
    void MovingBehaviour(); // Method to handle moving behaviour

    void DeathBehaviour(); // Method to handle death behaviour

    bool IsBossReadyToAttack(); // Method to check if the boss is ready to attack

    void UpdateAttackIndex();
}
