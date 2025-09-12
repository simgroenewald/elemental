using System;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySetupEvent : MonoBehaviour
{
    public event Action<AbilitySetupEvent, OnAbilitySetupEventArgs> OnSingleMovementAbilitySetup;
    public event Action<AbilitySetupEvent, OnAbilitySetupEventArgs> OnSingleStaticAbilitySetup;
    public event Action<AbilitySetupEvent, OnAbilitySetupEventArgs> OnMultiAbilitySetup;

    public void RaiseSingleMovementAbilitySetupEvent(
        bool cast,
        TargetDirection direction,
        float aimAngle,
        float abilityAimAngle,
        Vector3 abilityAimDirectionVector,
        Character characterCaster,
        Character characterTarget)
    {
        OnSingleMovementAbilitySetup?.Invoke(
            this, new OnAbilitySetupEventArgs()
            {
                cast = cast,
                direction = direction,
                aimAngle = aimAngle,
                abilityAimAngle = abilityAimAngle,
                abilityAimDirectionVector = abilityAimDirectionVector,
                characterCaster = characterCaster,
                characterTarget = characterTarget
            });
    }

    public void RaiseSingleStaticAbilitySetupEvent(
    Character characterCaster,
    Character characterTarget)
    {
        OnSingleStaticAbilitySetup?.Invoke(
            this, new OnAbilitySetupEventArgs()
            {
                characterCaster = characterCaster,
                characterTarget = characterTarget
            });
    }

    public void RaiseMultiAbilitySetupEvent(
        Character characterCaster,
        List<Character> characterTargets)
        {
            OnMultiAbilitySetup?.Invoke(
                this, new OnAbilitySetupEventArgs()
                {
                    characterCaster = characterCaster,
                    characterTargets = characterTargets
                });
        }
}

public class OnAbilitySetupEventArgs : EventArgs
{
    public bool cast;
    public TargetDirection direction;
    public float aimAngle;
    public float abilityAimAngle;
    public Vector3 abilityAimDirectionVector;
    public Character characterCaster;
    public Character characterTarget;
    public List<Character> characterTargets;
}