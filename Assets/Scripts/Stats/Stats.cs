using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.TextCore.Text;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using System.Data;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor.SpeedTree.Importer;
using UnityEditor.Playables;

public class Stats : MonoBehaviour
{
    private Character character;
    [SerializeField] private Dictionary<StatType, float> originalStatsDict = new Dictionary<StatType, float>();
    [SerializeField] private Dictionary<StatType, float> statsDict = new Dictionary<StatType, float>();
    [SerializeField] private Dictionary<StatType, float> displayStatsDict = new Dictionary<StatType, float>();
    public List<HealthLevelModifier> healthLevelModifierList = new List<HealthLevelModifier>();
    public List<AttackCountModifier> attackCountModifierList = new List<AttackCountModifier>();
    internal Action<StatType, float> OnStatUpdated;
    Mana mana;
    Health health;

    private Dictionary<StatType, Dictionary<string, List<float>>> statModifierData = new Dictionary<StatType, Dictionary<string, List<float>>>();

    private void Awake()
    {
        character = GetComponent<Character>();
        health = character.health;
        mana = character.mana;
    }
    private void OnEnable()
    {
        character.statModifierEvents.OnAddBasicStatEvent += (statType, stat, isPercentage) =>{ModifybasicStat(statType, stat, isPercentage, true);};
        character.statModifierEvents.OnRemoveBasicStatEvent += (statType, stat, isPercentage) => { ModifybasicStat(statType, stat, isPercentage, false); };
        character.statModifierEvents.OnAddHealthLevelItemEvent += AddHealthLevelModifier;
        character.statModifierEvents.OnRemoveHealthLevelItemEvent += RemoveHealthLevelModifier;
        character.statModifierEvents.OnAddAttackCountItemEvent += AddAttackCountModifier;
        character.statModifierEvents.OnRemoveAttackCountItemEvent += RemoveAttackCountModifier;
        character.statModifierEvents.OnConsumableItemUsedEvent += ApplyConsumableEffects;
    }

    private void OnDisable()
    {
        character.statModifierEvents.OnAddBasicStatEvent -= (statType, stat, isPercentage) => { ModifybasicStat(statType, stat, isPercentage, true); };
        character.statModifierEvents.OnRemoveBasicStatEvent -= (statType, stat, isPercentage) => { ModifybasicStat(statType, stat, isPercentage, false); };
        character.statModifierEvents.OnAddHealthLevelItemEvent -= AddHealthLevelModifier;
        character.statModifierEvents.OnRemoveHealthLevelItemEvent -= RemoveHealthLevelModifier;
        character.statModifierEvents.OnRemoveAttackCountItemEvent -= RemoveAttackCountModifier;
        character.statModifierEvents.OnConsumableItemUsedEvent -= ApplyConsumableEffects;
    }


    public void Initialise(StatsSO statsSO)
    {

        foreach (StatType stat in Enum.GetValues(typeof(StatType)))
        {
            originalStatsDict[stat] = 0;
            statsDict[stat] = 0;

            Dictionary<string, List<float>> modifierDict = new Dictionary<string, List<float>>();

            modifierDict["flat"] = new List<float>();
            modifierDict["percentage"] = new List<float>();

            statModifierData[stat] = modifierDict;
        }

        foreach (var stat in statsSO.stats)
        {
            displayStatsDict[stat.type] = stat.value;
            originalStatsDict[stat.type] = stat.value;
            statsDict[stat.type] = stat.value;
        }
    }

    public float GetStat(StatType statType)
    {
        if (statsDict.ContainsKey(statType))
            return statsDict[statType];

        return 0;
    }

    public float GetOriginalStat(StatType statType)
    {
        if (originalStatsDict.ContainsKey(statType))
            return originalStatsDict[statType];

        return 0;
    }

    public void SetStat(StatType statType, float value)
    {
        if (statsDict.ContainsKey(statType))
            statsDict[statType] = value;
    }

    public void AddHealthLevelModifier(string name, float trigger, float duration, float increase, bool isPercentage)
    {
        HealthLevelModifier healthLevelModifier = new HealthLevelModifier();
        healthLevelModifier.name = name;
        healthLevelModifier.trigger = trigger;
        healthLevelModifier.duration = duration;
        healthLevelModifier.increase = increase;
        healthLevelModifier.isPercentage = isPercentage;
        healthLevelModifierList.Add(healthLevelModifier);
    }

    public void RemoveHealthLevelModifier(string name)
    {
        foreach (var healthLevelModifier in healthLevelModifierList)
        {
            if (healthLevelModifier.name == name) {
                healthLevelModifierList.Remove(healthLevelModifier);
                return;
            }
        }
    }

    public void AddAttackCountModifier(StatType statType, string name, float count, float duration, float val, bool isPercentage)
    {
        AttackCountModifier attackCountModifier = new AttackCountModifier();
        attackCountModifier.statType = statType;
        attackCountModifier.name = name;
        attackCountModifier.count = count;
        attackCountModifier.duration = duration;
        attackCountModifier.val = val;
        attackCountModifier.isPercentage = isPercentage;
        attackCountModifier.counter = 0;
        attackCountModifierList.Add(attackCountModifier);
    }

    public void RemoveAttackCountModifier(string name)
    {
        foreach (var attackCountModifier in attackCountModifierList)
        {
            if (attackCountModifier.name == name)
            {
                attackCountModifierList.Remove(attackCountModifier);
                return;
            }
        }
    }

    private void ModifybasicStat(StatType statType, float val, bool isPercentage, bool isAdding)
    {
        if (statType == StatType.Health)
        {
            if (isAdding) AddModifiers(statType, val, isPercentage);
            else RemoveModifiers(statType, val, isPercentage);

            // First get original values
            float currentMaxHealth = GetStat(StatType.Health);
            float currentHealth = health.GetCurrentHealth();
            float percentageOfMax = currentHealth / currentMaxHealth;

            // Update Max
            UpdateStat(statType);

            // Set new current health
            health.UpdateHealth(percentageOfMax);
        }
        if (statType == StatType.Mana)
        {
            if (isAdding) AddModifiers(statType, val, isPercentage);
            else RemoveModifiers(statType, val, isPercentage);

            // First get original values
            float currentMaxMana = GetStat(StatType.Mana);
            float currentMana= mana.GetCurrentMana();
            float percentageOfMax = currentMana / currentMaxMana;

            // Update Max
            UpdateStat(statType);

            // Set new current mana
            mana.UpdateMana(percentageOfMax);
        }
        if (statType == StatType.HealthRegen)
        {
            if (isAdding) AddModifiers(statType, val, isPercentage);
            else RemoveModifiers(statType, val, isPercentage);
            UpdateStat(statType);
        }
        if (statType == StatType.ManaRegen)
        {
            if (isAdding) AddModifiers(statType, val, isPercentage);
            else RemoveModifiers(statType, val, isPercentage);
            UpdateStat(statType);
        }
        if (statType == StatType.PhysicalDamageBonus)
        {
            if (isAdding) AddModifiers(statType, val, isPercentage);
            else RemoveModifiers(statType, val, isPercentage);
            UpdatePercentageStat(statType);
            // Apply modifiers to all phyical abilities
            foreach (Ability ability in character.abilityList)
            {
                if (!ability.abilityDetails.isMagical)
                {
                    float newAbilityPhysicalDamage = GetTotalModifiedValue(StatType.PhysicalDamageBonus, ability.abilityDetails.damage);
                    ability.UpdatePhysicalDamage(newAbilityPhysicalDamage);
                }
            }
        }
        if (statType == StatType.MagicalDamageBonus)
        {
            if (isAdding) AddModifiers(statType, val, isPercentage);
            else RemoveModifiers(statType, val, isPercentage);
            UpdatePercentageStat(statType);
            // Apply modifiers to all magical abilities
            foreach (Ability ability in character.abilityList)
            {
                if (ability.abilityDetails.isMagical)
                {
                    float newAbilityMagicalDamage = GetTotalModifiedValue(StatType.MagicalDamageBonus, ability.abilityDetails.damage);
                    ability.UpdateMagicalDamage(newAbilityMagicalDamage);
                }
            }
        }
        if (statType == StatType.RangeBonus)
        {
            if (isAdding) AddModifiers(statType, val, isPercentage);
            else RemoveModifiers(statType, val, isPercentage);
            UpdatePercentageStat(statType);
            foreach (Ability ability in character.abilityList)
            {
                float newAbilityRange = GetTotalModifiedValue(StatType.RangeBonus, ability.abilityDetails.range);
                ability.UpdateRange(newAbilityRange);
            }
        }
        if (statType == StatType.ProjectileSpeed)
        {
            if (isAdding) AddModifiers(statType, val, isPercentage);
            else RemoveModifiers(statType, val, isPercentage);
            if (character.baseAbility.abilityDetails.isRanged)
            {
                float newAbilityProjectileSpeed = GetTotalModifiedValue(StatType.ProjectileSpeed, character.baseAbility.abilityDetails.projectileSpeed);
                character.baseAbility.UpdateProjectileSpeed(newAbilityProjectileSpeed);
            }
        }
        if (statType == StatType.AttackSpeed)
        {
            if (isAdding) AddModifiers(statType, val, isPercentage);
            else RemoveModifiers(statType, val, isPercentage);
            UpdateStat(statType);
            character.characterCombat.SetPlayerAttackAnimationSpeed();
        }
        if (statType == StatType.MovementSpeed)
        {
            if (isAdding) AddModifiers(statType, val, isPercentage);
            else RemoveModifiers(statType, val, isPercentage);
            UpdateStat(statType);
            character.characterMovement.SetMovementSpeed();
        }
        if (statType == StatType.Armor)
        {
            if (isAdding) AddModifiers(statType, val, isPercentage);
            else RemoveModifiers(statType, val, isPercentage);
            UpdateStat(statType);
        }
        // Percentage Stats - Deminishing
        if (statType == StatType.CritChance)
        {
            if (isAdding) AddModifiers(statType, val, isPercentage);
            else RemoveModifiers(statType, val, isPercentage);
            UpdatePercentageChanceStat(statType);
            character.baseAbility.UpdateCritChance(GetStat(StatType.CritChance));
        }
        if (statType == StatType.Evasion)
        {
            if (isAdding) AddModifiers(statType, val, isPercentage);
            else RemoveModifiers(statType, val, isPercentage);
            UpdatePercentageChanceStat(statType);
        }
        if (statType == StatType.HealthSteal)
        {
            if (isAdding) AddModifiers(statType, val, isPercentage);
            else RemoveModifiers(statType, val, isPercentage);
            UpdatePercentageChanceStat(statType);
        }
        if (statType == StatType.Cooldown)
        {
            if (isAdding) AddModifiers(statType, val, isPercentage);
            else RemoveModifiers(statType, val, isPercentage);
            float newAbilityCooldown = 0;
            foreach (Ability ability in character.abilityList)
            {
                if (ability.abilityDetails.coolDownTime > 0f)
                {
                    newAbilityCooldown = GetTotalModifiedValue(StatType.Cooldown, ability.abilityDetails.coolDownTime);
                    ability.UpdateCooldown(newAbilityCooldown);
                }
            }
        }
        if (displayStatsDict.ContainsKey(statType))
            OnStatUpdated?.Invoke(statType, statsDict[statType]);
    }

    private void AddModifiers(StatType stat, float newAmount, bool isPercentage)
    {
        if (isPercentage) {
            statModifierData[stat]["percentage"].Add(newAmount / 100f);
        }
        else {
            statModifierData[stat]["flat"].Add(newAmount);
        }
    }

    private void RemoveModifiers(StatType stat, float removeAmount, bool isPercentage)
    {
        if (isPercentage)
        {
            foreach (float value in statModifierData[stat]["percentage"])
            {
                if (Mathf.Approximately(value, removeAmount / 100f))
                {
                    statModifierData[stat]["percentage"].Remove(value);
                    return;
                }
            }
        }
        else
        {
            foreach (var value in statModifierData[stat]["flat"])
            {
                if (value == removeAmount)
                {
                    statModifierData[stat]["flat"].Remove(value);
                    return;
                }
            }
        }
    }

    private void UpdateStat(StatType stat)
    {
        float newTotalValue = GetTotalModifiedValue(stat, GetOriginalStat(stat));

        SetStat(stat, newTotalValue);
    }

    private void UpdatePercentageChanceStat(StatType stat)
    {
        if (statModifierData[stat]["percentage"].Count > 0)
        {
            float newTotal = (1 - statModifierData[stat]["percentage"][0]);
            for (int i = 1; i < statModifierData[stat]["percentage"].Count; i++)
            {
                newTotal *= (1 - statModifierData[stat]["percentage"][i]);
            }

            SetStat(stat, (1 - newTotal) * 100f);
        } else
        {
            SetStat(stat, 0);
        }

    }

    private void UpdatePercentageStat(StatType stat)
    {
        float totalPercentage = 0;
        if (statModifierData[stat]["percentage"].Count > 0)
        {
            foreach(var value in statModifierData[stat]["percentage"])
            {
                totalPercentage += value;
            }

            SetStat(stat, totalPercentage);
        }
        else
        {
            SetStat(stat, 0);
        }
    }

    public float GetTotalModifiedValue(StatType stat, float originalValue)
    {
        List<float> percentageModifiers = statModifierData[stat]["percentage"];
        List<float> flatModifiers = statModifierData[stat]["flat"];

        float totalPercentageModifier = 0f;
        float totalFlatModifier = 0f;

        foreach (float percentageModifier in percentageModifiers)
        {
            totalPercentageModifier += percentageModifier;
        }
        foreach (float flatModifier in flatModifiers)
        {
            totalFlatModifier += flatModifier;
        }

        float newTotalValue = (originalValue + totalFlatModifier) + ((originalValue + totalFlatModifier) * (totalPercentageModifier));

        return newTotalValue;
    }

    private void ApplyConsumableEffects(StatType statType, float val, bool isPercentage)
    {
        if (statType == StatType.Health)
        {
            if (isPercentage)
            {
                float additionalHealth = GetStat(StatType.Health) * val / 100;
                character.healthEvents.RaiseIncreaseHealthEvent(additionalHealth);
            }
            else
            {
                character.healthEvents.RaiseIncreaseHealthEvent(val);
            }
        }
        if (statType == StatType.Mana)
        {
            if (isPercentage)
            {
                float additionalMana = GetStat(StatType.Mana) * val / 100;
                character.manaEvents.RaiseIncreaseManaEvent(additionalMana);
            }
            else
            {
                character.manaEvents.RaiseIncreaseManaEvent(val);
            }
        }
    }

    public float EvaluateDamageTakingStats(float damage)
    {
        float evasion = GetStat(StatType.Evasion);
        float armor = GetStat(StatType.Armor);

        // Check if attack is evaded
        int rndNumber = UnityEngine.Random.Range(1, 101);
        if (rndNumber >= (int)evasion)
        {
            if (armor > 0f)
            {
                // Apply armor
                float damageReduction = (0.052f * armor) / (0.9f + 0.048f * Math.Abs(armor));
                damage = damage * (1f - damageReduction);
            }
            return damage;
        }

        return 0f;
    }

    public Dictionary<StatType, float> GetAllDisplayStats()
    {
        return displayStatsDict;
    }

}

public class HealthLevelModifier
{
    public string name;
    public float trigger;
    public float duration;
    public float increase;
    public bool isPercentage;
    public bool isActive;
}

public class AttackCountModifier
{
    public StatType statType;
    public string name;
    public float count;
    public float duration;
    public float val;
    public bool isPercentage;
    public int counter;
}
