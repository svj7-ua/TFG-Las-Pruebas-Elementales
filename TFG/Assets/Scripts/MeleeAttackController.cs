using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        // Obtener el componente Animator del objeto
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Comprobar si la animación ha terminado
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 && !animator.IsInTransition(0))
        {
            // Destruir el objeto al terminar la animación
            Destroy(gameObject);
        }
    }
}
