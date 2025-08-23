using System;
using UnityEngine;

public class HealthEvents : MonoBehaviour
{
    public event Action<float> OnReduceHealth;
    public event Action<float> OnIncreaseHealth;
    public event Action<float> OnUpdateHealthbar;
    public event Action<float> OnUpdateHealthbarMax;

    public void RaiseReduceHealthEvent(float damage)
    {
        OnReduceHealth?.Invoke(damage);
    }

    public void RaiseIncreaseHealthEvent(float health)
    {
        OnIncreaseHealth?.Invoke(health);
    }

    public void RaiseUpdateHealthbar(float currentHealth)
    {
        OnUpdateHealthbar?.Invoke(currentHealth);
    }

    public void RaiseUpdateHealthbarMax(float maxHealth)
    {
        OnUpdateHealthbarMax?.Invoke(maxHealth);
    }
}