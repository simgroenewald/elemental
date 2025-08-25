using System;
using UnityEngine;

public class StatModifierEvents : MonoBehaviour
{
    public event Action<float> OnIncreaseMaxHealth;

    public void RaiseIncreaseMaxHealth(float val)
    {
        OnIncreaseMaxHealth?.Invoke(val);
    }
}