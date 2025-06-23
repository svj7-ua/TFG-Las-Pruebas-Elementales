using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopController : MonoBehaviour
{

    [SerializeField] private GameObject meleeSpellcard;
    [SerializeField] private GameObject rangedSpellcard;
    [SerializeField] private GameObject dashSpellcard;

    // Start is called before the first frame update
    void Start()
    {

        meleeSpellcard.GetComponent<SpellCard>().SetSpellCard(GetRandomSpellCard());
        meleeSpellcard.GetComponent<SpellCard>().SetSpellCardType(EnumSpellCardTypes.Melee);
        meleeSpellcard.GetComponent<SpellCard>().SetItemInShop(gameObject);

        rangedSpellcard.GetComponent<SpellCard>().SetSpellCard(GetRandomSpellCard());
        rangedSpellcard.GetComponent<SpellCard>().SetSpellCardType(EnumSpellCardTypes.Ranged);
        rangedSpellcard.GetComponent<SpellCard>().SetItemInShop(gameObject);

        dashSpellcard.GetComponent<SpellCard>().SetSpellCard(GetRandomSpellCard());
        dashSpellcard.GetComponent<SpellCard>().SetSpellCardType(EnumSpellCardTypes.Dash_AOE);
        dashSpellcard.GetComponent<SpellCard>().SetItemInShop(gameObject);

    }

    private EnumSpellCards GetRandomSpellCard()
    {
        // Get a random spell card from the EnumSpellCards enum
        System.Array values = System.Enum.GetValues(typeof(EnumSpellCards));
        return (EnumSpellCards)values.GetValue(Random.Range(1, values.Length)); // Value 0 is reserved for "None"
    }

    public void RemoveAllItemsFromShop()
    {
        if(meleeSpellcard!=null) Destroy(meleeSpellcard);
        if(rangedSpellcard!=null) Destroy(rangedSpellcard);
        if(dashSpellcard!=null) Destroy(dashSpellcard);
    }

}
