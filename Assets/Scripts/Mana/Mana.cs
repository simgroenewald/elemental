using UnityEngine;
using UnityEngine.Rendering;

[DisallowMultipleComponent]
public class Mana : MonoBehaviour
{
    private Character character;
    private float currentMana;
    float regenBucket;
    [SerializeField] private GameObject fillArea;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    private void OnEnable()
    {
        character.manaEvents.OnReduceMana += OnReduceMana;
        character.manaEvents.OnIncreaseMana += OnIncreaseMana;
    }

    private void OnDisable()
    {
        character.manaEvents.OnReduceMana -= OnReduceMana;
        character.manaEvents.OnIncreaseMana -= OnIncreaseMana;
    }

    private void Update()
    {
        if (character.stats.GetStat(StatType.ManaRegen) != 0 && Time.frameCount % character.stats.GetStat(StatType.ManaRegen) == 0)
        {
            if (currentMana < character.stats.GetStat(StatType.Mana))
            {
                OnIncreaseMana(1);
            }
        }

        if (currentMana >= character.stats.GetStat(StatType.Mana) || character.stats.GetStat(StatType.ManaRegen) == 0) return;

        regenBucket += character.stats.GetStat(StatType.ManaRegen) * Time.deltaTime;
        int whole = Mathf.FloorToInt(regenBucket);
        if (whole > 0)
        {
            OnIncreaseMana(whole);
            regenBucket -= whole;
        }
    }

    public void SetMana()
    {
        currentMana = character.stats.GetStat(StatType.Mana);

        character.manaEvents.RaiseUpdateManabarMax(character.stats.GetStat(StatType.Mana));
        character.manaEvents.RaiseUpdateManabar(currentMana);
    }

    public void UpdateMana(float percentageOfMax)
    {
        float maxMana = character.stats.GetStat(StatType.Mana);
        currentMana = maxMana * percentageOfMax;

        character.manaEvents.RaiseUpdateManabarMax(maxMana);
        character.manaEvents.RaiseUpdateManabar(currentMana);
    }

    public float GetCurrentMana()
    {
        return currentMana;
    }

    public void OnReduceMana(float damage)
    {
        float newMana = currentMana - damage;
        if (newMana <= 0)
        {
            currentMana = 0;
        }
        else
        {
            currentMana = newMana;
        }
        character.manaEvents.RaiseUpdateManabar(currentMana);
    }

    public void OnIncreaseMana(float mana)
    {
        float newMana = currentMana + mana;
        if (newMana > character.stats.GetStat(StatType.Mana))
        {
            currentMana = character.stats.GetStat(StatType.Mana);
        }
        else
        {
            currentMana = newMana;
        }
        character.manaEvents.RaiseUpdateManabar(currentMana);
    }
}
