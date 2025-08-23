using System;
using UnityEngine;

public class AbilityEvents : MonoBehaviour
{
    public event Action OnCastAbility;
    public event Action OnMeleeAttack;
    public event Action OnMeleeEndAttack;
    public event Action OnAbilityCasted;

    public void RaiseCastAbilityEvent()
    {
        OnCastAbility?.Invoke();
    }

    public void RaiseMeleeAttackEvent()
    {
        OnMeleeAttack?.Invoke();
    }

    public void RaiseAbilityCastedEvent()
    {
        OnAbilityCasted?.Invoke();
    }

    public void RaiseMeleeEndAttackEvent()
    {
        OnMeleeEndAttack?.Invoke();
    }
}