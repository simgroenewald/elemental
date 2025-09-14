using GLTFast.Schema;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

[RequireComponent(typeof(AbilityActivationEvents))]

[DisallowMultipleComponent]
public class ActiveAbility: MonoBehaviour
{
    [SerializeField] private Transform abilityCastPositionLeftTransform;
    [SerializeField] private Transform abilityCastPositionRightTransform;
    [SerializeField] private Vector3 castPositionOffset;

    private Character character;
    private StatModifierEvents statModifierEvents;

    private AbilityActivationEvents abilityActivationEvents;
    public Ability currentAbility;
    public Ability stagedAbility;

    private void Awake()
    {
        character = GetComponent<Character>();
        statModifierEvents = character.statModifierEvents;
        abilityActivationEvents = GetComponent<AbilityActivationEvents>();
    }

    private void Start()
    {
        SetCharacterStartAbility();
    }

    private void OnEnable()
    {
        abilityActivationEvents.OnSetActiveAbility += OnSetActiveAbility;
        abilityActivationEvents.OnStageAbility += OnStageAbility;
        abilityActivationEvents.OnActivateStagedAbility += OnActivateStagedAbility;
    }

    private void OnDisable()
    {
        abilityActivationEvents.OnSetActiveAbility -= OnSetActiveAbility;
        abilityActivationEvents.OnStageAbility -= OnStageAbility;
        abilityActivationEvents.OnActivateStagedAbility -= OnActivateStagedAbility;
    }
    private void OnSetActiveAbility(AbilityActivationEvents abilityActivationEvent, AbilityEventArgs abilityEventArgs)
    {
        SetAbility(abilityEventArgs.ability);
    }

    private void OnStageAbility(AbilityActivationEvents abilityActivationEvent, AbilityEventArgs abilityEventArgs)
    {
        StageAbility(abilityEventArgs.ability);
    }

    private void OnActivateStagedAbility()
    {
        SetAbility(stagedAbility);
    }

    private void SetAbility(Ability ability)
    {
        if (ability.CanActivate())
        {
            if (currentAbility != ability)
            {
                ability.ResetStates();
            }
            {
                RemoveModifiers();
                currentAbility = ability;
                if (ability.abilityDetails.isRanged)
                {
                    abilityCastPositionLeftTransform.localPosition = currentAbility.abilityDetails.abilityCastPositionLeft;
                    abilityCastPositionRightTransform.localPosition = currentAbility.abilityDetails.abilityCastPositionRight;
                    castPositionOffset = currentAbility.abilityDetails.castPositionOffset;
                }
                ApplyModifiers();
            }
        }
    }

    private void RemoveModifiers()
    {
        if (currentAbility != null && currentAbility.abilityDetails.modifierData != null)
        {
            foreach (var modifier in currentAbility.abilityDetails.modifierData)
            {
                statModifierEvents.RaiseRemoveBasicStatEvent(modifier.statType, modifier.value, modifier.isPercentage);
            }
        }
    }

    private void ApplyModifiers()
    {
        if (currentAbility.abilityDetails.modifierData.Count > 0)
        {
            foreach (var modifier in currentAbility.abilityDetails.modifierData)
            {
                statModifierEvents.RaiseAddBasicStatEvent(modifier.statType, modifier.value, modifier.isPercentage);
            }
        }
        if (currentAbility.abilityDetails.instantModifierData.Count > 0)
        {
            foreach (var modifier in currentAbility.abilityDetails.instantModifierData)
            {
                statModifierEvents.RaiseConsumableUsedEvent(modifier.statType, modifier.value, modifier.isPercentage);
            }
        }
    }

    private void SetCharacterStartAbility()
    {
        foreach (Ability ability in character.abilityList)
        {
            if (ability.abilityDetails == character.characterDetails.startingAbility)
            {
                SetAbility(ability);
                break;
            }
        }
    }

    private void StageAbility(Ability ability)
    {
        stagedAbility = ability;
    }

    public ProjectileDetailsSO GetCurrentProjectile()
    {
        return currentAbility.abilityDetails.abilityProjectileDetails;
    }

    public Ability GetCurrentAbility()
    {
        return currentAbility;
    }

    public Ability GetStagedAbility()
    {
        return stagedAbility;
    }

    public Vector3 GetCastPosition(TargetDirection direction)
    {
        if (direction == TargetDirection.Left)
        {
            return abilityCastPositionLeftTransform.position;
        } else
        {
            return abilityCastPositionRightTransform.position;
        }
    }

    public Vector3 GetCastPosition(Vector3 position)
    {
        position = position + castPositionOffset;
        return position;
    }

    public void RemoveCurrentAbility()
    {
        currentAbility = null;
    }
}
