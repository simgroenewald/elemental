using UnityEngine;
using System;

[DisallowMultipleComponent]
public class SetActiveAbilityEvent : MonoBehaviour
{
    public event Action<SetActiveAbilityEvent, SetAbilityEventArgs> OnSetActiveAbility;

    public void CallSetActiveAbilityEvent(Ability ability)
    {
        OnSetActiveAbility?.Invoke(this, new SetAbilityEventArgs() { ability = ability });
    }
}

public class SetAbilityEventArgs : EventArgs
{
    public Ability ability;
}
