using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class AbilityController : MonoBehaviour
{
    public AbilitySelectorUI abilitySelectorUI;
    public ItemDetailsUI itemDetails;
    [SerializeField] private ActionKeysSO actionKeys;
    List<Ability> abilities;
    private Player player;

    public List<BackpackItem> initialItems = new List<BackpackItem>();

    private void Awake()
    {
        itemDetails = GameManager.Instance.itemDetailsUI;
        abilitySelectorUI = GameManager.Instance.abilitySelectorUI;
    }

    private void Start()
    {
        player = GameManager.Instance.player;
        abilities = player.abilityList;
        ActivatePassiveAbilities();
        SetUpUI();
        UpdateAbilitiesUI();
    }

    private void ActivatePassiveAbilities()
    {
        foreach (var ability in abilities)
        {
            if (ability.abilityDetails.isPassive)
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

    }

    private void UpdateAbilitiesUI()
    {
        abilitySelectorUI.ResetAllAbilities();
        for (int i = 0; i < abilities.Count; i++)
        {
            abilities[i].abilityDetails.triggerKey = actionKeys.actionKeyBindings[i].key;
            abilitySelectorUI.UpdateAbilitySlot(i, abilities[i].abilityDetails.icon);
        }
    }

    private void SetUpUI()
    {
        abilitySelectorUI.Initialise(4);
        this.abilitySelectorUI.OnDescriptionRequested += HandleDescriptionRequested;
        this.abilitySelectorUI.OnStageAbility += StageAbility;
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
        itemDetails.UpdateItemDetails(abilityDetails.icon, abilityDetails.abilityName, description);
        abilitySelectorUI.DeselectAllAbilities();
        abilitySelectorUI.Focus(index);
    }

    public string SetUpDescription(AbilityDetailsSO abilityDetails)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(abilityDetails.description);
        sb.AppendLine();
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
            sb.AppendLine();
        }
        if (abilityDetails.hasEffectTime)
        {
            sb.Append($"Effect Time: {abilityDetails.effectTime}s");
            sb.AppendLine();
        }
        if (abilityDetails.hasCooldown)
        {
            sb.Append($"Cooldown Time: {abilityDetails.coolDownTime}s");
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
