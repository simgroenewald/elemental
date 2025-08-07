using System;
using UnityEngine;

[RequireComponent(typeof(SetActiveAbilityEvent))]

[DisallowMultipleComponent]
public class ActiveAbility: MonoBehaviour
{
    [SerializeField] private Transform abilityCastPositionTransform;
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
    private void OnSetActiveAbility(SetActiveAbilityEvent setActiveWeaponEvent, SetActiveAbilityEventArgs setActiveAbilityEventArgs)
    {
        SetAbility(setActiveAbilityEventArgs.ability);
    }

    private void SetAbility(Ability ability)
    {
        currentAbility = ability;

        abilityCastPositionTransform.localPosition = currentAbility.abilityDetails.abilityCastPosition;
    }

    public ProjectileDetailsSO GetCurrentProjectile()
    {
        return currentAbility.abilityDetails.abilityProjectileDetails;
    }

    public Ability GetCurrentAbility()
    {
        return currentAbility;
    }

    public Vector3 GetCastPosition()
    {
        return abilityCastPositionTransform.position;
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
