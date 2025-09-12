using UnityEngine;
using System;

[DisallowMultipleComponent]
public class AbilityActivationEvents : MonoBehaviour
{
    public event Action<AbilityActivationEvents, AbilityEventArgs> OnSetActiveAbility;
    public event Action<AbilityActivationEvents, AbilityEventArgs> OnStageAbility;
    public event Action OnActivateStagedAbility;

    public void CallSetActiveAbilityEvent(Ability ability)
    {
        OnSetActiveAbility?.Invoke(this, new AbilityEventArgs() { ability = ability });
    }

    public void CallStageAbilityEvent(Ability ability)
    {
        OnStageAbility?.Invoke(this, new AbilityEventArgs() { ability = ability });
    }

    public void CallActivateStagedAbilityEvent()
    {
        OnActivateStagedAbility?.Invoke();
    }

}

public class AbilityEventArgs : EventArgs
{
    public Ability ability;
}
