using System;
using UnityEngine;

public class AbilityEvents : MonoBehaviour
{
    public event Action<bool, TargetDirection, float, float, Vector3> OnAbilitySetup;
    public event Action OnCastAbility;
    public event Action OnAbilityCasted;


    public void RaiseAbilitySetupEvent(
        bool cast,
        TargetDirection direction,
        float aimAngle,
        float abilityAimAngle,
        Vector3 abilityAimDirectionVector)
    {
        OnAbilitySetup?.Invoke(cast, direction, aimAngle, abilityAimAngle, abilityAimDirectionVector);
    }

    public void RaiseCastAbilityEvent()
    {
        OnCastAbility?.Invoke();
    }
    public void RaiseAbilityCastedEvent()
    {
        OnAbilityCasted?.Invoke();
    }
}