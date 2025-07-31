using UnityEngine;

[DisallowMultipleComponent]
public class Health: MonoBehaviour
{
    private int maxHealth;
    private int currentHealth;

    public void SetMaxHealth(int maxHealth)
    {
        this.maxHealth = maxHealth;
        currentHealth = maxHealth;
    }

    public int GetMaxHealth() { 
        return this.maxHealth;
    }
}
