using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainManaBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text manaText;
    [SerializeField] Player player;

    float currentMana;
    float maxMana;

    private void Start()
    {
        player = GameManager.Instance.player;
        player.manaEvents.OnUpdateManabarMax += OnUpdateMaxMana;
        player.manaEvents.OnUpdateManabar += OnUpdateManabar;
    }

    private void OnDisable()
    {
        player.manaEvents.OnUpdateManabarMax += OnUpdateMaxMana;
        player.manaEvents.OnUpdateManabar -= OnUpdateManabar;
    }

    private void OnUpdateMaxMana(float maxMana)
    {
        this.maxMana = maxMana;
        Updatetext((int)currentMana, (int)maxMana);
    }

    private void OnUpdateManabar(float currentMana)
    {
        this.currentMana = currentMana;
        slider.value = currentMana / maxMana;
        Updatetext((int)currentMana, (int)maxMana);
    }

    private void Updatetext(int currentMana, int maxMana)
    {
        manaText.text = $"{currentMana} / {maxMana}";
    }
}
