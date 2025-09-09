using System;
using UnityEngine;

public class AbilitySetupEvent : MonoBehaviour
{
    public event Action<AbilitySetupEvent, OnAbilitySetupEventArgs> OnAbilitySetup;

    public void RaiseAbilitySetupEvent(
        bool cast,
        TargetDirection direction,
        float aimAngle,
        float abilityAimAngle,
        Vector3 abilityAimDirectionVector,
        Character characterCaster,
        Character characterTarget)
    {
        OnAbilitySetup?.Invoke(
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
}