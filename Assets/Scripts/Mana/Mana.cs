using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Rendering;
using static Unity.Cinemachine.CinemachineFreeLookModifier;

[DisallowMultipleComponent]
public class Mana : MonoBehaviour
{
    private Coroutine manaRegenCoroutine;
    private float tickSeconds = 0.1f;
    private Character character;
    private float currentMana;
    private float regenBucket;
    private float flatBonusManaRegen;
    private float percentageBonusManaRegen;
    [SerializeField] private GameObject fillArea;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    private void OnEnable()
    {
        character.manaEvents.OnReduceMana += OnReduceMana;
        character.manaEvents.OnIncreaseMana += OnIncreaseMana;
        manaRegenCoroutine = StartCoroutine(RegenLoop());
    }

    private void OnDisable()
    {
        character.manaEvents.OnReduceMana -= OnReduceMana;
        character.manaEvents.OnIncreaseMana -= OnIncreaseMana;
        if (manaRegenCoroutine != null) StopCoroutine(manaRegenCoroutine);
    }

    private IEnumerator RegenLoop()
    {
        var wait = new WaitForSeconds(tickSeconds);

        while (true)
        {
            float maxMana = character.stats.GetStat(StatType.Mana);
            float manaRegen = character.stats.GetStat(StatType.ManaRegen);

            if (manaRegen > 0)
            {
                float manaRegenTotal = manaRegen * (1f + percentageBonusManaRegen) + flatBonusManaRegen;

                if (manaRegenTotal > 0f && currentMana < maxMana)
                {
                    regenBucket += manaRegenTotal * tickSeconds;

                    int whole = Mathf.FloorToInt(regenBucket);
                    if (whole > 0)
                    {
                        int amountToIncrease = Mathf.Min(whole, Mathf.CeilToInt(maxMana - currentMana));
                        if (amountToIncrease > 0)
                        {
                            OnIncreaseMana(amountToIncrease);
                            regenBucket -= amountToIncrease;
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
