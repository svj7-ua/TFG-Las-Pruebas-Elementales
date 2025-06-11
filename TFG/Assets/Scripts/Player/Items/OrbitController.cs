using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitController : MonoBehaviour
{

    [SerializeField]
    private GameObject lightningOrb; // Prefab of the orbiting orb
    [SerializeField]
    private GameObject fireOrb; // Prefab of the orbiting orb
    [SerializeField]
    private GameObject poisonOrb; // Prefab of the orbiting orb
    [SerializeField]
    private GameObject ArcaneOrb; // Prefab of the orbiting orb


    [SerializeField]
    private GameObject centerOfOrbit; // The center point around which the orbs will orbit


    [Space]
    [Header("Orbit Settings")]
    [Range(1.0f, 100.0f)]
    [SerializeField]
    private float orbitSpeed = 80f; // Speed of the orbiting orbs

    [SerializeField]
    private List<GameObject> orbits;
    private int currentNumberOfOrbs = 0; // Current number of orbs active

    
    [Space]
    [SerializeField]
    private bool hasAtLeastOneOrb = false; // Flag to check if the player has at least one orb

    // Start is called before the first frame update
    void Start()
    {

        if (orbits == null)
        {
            Debug.LogError("OrbitController: orbits list is not set. Please assign the orbits in the inspector.");
            return;
        }

        if (centerOfOrbit == null)
        {
            Debug.LogError("OrbitController: centerOfOrbit is not set. Please assign a center point for the orbs to orbit around.");
            return;
        }

    }

    void FixedUpdate()
    {
        if (hasAtLeastOneOrb)
        {
            // Rotate the orbs around the center point
            RotateOrbs();
        }
    }


    private void RotateOrbs()
    {

        //Rotates centerOfOrbit around its Y axis
        centerOfOrbit.transform.Rotate(Vector3.up, orbitSpeed * Time.deltaTime);

    }


    public void ActivateOrb(EnumElementTypes element)
    {
        Debug.Log($"OrbitController: Activating orb for element: {element}");

        switch (element)
        {
            case EnumElementTypes.Lightning:

                // Generates an instance of the lightning orb at the position of the current index
                GameObject lightningInstance = Instantiate(lightningOrb, new Vector3(0, 0, 0), Quaternion.identity);
                lightningInstance.transform.SetParent(orbits[currentNumberOfOrbs].transform); // Set the parent of the orb to the center of orbit
                lightningInstance.transform.localPosition = Vector3.zero; // Set the position of the orb to the center of orbit
                lightningOrb.SetActive(true); // Activate the lightning orb
                currentNumberOfOrbs++; // Increment the number of orbs active
                break;

            case EnumElementTypes.Fire:
                // Generates an instance of the fire orb at the position of the current index
                GameObject fireInstance = Instantiate(fireOrb, new Vector3(0, 0, 0), Quaternion.identity);
                fireInstance.transform.SetParent(orbits[currentNumberOfOrbs].transform); // Set the parent of the orb to the center of orbit
                fireInstance.transform.localPosition = Vector3.zero; // Set the position of the orb to the center of orbit
                fireOrb.SetActive(true); // Activate the fire orb
                currentNumberOfOrbs++; // Increment the number of orbs active
                break;

            case EnumElementTypes.Poison:
                // Generates an instance of the poison orb at the position of the current index
                GameObject poisonInstance = Instantiate(poisonOrb, new Vector3(0, 0, 0), Quaternion.identity);
                poisonInstance.transform.SetParent(orbits[currentNumberOfOrbs].transform); // Set the parent of the orb to the center of orbit
                poisonInstance.transform.localPosition = Vector3.zero; // Set the position of the orb to the center of orbit
                poisonOrb.SetActive(true); // Activate the poison orb
                currentNumberOfOrbs++; // Increment the number of orbs active
                break;

            case EnumElementTypes.Arcane:
                // Generates an instance of the arcane orb at the position of the current index
                GameObject arcaneInstance = Instantiate(ArcaneOrb, new Vector3(0, 0, 0), Quaternion.identity);
                arcaneInstance.transform.SetParent(orbits[currentNumberOfOrbs].transform); // Set the parent of the orb to the center of orbit
                arcaneInstance.transform.localPosition = Vector3.zero; // Set the position of the orb to the center of orbit
                ArcaneOrb.SetActive(true); // Activate the arcane orb
                currentNumberOfOrbs++; // Increment the number of orbs active
                break;

            default:
                Debug.LogWarning("OrbitController: Unknown element type. No orb activated.");
                return;
        }


        if (!hasAtLeastOneOrb)
        {
            hasAtLeastOneOrb = true; // Set the flag to true if the player has at least one orb
            Debug.Log("OrbitController: At least one orb is now active.");
        }

    }

    public int GetCurrentNumberOfOrbs()
    {
        return currentNumberOfOrbs; // Return the current number of orbs active
    }
    
    // TODO: PROBABLY THE OPTION OF DROPPING ITEMS WILL BE REMOVED
    public void DeactivateOrb(EnumElementTypes element)
    {

        return;

        // switch (element)
        // {
        //     case EnumElementTypes.Lightning:
        //         lightningOrb.SetActive(false); // Deactivate the lightning orb
        //         break;

        //     case EnumElementTypes.Fire:
        //         fireOrb.SetActive(false); // Deactivate the fire orb
        //         break;

        //     case EnumElementTypes.Poison:
        //         poisonOrb.SetActive(false); // Deactivate the poison orb
        //         break;

        //     case EnumElementTypes.Arcane:
        //         ArcaneOrb.SetActive(false); // Deactivate the arcane orb
        //         break;

        //     default:
        //         Debug.LogWarning("OrbitController: Unknown element type. No orb deactivated.");
        //         break;
        // }

        // // Check if all orbs are deactivated
        // if (!lightningOrb.activeSelf && !fireOrb.activeSelf && !poisonOrb.activeSelf && !ArcaneOrb.activeSelf)
        // {
        //     hasAtLeastOneOrb = false; // Reset the flag if no orbs are active
        // }

    }
}
