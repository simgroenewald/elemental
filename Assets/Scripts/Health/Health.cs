using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Rendering;
using static Unity.Cinemachine.CinemachineFreeLookModifier;

[DisallowMultipleComponent]
public class Health: MonoBehaviour
{
    private Coroutine healthRegenCoroutine;
    private float tickSeconds = 0.1f;
    private Character character;
    private float currentHealth;
    private float regenBucket;
    private float flatBonusHealthRegen;
    private float percentageBonusHealthRegen;
    [SerializeField] private GameObject fillArea;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    private void OnEnable()
    {
        character.healthEvents.OnReduceHealth += OnReduceHealth;
        character.healthEvents.OnIncreaseHealth += OnIncreaseHealth;
        healthRegenCoroutine = StartCoroutine(RegenLoop());
    }

    private void OnDisable()
    {
        character.healthEvents.OnReduceHealth -= OnReduceHealth;
        character.healthEvents.OnIncreaseHealth -= OnIncreaseHealth;
        if (healthRegenCoroutine != null) StopCoroutine(healthRegenCoroutine);
    }

/*    private void Update()
    {
        if (character.stats.GetStat(StatType.HealthRegen) != 0 && Time.frameCount % character.stats.GetStat(StatType.HealthRegen) == 0)
        {
            if (currentHealth < character.stats.GetStat(StatType.Health))
            {
                OnIncreaseHealth(1);
            }
        }

        if (currentHealth >= character.stats.GetStat(StatType.Health) || character.stats.GetStat(StatType.HealthRegen) == 0) return;

        regenBucket += character.stats.GetStat(StatType.HealthRegen) * Time.deltaTime;
        int whole = Mathf.FloorToInt(regenBucket);
        if (whole > 0)
        {
            OnIncreaseHealth(whole);
            regenBucket -= whole;
        }
    }*/

    private IEnumerator RegenLoop()
    {
        var wait = new WaitForSeconds(tickSeconds);

        while (true)
        {
            float maxHealth = character.stats.GetStat(StatType.Health);
            float healthRegen = character.stats.GetStat(StatType.HealthRegen);

            if (healthRegen > 0)
            {
                // --- Check modifiers dictionary each tick ---
                if (character.stats.healthLevelModifierList.Count > 0)
                {
                    foreach (HealthLevelModifier modifier in character.stats.healthLevelModifierList)
                    {
                        // Trigger if not already running
                        if (!modifier.isActive)
                        {
                            float currentHealthPercent = currentHealth / Mathf.Max(1f, maxHealth) * 100;
                            if (currentHealthPercent <= modifier.trigger)
                            {
                                StartCoroutine(ApplyModifier(modifier));
                                modifier.isActive = true; // mark as active so we don’t double-apply
                            }
                        }
                    }
                }

                float healthRegenTotal = healthRegen * (1f + percentageBonusHealthRegen) + flatBonusHealthRegen;

                if (healthRegenTotal > 0f && currentHealth < maxHealth)
                {
                    regenBucket += healthRegenTotal * tickSeconds;

                    int whole = Mathf.FloorToInt(regenBucket);
                    if (whole > 0)
                    {
                        int amountToHeal = Mathf.Min(whole, Mathf.CeilToInt(maxHealth - currentHealth));
                        if (amountToHeal > 0)
                        {
                            OnIncreaseHealth(amountToHeal);
                            regenBucket -= amountToHeal;
                        }
                        else
                        {
                            regenBucket = 0f;
                        }
                    }
                }
            }

            yield return wait;
        }
    }

    private IEnumerator ApplyModifier(HealthLevelModifier modifier)
    {
        if (modifier.isPercentage) percentageBonusHealthRegen += modifier.increase;
        else flatBonusHealthRegen += modifier.increase;

        yield return new WaitForSeconds(modifier.duration);

        if (modifier.isActive)
        {
            if (modifier.isPercentage) percentageBonusHealthRegen -= modifier.increase;
            else flatBonusHealthRegen -= modifier.increase;

            modifier.isActive = false; // allow it to be reapplied later if needed
        }
    }


    public void SetHealth()
    {
        currentHealth = character.stats.GetStat(StatType.Health);

        character.healthEvents.RaiseUpdateHealthbarMax(character.stats.GetStat(StatType.Health));
        character.healthEvents.RaiseUpdateHealthbar(currentHealth);
    }

    public void UpdateHealth(float percentageOfMax)
    {
        float maxHealth = character.stats.GetStat(StatType.Health);
        currentHealth = maxHealth * percentageOfMax;

        character.healthEvents.RaiseUpdateHealthbarMax(maxHealth);
        character.healthEvents.RaiseUpdateHealthbar(currentHealth);
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public void OnReduceHealth(float damage)
    {
        float newHealth = currentHealth - damage;
        if (newHealth <= 0)
        {
            currentHealth = 0;
            Destroy(fillArea);
            // Death
            character.characterState.SetToDying();
            character.movementEvents.RaiseOnDying();
        }
        else
        {
            currentHealth = newHealth;
        }
        character.healthEvents.RaiseUpdateHealthbar(currentHealth);
    }

    public void OnIncreaseHealth(float health)
    {
        float newHealth = currentHealth + health;
        if (newHealth > character.stats.GetStat(StatType.Health))
        {
            currentHealth = character.stats.GetStat(StatType.Health);
        }
        else
        {
            currentHealth = newHealth;
        }
        character.healthEvents.RaiseUpdateHealthbar(currentHealth);
    }
}
