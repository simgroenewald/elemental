using System;
using UnityEngine;
using UnityEngine.TextCore.Text;

[RequireComponent(typeof(StatModifierEvents))]
[RequireComponent(typeof(Health))]
public class CharacterStatsModifier : MonoBehaviour
{

    private StatModifierEvents statModiferEvents;
    private Health health;

    private void Awake()
    {
        statModiferEvents = GetComponent<StatModifierEvents>();
        health = GetComponent<Health>();
    }

    private void Start()
    {
        statModiferEvents.OnIncreaseMaxHealth += HandleOnIncreaseMaxHealth;
    }

    private void HandleOnIncreaseMaxHealth(float val)
    {
        float maxHealth = health.GetMaxHealth();
        float currenthealth = health.GetCurrentHealth();
        maxHealth = maxHealth + (maxHealth * (val / 100));
        currenthealth = currenthealth + (currenthealth * (val / 100));

        health.SetHealth(maxHealth, currenthealth);
    }
}
