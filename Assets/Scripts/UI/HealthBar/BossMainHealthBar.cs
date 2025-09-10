using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossMainHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private GameObject healthbarObject;
    private Character enemy;

    float currentHealth;
    float maxHealth;

    private void OnDisable()
    {
        GameEventManager.Instance.globalUIEvents.OnShowBossMainHealthBar -= SetUpHealthBar;
        GameEventManager.Instance.globalUIEvents.OnRemoveBossMainHealthBar -= RemoveHealthBar;
    }

    private void Start()
    {
        GameEventManager.Instance.globalUIEvents.OnShowBossMainHealthBar += SetUpHealthBar;
        GameEventManager.Instance.globalUIEvents.OnRemoveBossMainHealthBar += RemoveHealthBar;
        healthbarObject.SetActive(false);
    }

    private void SetUpHealthBar(Character enemy)
    {
        healthbarObject.SetActive(true);
        this.enemy = enemy;
        enemy.healthEvents.OnUpdateHealthbarMax += OnUpdateMaxHealth;
        enemy.healthEvents.OnUpdateHealthbar += OnUpdateHealthbar;
        OnUpdateMaxHealth(enemy.stats.GetStat(StatType.Health));
        OnUpdateHealthbar(enemy.stats.GetStat(StatType.Health));
    }

    private void RemoveHealthBar()
    {
        enemy.healthEvents.OnUpdateHealthbarMax -= OnUpdateMaxHealth;
        enemy.healthEvents.OnUpdateHealthbar -= OnUpdateHealthbar;
        healthbarObject.SetActive(false);
        enemy = null;
    }

    private void OnUpdateMaxHealth(float maxHealth)
    {
        this.maxHealth = maxHealth;
        Updatetext((int)currentHealth, (int)maxHealth);
    }

    private void OnUpdateHealthbar(float currentHealth)
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
