using UnityEngine;

public class Ability
{
    public AbilityDetailsSO abilityDetails;
    public int abilityListPosition;
    public float abilityCooldownTime;
    public bool isCoolingDown;

    private void UpdateDamage(float damage)
    {
        if (abilityDetails.isBaseAbility)
        { 
            if (abilityDetails.isRanged)
                abilityDetails.abilityProjectileDetails.projectileDamage = abilityDetails.abilityProjectileDetails.projectileDamage + damage;
            else
                abilityDetails.physicalDamage = abilityDetails.physicalDamage + damage;
        }
    }

    private void UpdateRange(float range)
    {
        if (abilityDetails.isBaseAbility)
        {
            if (abilityDetails.isRanged)
                abilityDetails.abilityProjectileDetails.projectileDamage = abilityDetails.abilityProjectileDetails.projectileRange + range;
            else
                abilityDetails.physicalDamage = abilityDetails.range + range;
        }
    }
}


