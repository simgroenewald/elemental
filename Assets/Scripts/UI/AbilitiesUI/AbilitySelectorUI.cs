using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class AbilitySelectorUI : MonoBehaviour
{
    [SerializeField] private AbilityUI abilityUIPrefab;
    [SerializeField] private RectTransform abilities;
    [SerializeField] private DetailsUI detailsUI;
    [SerializeField] private ActionKeysSO actionKeys;
    List<AbilityUI> abilityUIList = new List<AbilityUI>();

    public event Action<int> OnDescriptionRequested;
    public event Action<int> OnStageAbility;

    private void Awake()
    {
        detailsUI.ResetDetails();
    }

    private void Update()
    {
        foreach (var binding in actionKeys.actionKeyBindings)
        {
            if (Input.GetKeyDown(binding.key))
            {
                Unfocus();
                Focus(binding.index);
                OnStageAbility?.Invoke(binding.index);
                return;
            }
        }
    }

    public void Initialise(int size)
    {
        for (int i = 0; i < size; i++)
        {
            string actionBtnTxt = GetActionButton(i);
            AbilityUI abilityUI = Instantiate(abilityUIPrefab, abilities);
            abilityUI.InitialiseControl(actionBtnTxt);
            abilityUIList.Add(abilityUI);
            abilityUI.OnAbilityClicked += HandleAbilitySelected;
        }
    }

    private void OnDestroy()
    {
        for (int i = 0; i < abilityUIList.Count; i++)
        {
            abilityUIList[i].OnAbilityClicked -= HandleAbilitySelected;
        }
    }

    private string GetActionButton(int i)
    {
        List<string> actionButtons = new List<string> {"Q", "W", "E", "R"};
        string actionBtnTxt = actionButtons[i];
        return actionBtnTxt;
    }

    public void UpdateAbilitySlot(int index, Sprite image)
    {
        if (abilityUIList.Count > index)
        {
            abilityUIList[index].SetAbilitySlot(image);
        }
    }

    private void HandleAbilitySelected(AbilityUI abilityUI)
    {
        int index = abilityUIList.IndexOf(abilityUI);
        if (index == -1)
        {
            return;
        }
        OnDescriptionRequested?.Invoke(index);
        OnStageAbility?.Invoke(index);
    }

    public void Focus(int index)
    {
        DeselectAllAbilities();
        abilityUIList[index].Select();
    }

    public void Unfocus()
    {
        DeselectAllAbilities();
        detailsUI.ResetDetails();
    }

    public void DeselectAllAbilities()
    {
        foreach (AbilityUI abilityUI in abilityUIList)
        {
            abilityUI.Deselect();
        }
    }

    internal void ResetAllAbilities()
    {
        detailsUI.ResetDetails();
        foreach (var abilityUI in abilityUIList)
        {
            abilityUI.ResetSlot();
            abilityUI.Deselect();
        }
    }

    public void UpdateCooldown(int index, float value)
    {
        abilityUIList[index].UpdateCooldown(value);
    }

    public void ResetCooldown(int index)
    {
        abilityUIList[index].ResetCooldown();
    }

    public void UpdateEffectTime(int index, float value)
    {
        abilityUIList[index].UpdateEffectTime(value);
    }

    public void ResetEffectTime(int index)
    {
        abilityUIList[index].ResetEffectTime();
    }

    public void LockAbility(int index)
    {
        abilityUIList[index].LockAbility();
    }

    public void UnlockAbility (int index)
    {
        abilityUIList[index].UnlockAbility();
    }

}
