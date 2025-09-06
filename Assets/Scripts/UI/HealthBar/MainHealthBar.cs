using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class MainHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] Player player;

    float currentHealth;
    float maxHealth;

    private void Start()
    {
        player = GameManager.Instance.player;
        player.healthEvents.OnUpdateHealthbarMax += OnUpdateMaxHealth;
        player.healthEvents.OnUpdateHealthbar += OnUpdatehealthbar;
    }

    private void OnDisable()
    {
        player.healthEvents.OnUpdateHealthbarMax += OnUpdateMaxHealth;
        player.healthEvents.OnUpdateHealthbar -= OnUpdatehealthbar;
    }

    private void OnUpdateMaxHealth(float maxHealth)
    {
        this.maxHealth = maxHealth;
        Updatetext((int)currentHealth, (int)maxHealth);
    }

    private void OnUpdatehealthbar(float currentHealth)
    {
        this.currentHealth = currentHealth;
        slider.value = currentHealth / maxHealth;
        Updatetext((int)currentHealth, (int)maxHealth);
    }

    private void Updatetext(int currentHealth, int maxHealth)
    {
        healthText.text = $"{currentHealth} / {maxHealth}";
    }
}
