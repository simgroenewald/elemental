using UnityEngine;
using System;

[DisallowMultipleComponent]
public class SetActiveAbilityEvent : MonoBehaviour
{
    public event Action<SetActiveAbilityEvent, SetActiveAbilityEventArgs> OnSetActiveAbility;

    public void CallSetActiveAbilityEvent(Ability ability)
    {
        OnSetActiveAbility?.Invoke(this, new SetActiveAbilityEventArgs() { ability = ability });
    }
}

public class SetActiveAbilityEventArgs : EventArgs
{
    public Ability ability;
}
