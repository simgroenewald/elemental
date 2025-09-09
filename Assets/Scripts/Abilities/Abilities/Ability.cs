using GLTFast.Schema;
using System;
using System.Diagnostics;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Ability
{
    public AbilityDetailsSO abilityDetails;
    public int abilityListPosition;
    public float abilityCooldownTime;
    public bool isCoolingDown;

    public void UpdatePhysicalDamage(float damage)
    {
        if (!abilityDetails.isMagical)
            abilityDetails._damage = damage;
    }

    public void UpdateMagicalDamage(float damage)
    {
        if (abilityDetails.isMagical)
            abilityDetails._damage = damage;
    }

    public void UpdateRange(float range)
    {
            abilityDetails._range = range;
    }

    public void UpdateProjectileSpeed(float projectileSpeed)
    {
        if (abilityDetails.isRanged)
            abilityDetails._projectileSpeed = projectileSpeed;
    }

    internal void UpdateCritChance(float critChance)
    {
        if (abilityDetails.isBaseAbility)
        {
            abilityDetails.isCritical = true;
            abilityDetails._critChance = critChance;
        }
    }

    internal void UpdateCooldown(float cooldown)
    {
        if (abilityDetails._coolDownTime > 0)
        {
            abilityDetails._coolDownTime = cooldown;
        } else
        {
            abilityDetails._coolDownTime = 0;
        }
    }

    public float EvaluateDamageDealingStats(Character character, float damage)
    {
        damage = ApplyCritDamage(damage, (int)abilityDetails._critChance);

        float modifierAdditinalDamage = 0;
        if (abilityDetails.isBaseAbility)
        {
            foreach (var attackCountModifier in character.stats.attackCountModifierList)
            {
                if (attackCountModifier.counter == attackCountModifier.count)
                {
                    attackCountModifier.counter = 0;
                    if (attackCountModifier.statType == StatType.PhysicalDamageBonus && !abilityDetails.isMagical)
                    {
                        modifierAdditinalDamage = HelperUtilities.GetModifyAmountResult(damage, attackCountModifier.val, attackCountModifier.isPercentage);
                    }
                    else if (attackCountModifier.statType == StatType.MagicalDamageBonus && abilityDetails.isMagical)
                    {
                        modifierAdditinalDamage = HelperUtilities.GetModifyAmountResult(damage, attackCountModifier.val, attackCountModifier.isPercentage);
                    }
                    else if (attackCountModifier.statType == StatType.CritChance)
                    {
                        modifierAdditinalDamage = ApplyCritDamage(damage, (int)attackCountModifier.val);
                    }
                }
                else
                {
                    attackCountModifier.counter += 1;
                }
            }
        }

        return damage + modifierAdditinalDamage;
    }

    private float ApplyCritDamage(float damage, int chance)
    {
        int rndNumber = UnityEngine.Random.Range(1, 101);
        if (rndNumber <= chance)
        {
            int critDamagePercent = UnityEngine.Random.Range(50, 200);
            float critDamage = damage * (critDamagePercent / 100f);
            damage = damage + critDamage;
        }

        return damage;
    }
}


