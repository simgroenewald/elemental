using System;
using UnityEngine;

[RequireComponent(typeof(SetActiveAbilityEvent))]

[DisallowMultipleComponent]
public class ActiveAbility: MonoBehaviour
{
    [SerializeField] private Transform abilityCastPositionLeftTransform;
    [SerializeField] private Transform abilityCastPositionRightTransform;
    [SerializeField] private Transform abilityEffectPositionTransform;

    private SetActiveAbilityEvent setAbilityEvent;
    private Ability currentAbility;

    private void Awake()
    {
        setAbilityEvent = GetComponent<SetActiveAbilityEvent>();
    }

    private void OnEnable()
    {
        setAbilityEvent.OnSetActiveAbility += OnSetActiveAbility;
    }

    private void OnDisable()
    {
        setAbilityEvent.OnSetActiveAbility -= OnSetActiveAbility;
    }
    private void OnSetActiveAbility(SetActiveAbilityEvent setActiveAbilityEvent, SetAbilityEventArgs setActiveAbilityEventArgs)
    {
        SetAbility(setActiveAbilityEventArgs.ability);
    }

    private void SetAbility(Ability ability)
    {
        currentAbility = ability;
        if (ability.abilityDetails.isRanged)
        {
            abilityCastPositionLeftTransform.localPosition = currentAbility.abilityDetails.abilityCastPositionLeft;
            abilityCastPositionRightTransform.localPosition = currentAbility.abilityDetails.abilityCastPositionRight;
        }
    }

    public ProjectileDetailsSO GetCurrentProjectile()
    {
        return currentAbility.abilityDetails.abilityProjectileDetails;
    }

    public Ability GetCurrentAbility()
    {
        return currentAbility;
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

    public Vector3 GetCastEffectPosition()
    {
        return abilityEffectPositionTransform.position;
    }

    public void RemoveCurrentAbility()
    {
        currentAbility = null;
    }
}
