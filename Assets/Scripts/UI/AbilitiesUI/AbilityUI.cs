using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;

public class AbilityUI : MonoBehaviour
{

    [SerializeField] Image abilityIcon;
    [SerializeField] Sprite emptyIcon;
    [SerializeField] TMP_Text controlText;
    [SerializeField] Image border;

    public event Action<AbilityUI> OnAbilityClicked;

    private bool isEmpty = true;

    public void Awake()
    {
        ResetSlot();
        Deselect();
    }

    public void InitialiseControl(string control)
    {
        controlText.text = control;
    }

    public void ResetSlot()
    {
        abilityIcon.sprite = emptyIcon;
        isEmpty = true;
    }

    public void Deselect()
    {
        border.enabled = false;
    }

    public void SetAbilitySlot(Sprite sprite)
    {
        abilityIcon.sprite = sprite;
        isEmpty = false;
    }

    public void Select()
    {
        border.enabled = true;
    }

    public void OnPointerClick(BaseEventData data)
    {
        PointerEventData pointerData = (PointerEventData)data;
        if (pointerData.button == PointerEventData.InputButton.Left)
            OnAbilityClicked?.Invoke(this);

    }
}
