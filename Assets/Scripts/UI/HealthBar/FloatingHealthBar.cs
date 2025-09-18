using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class FloatingHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] Character character;
    [SerializeField] GameObject fill;

    float maxHealth;


    private void OnEnable()
    {
        character.healthEvents.OnUpdateHealthbarMax += OnUpdateMaxHealth;
        character.healthEvents.OnUpdateHealthbar += OnUpdatehealthbar;
    }

    private void OnDisable()
    {
        character.healthEvents.OnUpdateHealthbarMax -= OnUpdateMaxHealth;
        character.healthEvents.OnUpdateHealthbar -= OnUpdatehealthbar;
    }

    private void OnUpdateMaxHealth(float maxHealth)
    {
        ShowFill();
        this.maxHealth = maxHealth;
    }

    private void OnUpdatehealthbar(float currentHealth)
    {
        if (currentHealth <= 0)
        {
            HideFill();
        }
        slider.value = currentHealth / maxHealth;
    }

    private void HideFill()
    {
        fill.SetActive(false);
    }

    private void ShowFill()
    {
        fill.SetActive(true);
    }
}
