using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEffect
{
    void ApplyEffect(GameObject target, int index = 0);
    Sprite getIcon(); // Returns the icon of the effect

    void SetSpellCardType(EnumSpellCardTypes type); // Set the spell card type to apply the effects from the active effects inventory

    EnumSpellCards getSpellCard();
    EnumSpellCardTypes getSpellCardType();

    string getDescription(); // Get the description of the effect

    void UpgradeEffect(); // Upgrade the effect
}
