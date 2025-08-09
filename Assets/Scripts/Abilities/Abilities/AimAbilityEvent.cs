using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[DisallowMultipleComponent]
public class AimAbilityEvent : MonoBehaviour
{
    public event Action<AimAbilityEvent, AimAbilityEventArgs> OnAbilityAim;

    public void CallAimAbilityEvent(TargetDirection direction, float aimAngle, float abilityAimAngle, Vector3 abilityAimDirectionVector)
    {
        OnAbilityAim?.Invoke(this, new AimAbilityEventArgs() { direction = direction, aimAngle = aimAngle, abilityAimAngle = abilityAimAngle, abilityAimDirectionVector = abilityAimDirectionVector });
    }

}


public class AimAbilityEventArgs : EventArgs
{
    public TargetDirection direction;
    public float aimAngle;
    public float abilityAimAngle;
    public Vector3 abilityAimDirectionVector;
}