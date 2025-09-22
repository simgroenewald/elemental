using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class AbilityController : MonoBehaviour
{
    public AbilitySelectorUI abilitySelectorUI;
    public AbilityUnlockedUI abilityUnlockedUI;
    public DetailsUI details;
    [SerializeField] private ActionKeysSO actionKeys;
    [SerializeField] private SoundEffectSO abilityUnlockedSound;
    List<Ability> abilities;
    private Player player;

    public List<BackpackItem> initialItems = new List<BackpackItem>();

    private void Awake()
    {
        details = GameManager.Instance.detailsUI;
        abilitySelectorUI = GameManager.Instance.abilitySelectorUI;
        abilityUnlockedUI = GameManager.Instance.abilityUnlockedUI;
    }

    private void Start()
    {
        player = GameManager.Instance.player;
        abilities = player.abilityList;
        SetUpUI();
        UpdateAbilitiesUI();
        abilityUnlockedUI.OnContinue += HandleContinue;
    }

    public void ResetAbilities()
    {
        UpdateAbilitiesUI();
    }

    private void ActivatePassiveAbility(Ability ability)
    {
        if (ability.abilityDetails.isPassive && !ability.locked)
        {
            if (ability.abilityDetails.modifierData.Count > 0)
            {
                foreach (var modifier in ability.abilityDetails.modifierData)
                {
                    player.statModifierEvents.RaiseAddBasicStatEvent(modifier.statType, modifier.value, modifier.isPercentage);
                }
            }
        }
    }

    public void UnlockAbility(bool showUI, int index)
    {
        GameManager.Instance.newAbilityUnlocked = true;
        if (index >= abilities.Count)
        {
            return;
        }
        Ability ability = abilities[index];

        if (showUI)
        {
            SoundEffectManager.Instance.PlaySoundEffect(abilityUnlockedSound);
            string description = SetUpDescription(ability.abilityDetails);
            abilityUnlockedUI.SetAbilityDetails(ability.abilityDetails.icon, ability.abilityDetails.name, description);
        }

        ability.locked = false;
        abilitySelectorUI.UnlockAbility(index);
        ActivatePassiveAbility(ability);
    }

    public void HandleContinue()
    {
        abilityUnlockedUI.ResetAbilityDetails();
        GameManager.Instance.state = GameState.playing;
        if (GameManager.Instance.previousState == GameState.bossRoom)
        {
            GameManager.Instance.SetStateCompleteLevel();
        }
    }

    private void Update()
    {
        for (int i = 0; i < abilities.Count; i++)
        {
            if (abilities[i].isEffectTime)
            {
                abilities[i].abilityEffectTime += Time.deltaTime;

                float ratio = abilities[i].abilityEffectTime / abilities[i].abilityDetails.effectTime;
                abilitySelectorUI.UpdateEffectTime(i, ratio);

                if (abilities[i].abilityEffectTime >= abilities[i].abilityDetails.effectTime)
                {
                    abilities[i].EffectTimeEnd();
                    abilitySelectorUI.ResetEffectTime(i);
                }
            }
            if (abilities[i].isCoolingDown)
            {
                abilities[i].abilityCooldownTime -= Time.deltaTime;

                float ratio = abilities[i].abilityCooldownTime / abilities[i].abilityDetails._coolDownTime;
                abilitySelectorUI.UpdateCooldown(i, ratio);

                // Clamp to 0 and mark cooldown finished
                if (abilities[i].abilityCooldownTime <= 0f)
                {
                    abilities[i].ResetStates();
                    abilitySelectorUI.ResetCooldown(i);
                }

            }
        }
    }

    private void UpdateAbilitiesUI()
    {
        abilitySelectorUI.ResetAllAbilities();
        for (int i = 0; i < abilities.Count; i++)
        {
            abilities[i].locked = true;
            abilities[i].abilityDetails.triggerKey = actionKeys.actionKeyBindings[i].key;
            abilitySelectorUI.UpdateAbilitySlot(i, abilities[i].abilityDetails.icon);
            abilitySelectorUI.LockAbility(i);
        }
        UnlockAbility(false, 0);
    }

    private void SetUpUI()
    {
        abilitySelectorUI.Initialise(4);
        this.abilitySelectorUI.OnDescriptionRequested += HandleDescriptionRequested;
        this.abilitySelectorUI.OnStageAbility += StageAbility;
    }

    private void OnDestroy()
    {
        this.abilitySelectorUI.OnDescriptionRequested -= HandleDescriptionRequested;
        this.abilitySelectorUI.OnStageAbility -= StageAbility;
    }


    public void StageAbility(int index)
    {
        Debug.Log("Ability Staging Event");
        GameManager.Instance.player.abilityActivationEvents.CallStageAbilityEvent(abilities[index]);
    }

    private void HandleDescriptionRequested(int index)
    {
        Ability ability = abilities[index];

        AbilityDetailsSO abilityDetails = ability.abilityDetails;
        string description = SetUpDescription(abilityDetails);
        details.UpdateDetails(abilityDetails.icon, abilityDetails.abilityName, description);
        abilitySelectorUI.DeselectAllAbilities();
        abilitySelectorUI.Focus(index);
    }

    public string SetUpDescription(AbilityDetailsSO abilityDetails)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(abilityDetails.description);
        sb.AppendLine();
        if (!abilityDetails.isEnemyTargetable && !abilityDetails.isPassive)
        {
            sb.Append("Triggerable");
            sb.AppendLine();
        } else if (abilityDetails.isEnemyTargetable)
        {
            sb.Append("Targeted");
            sb.AppendLine();
        }
        if (abilityDetails.isPassive)
        {
            sb.Append("Passive");
            sb.AppendLine();
        }
        if (abilityDetails.isCritical)
        {
            sb.Append($"Critical Damage Chance: {abilityDetails.critChance}%");
            sb.AppendLine();
        }
        if (abilityDetails.damage > 0)
        {
            sb.Append($"Damage: {abilityDetails.damage}");
            if (abilityDetails.isMagical)
            {
                sb.Append(" (Magical)");
            }
            else
            {
                sb.Append(" (Physical)");
            }
            sb.AppendLine();
        }
        if (abilityDetails.hasEffectTime)
        {
            sb.Append($"Effect Time: {abilityDetails.effectTime}s");
            sb.AppendLine();
        }
        if (abilityDetails.hasCooldown)
        {
            sb.Append($"Cooldown Time: {abilityDetails._coolDownTime}s");
            sb.AppendLine();
        }
        if (abilityDetails.manaCost > 0)
        {
            sb.Append($"Mana cost: {abilityDetails.manaCost}");
            sb.AppendLine();
        }
        return sb.ToString();
    }
}
