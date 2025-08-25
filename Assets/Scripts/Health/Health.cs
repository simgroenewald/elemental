using UnityEngine;
using UnityEngine.Rendering;

[DisallowMultipleComponent]
public class Health: MonoBehaviour
{
    private Character character;
    private float maxHealth;
    private float currentHealth;
    private float healthRegenRate;
    float regenBucket;
    [SerializeField] private GameObject fillArea;

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

    private void Update()
    {
        /*        if (characterStats.healthRegenRate != 0 && Time.frameCount % characterStats.healthRegenRate == 0)
                {
                    if (currentHealth < maxHealth)
                    {
                        OnIncreaseHealth(1);
                    }
                }*/

        if (currentHealth >= maxHealth || healthRegenRate == 0) return;

        regenBucket += healthRegenRate * Time.deltaTime;
        int whole = Mathf.FloorToInt(regenBucket);
        if (whole > 0)
        {
            OnIncreaseHealth(whole);
            regenBucket -= whole;
        }
    }

    public void SetHealth(float maxHealth, float currentHealth)
    {
        this.maxHealth = maxHealth;
        this.currentHealth = currentHealth;
        character.healthEvents.RaiseUpdateHealthbarMax(maxHealth);
        character.healthEvents.RaiseUpdateHealthbar(currentHealth);
    }

    public void SetHealthRegenRate(float healthRegenRate)
    {
        this.healthRegenRate = healthRegenRate;
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
