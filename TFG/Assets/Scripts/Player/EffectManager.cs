using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance { get; private set; }

    [Header("Effects Prefabs")]
    public GameObject convokeLightningPrefab;
    public GameObject electricExplosionPrefab;

    public GameObject wildFirePrefab;

    public GameObject poisonPuddlePrefab;

    public GameObject healingAreaPrefab;

    public GameObject fireballExplosionPrefab; // Prefab for fireball explosion effect

    public GameObject windShieldPrefab; // Prefab for wind shield effect
    

    public GameObject meteorRainPrefab; // Prefab for meteor rain effect

    [Space]
    [Header("Deprecated Effects")]
    public GameObject tornadoPrefab; // Prefab for tornado effect

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Asegura que solo haya un EffectManager en la escena
        }
    }
}
