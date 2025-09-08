using GLTFast.Schema;
using System;
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

    public float EvaluateDamageDealingStats(float damage)
    {
        int rndNumber = UnityEngine.Random.Range(1, 101);
        if (rndNumber <= (int)abilityDetails._critChance)
        {
            int critDamagePercent = UnityEngine.Random.Range(50, 200);
            float critDamage = damage * (critDamagePercent / 100);
            return damage + critDamage;
        }
        return damage;
    }
}


