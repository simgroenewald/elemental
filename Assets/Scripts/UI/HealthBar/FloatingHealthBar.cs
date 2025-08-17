using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class FloatingHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] Character character;

    float maxHealth;


    private void OnEnable()
    {
        character.healthEvents.OnUpdateHealthbarMax += OnUpdateMaxHealth;
        character.healthEvents.OnUpdateHealthbar += OnUpdatehealthbar;
    }

    private void OnDisable()
    {
        character.healthEvents.OnUpdateHealthbarMax += OnUpdateMaxHealth;
        character.healthEvents.OnUpdateHealthbar -= OnUpdatehealthbar;
    }

    private void OnUpdateMaxHealth(float maxHealth)
    {
        this.maxHealth = maxHealth;
    }

    private void OnUpdatehealthbar(float currentHealth)
    {
        slider.value = currentHealth / maxHealth;
    }
}
