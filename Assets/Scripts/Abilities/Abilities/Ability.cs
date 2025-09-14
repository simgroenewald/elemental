using GLTFast.Schema;
using System;
using System.Diagnostics;
using System.Security.Policy;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Ability
{
    public AbilityDetailsSO abilityDetails;
    public int abilityListPosition;
    public float abilityCooldownTime;
    public float abilityEffectTime;
    public bool isCoolingDown;
    public bool isEffectTime;
    public bool locked;
    private int counter;

    public void SetCounter()
    {
        counter = 0;
    }

    public void IncrementCounter()
    {
        counter++;
    }

    public bool CanActivate()
    {
        // Check if its targeted and if there is a valid target
        if (abilityDetails.isPassive)
        {
            return false;
        }
        if (locked)
        {
            return false;
        }
        return true;
    }

    public bool CanUse(float currentMana)
    {
        if (isCoolingDown)
        {
            return false;
        }
        if (abilityDetails.manaCost > currentMana)
        {
            return false;
        }
        return true;
    }

    public bool AttemptToUseSuccessful(Character character)
    {
        if (CanUse(character.mana.GetCurrentMana()))
        {
            AbilityTriggered();
            character.manaEvents.RaiseReduceManaEvent(abilityDetails.manaCost);
            return true;
        }
        else
        {
            DisplayManaError();
            return false;
        }
    }

    private void DisplayManaError()
    {
        UnityEngine.Debug.Log("Not Enoug mana");
    }

    public void ResetStates()
    {
        if (abilityDetails.hasEffectTime)
        {
            abilityEffectTime = abilityDetails.effectTime;
            isEffectTime = true;
        }
        if (abilityDetails.hasCountLimit)
        {
            counter = 0;
        }
        if (abilityDetails.hasCooldown)
        {
            abilityCooldownTime = abilityDetails.coolDownTime;
        }
    }

    public void AbilityTriggered()
    {
        if (isEffectTime)
        {
            return;
        }
        if (abilityDetails.hasCountLimit)
        {
            if (counter < abilityDetails.countLimit)
            {
                counter++;
                return;
            }
        }
        if (abilityDetails.hasCooldown)
        {
            isCoolingDown = true;
        }
    }

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


