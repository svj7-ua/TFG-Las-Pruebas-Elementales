using System.Collections.Generic;
using UnityEngine;

public class UnlockeableRunes : MonoBehaviour
{

    [SerializeField] private List<GameObject> unlockeableRunes_OriginalList;

    [SerializeField] private List<GameObject> unlockeableRunes_ShopList;

    private void Awake()
    {
        
        foreach (GameObject rune in unlockeableRunes_OriginalList)
        {
            if (!GameData.unlockedRunes[(int)rune.GetComponent<IItem>().GetRune()])
            {
                unlockeableRunes_ShopList.Add(rune);
            }
        }

    }


}