using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    public List<IEffect> meleeEffectsInventory = new List<IEffect>();
    public List<IEffect> rangedEffectsInventory = new List<IEffect>();
    public List<IEffect> dash_AOE_EffectsInventory = new List<IEffect>();

    [Space]
    [Header("Inventory UI Components")]
    [SerializeField] public GameObject inventoryPanel;
    [SerializeField] public GameObject itemSlotPrefab;
    [SerializeField] public GameObject meleeGrid;
    [SerializeField] public GameObject rangedGrid;
    [SerializeField] public GameObject dashAOE_Grid;


    [Space]
    public int INVENTORY_SIZE = 8;

    private int[] currentItemsCount = {0, 0, 0}; // [melee, ranged, dash]

    private bool[,] hasItem = new bool[3, 8]; // [type, index]
    
    private bool isEffectRepeated(IEffect effect, EnumSpellCardTypes type)
    {
        return hasItem[(int)type, (int)effect.getSpellCard()-1];        // Subtract 1 to get the index, since 0 is none for the spell cards
    }

    private void UpgradeEffect(IEffect effect, EnumSpellCardTypes type)
    {
        var list = type == EnumSpellCardTypes.Melee ? meleeEffectsInventory :
            type == EnumSpellCardTypes.Ranged ? rangedEffectsInventory :
            dash_AOE_EffectsInventory;

        foreach (var item in list){
            if(item.getSpellCard() == effect.getSpellCard()){
                Debug.Log("Upgrading effect: " + item.GetType().Name);
                item.UpgradeEffect(); // Apply the effect to the player
                return; // Exit the loop after applying the effect
            }
        }
    }
    
    public void AddMeleeEffect(IEffect effect)
    {
        if(!isEffectRepeated(effect, EnumSpellCardTypes.Melee)){
            meleeEffectsInventory.Add(effect);
            currentItemsCount[0]++;
            hasItem[(int)EnumSpellCardTypes.Melee, (int)effect.getSpellCard()-1] = true; // Set the item as added
        } else {
            UpgradeEffect(effect, EnumSpellCardTypes.Melee); // Upgrade the effect if it already exists
        }
        
        Debug.Log("Added impact effect:" + effect.GetType().Name);
    }

    public void AddRangedEffect(IEffect effect)
    {
        if(!isEffectRepeated(effect, EnumSpellCardTypes.Ranged)){
            rangedEffectsInventory.Add(effect);
            currentItemsCount[1]++;
            hasItem[(int)EnumSpellCardTypes.Ranged, (int)effect.getSpellCard()-1] = true; // Set the item as added
        } else {
            UpgradeEffect(effect, EnumSpellCardTypes.Ranged); // Upgrade the effect if it already exists
        }
        
        Debug.Log("Added ranged effect:" + effect.GetType().Name);
    }

    public void AddDashEffect(IEffect effect)
    {
        if(!isEffectRepeated(effect, EnumSpellCardTypes.Dash_AOE)){
            dash_AOE_EffectsInventory.Add(effect);
            currentItemsCount[2]++;
            hasItem[(int)EnumSpellCardTypes.Dash_AOE, (int)effect.getSpellCard()-1] = true; // Set the item as added
        } else {
            UpgradeEffect(effect, EnumSpellCardTypes.Dash_AOE); // Upgrade the effect if it already exists
        }
        Debug.Log("Added dash effect:" + effect.GetType().Name);
    }



    public int[] GetCurrentItemsCount()
    {
        return currentItemsCount;
    }
    
    public void ListEffects()
    {
        Debug.Log("Effects Inventory: ");

        ListMelee(); // Listar efectos de impacto

        ListRanged(); // Listar efectos de proyectiles

        ListDash(); // Listar efectos de dash/AOE

    }

    private void ListMelee(){
        ItemPanel[] slots = meleeGrid.GetComponentsInChildren<ItemPanel>();
        Debug.Log("Listing impact effects: " + meleeEffectsInventory.Count);
        int index = 0;
        foreach (var effect in meleeEffectsInventory)
        {
            Debug.Log("Adding to inventory: " + effect.GetType().Name);
            if (index >= slots.Length) break; // Evitar desbordamiento si hay más efectos que espacios

            // Obtener el ícono del efecto

            // Buscar el componente Image en el slot actual

            Debug.Log("Slot " + slots[index].debugName + " setting icon");
            slots[index].icon.gameObject.SetActive(true); // Asegurarse de que el icono esté activo
            slots[index].icon.sprite = effect.getIcon(); // Asignar el ícono al slot
            slots[index].effect = effect; // Asignar el efecto al slot


            index++;
        }

    }

    private void ListRanged(){
        ItemPanel[] slots = rangedGrid.GetComponentsInChildren<ItemPanel>();
        Debug.Log("Listing ranged effects: " + rangedEffectsInventory.Count);
        int index = 0;
        foreach (var effect in rangedEffectsInventory)
        {
            Debug.Log("Adding to inventory: " + effect.GetType().Name);
            if (index >= slots.Length) break; // Evitar desbordamiento si hay más efectos que espacios

            // Obtener el ícono del efecto

            // Buscar el componente Image en el slot actual

            Debug.Log("Slot " + slots[index].debugName + " setting icon");
            slots[index].icon.gameObject.SetActive(true); // Asegurarse de que el icono esté activo
            slots[index].icon.sprite = effect.getIcon(); // Asignar el ícono al slot
            slots[index].effect = effect; // Asignar el efecto al slot


            index++;
        }

    }

    private void ListDash(){
        ItemPanel[] slots = dashAOE_Grid.GetComponentsInChildren<ItemPanel>();
        Debug.Log("Listing dash effects: " + dash_AOE_EffectsInventory.Count);
        int index = 0;
        foreach (var effect in dash_AOE_EffectsInventory)
        {
            Debug.Log("Adding to inventory: " + effect.GetType().Name);
            if (index >= slots.Length) break; // Evitar desbordamiento si hay más efectos que espacios

            // Obtener el ícono del efecto

            // Buscar el componente Image en el slot actual

            Debug.Log("Slot " + slots[index].debugName + " setting icon");
            slots[index].icon.gameObject.SetActive(true); // Asegurarse de que el icono esté activo
            slots[index].icon.sprite = effect.getIcon(); // Asignar el ícono al slot
            slots[index].effect = effect; // Asignar el efecto al slot


            index++;
        }

    }

    public void DEBUG_ListImpactEffects()
    {
        Debug.Log("Impact Effects Inventory: ");
        foreach (var effect in meleeEffectsInventory)
        {
            Debug.Log(effect.GetType().Name);
        }
    }

    public void SwapEffectOrder(int index1, int index2, int type)
    {
        
        var list = type == 0 ? meleeEffectsInventory :
            type == 1 ? rangedEffectsInventory :
            dash_AOE_EffectsInventory;

        if (index1 >= 0 && index2 >= 0 && index1 < list.Count && index2 < list.Count)
        {
            (list[index1], list[index2]) = (list[index2], list[index1]);
        }
    }

    // Applies the effects starting from the index of the inventory up to the end of the inventory.
    public void ApplyEffects(GameObject target, int index, EnumSpellCardTypes type)
    {
        Debug.Log("Applying effects from index " + index + " to target: " + target.name);
        var list = type == EnumSpellCardTypes.Melee ? meleeEffectsInventory :
        type == EnumSpellCardTypes.Ranged ? rangedEffectsInventory :
        dash_AOE_EffectsInventory;

        if(index < list.Count){
            Debug.Log("Applying effect: " + list[index].GetType().Name + " to target: " + target.name);
            list[index].ApplyEffect(target, index);
        } else {
            Debug.Log("No more effects to Apply (Number of effects: " + list.Count + ")");
        }

    }

}
