using UnityEngine;
using UnityEngine.Rendering;

[DisallowMultipleComponent]
public class Health: MonoBehaviour
{
    private Character character;
    private float maxHealth;
    private float currentHealth;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    private void OnEnable()
    {
        character.healthEvents.OnReduceHealth += OnReduceHealth;
        character.healthEvents.OnIncreaseHealth += OnIncreaseHealth;
    }

    private void OnDisable()
    {
        character.healthEvents.OnReduceHealth -= OnReduceHealth;
        character.healthEvents.OnIncreaseHealth -= OnIncreaseHealth;
    }

    public void SetHealth(float maxHealth)
    {
        this.maxHealth = maxHealth;
        currentHealth = maxHealth;
        character.healthEvents.RaiseUpdateHealthbarMax(maxHealth);
    }

    public float GetMaxHealth() { 
        return maxHealth;
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
            // Death
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
        if (newHealth > maxHealth)
        {
            currentHealth = maxHealth;
        } else
        {
            currentHealth = newHealth;
        }
        character.healthEvents.RaiseUpdateHealthbar(currentHealth);
    }
}
