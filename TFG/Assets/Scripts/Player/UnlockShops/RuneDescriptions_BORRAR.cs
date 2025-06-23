public static class RuneDescriptions
{
    public static string GetRuneDescription(EnumRunes rune)
    {
        switch (rune)
        {
            case EnumRunes.FireOrb_Rune:
                return "Gransts an orbiting Fire orb that deals Fire damage to enemies on impact, with 30% chance to apply Dash Spell Card effects. Applies the burning effect.";
            case EnumRunes.LightningOrb_Rune:
                return "Gransts an orbiting Lightning orb that deals Lightning damage to enemies on impact, with 30% chance to apply Dash Spell Card effects. Applies the electrified effect.";
            case EnumRunes.ArcaneOrb_Rune:
                return "Gransts an orbiting Arcane orb that deals Arcane damage to enemies on impact, with 30% chance to apply Dash Spell Card effects. Applies the cursed effect.";
            case EnumRunes.PoisonOrb_Rune:
                return "Gransts an orbiting Poison orb that deals Poison damage to enemies on impact, with 30% chance to apply Dash Spell Card effects. Applies the poisoned effect.";
            case EnumRunes.FireElementalAdept_Rune:
                return "Grants the ability to ignore Fire resistance.";
            case EnumRunes.LightningElementalAdept_Rune:
                return "Grants the ability to ignore Lightning resistance.";
            case EnumRunes.ArcaneElementalAdept_Rune:
                return "Grants the ability to ignore Arcane resistance.";
            case EnumRunes.PoisonElementalAdept_Rune:
                return "Grants the ability to ignore Poison resistance.";
            case EnumRunes.WindElementalAdept_Rune:
                return "Grants the ability to ignore Wind resistance.";
            case EnumRunes.FireElementalMasterAdept_Rune:
                return "Grants the ability to ignore Fire resistance and immunity.";
            case EnumRunes.LightningElementalMasterAdept_Rune:
                return "Grants the ability to ignore Lightning resistance and immunity.";
            case EnumRunes.ArcaneElementalMasterAdept_Rune:
                return "Grants the ability to ignore Arcane resistance and immunity.";
            case EnumRunes.PoisonElementalMasterAdept_Rune:
                return "Grants the ability to ignore Poison resistance and immunity.";
            case EnumRunes.WindElementalMasterAdept_Rune:
                return "Grants the ability to ignore Wind resistance and immunity.";
            case EnumRunes.FireElementalResistance_Rune:
                return "Grants resistance against Fire damage.";
            case EnumRunes.LightningElementalResistance_Rune:
                return "Grants resistance against Lightning damage.";
            case EnumRunes.ArcaneElementalResistance_Rune:
                return "Grants resistance against Arcane damage.";
            case EnumRunes.PoisonElementalResistance_Rune:
                return "Grants resistance against Poison damage.";
            case EnumRunes.WindElementalResistance_Rune:
                return "Grants resistance against fire damage.";
            case EnumRunes.FireSlash_Rune:
                return "Fire Slash: A fiery attack that scorches your enemies with each strike.";
            case EnumRunes.LightningSlash_Rune:
                return "Lightning Slash: A swift strike that delivers a jolt of electricity to your foes.";
            case EnumRunes.ArcaneSlash_Rune:
                return "Arcane Slash: A mystical slash that channels arcane energy to cut through enemies.";
            case EnumRunes.PoisonSlash_Rune:
                return "Poison Slash: A deadly strike that poisons your enemies, causing lingering damage.";
            case EnumRunes.WindSlash_Rune:
                return "Wind Slash: A swift and agile attack that slices through the air, striking with precision.";
            case EnumRunes.ThirstingBlade_Rune:
                return "Thirsting Blade: A blade that craves the life force of your enemies, restoring your health with each kill.";
            case EnumRunes.BloodPact_Rune:
                return "Blood Pact: A dark pact that enhances your power at the cost of your own health.";
            case EnumRunes.LifeOrbHealingIncrease_Rune:
                return "Life Orb Healing Increase: Increases the healing effects of Life Orbs, restoring more health.";
            case EnumRunes.LifeOrbDropIncrease_Rune:
                return "Life Orb Drop Increase: Boosts the drop rate of Life Orbs, ensuring you have more healing resources.";
            case EnumRunes.LifeIncrease_Rune:
                return "Life Increase: Enhances your maximum health, allowing you to endure more damage.";
            case EnumRunes.GlassCanon_Rune:
                return "Glass Canon: Increases your damage output significantly but reduces your defense, making you more vulnerable.";
            case EnumRunes.SoulEater_Rune:
                return "Soul Eater: A rune that allows you to consume the souls of your enemies, granting you power and health.";
            case EnumRunes.Vampirism_Rune:
                return "Vampirism: A dark power that allows you to drain health from your enemies with each attack.";
            case EnumRunes.LifeOrbHealthIncrease_Rune:
                return "Life Orb Health Increase: Enhances the health restoration from Life Orbs, making them more effective.";
            case EnumRunes.SoulVampirism_Rune:
                return "Soul Vampirism: A powerful rune that allows you to drain the life force of your enemies, restoring your own health.";
            default:
                return "Unknown Rune: This rune's power is yet to be discovered.";
        }
    }
}