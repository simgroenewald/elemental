using System;
using UnityEngine;

public class ManaEvents : MonoBehaviour
{
    public event Action<float> OnReduceMana;
    public event Action<float> OnIncreaseMana;
    public event Action<float> OnUpdateManabar;
    public event Action<float> OnUpdateManabarMax;

    public void RaiseReduceManaEvent(float manaUsed)
    {
        OnReduceMana?.Invoke(manaUsed);
    }

    public void RaiseIncreaseManaEvent(float mana)
    {
        OnIncreaseMana?.Invoke(mana);
    }

    public void RaiseUpdateManabar(float currentMana)
    {
        OnUpdateManabar?.Invoke(currentMana);
    }

    public void RaiseUpdateManabarMax(float maxMana)
    {
        OnUpdateManabarMax?.Invoke(maxMana);
    }
}