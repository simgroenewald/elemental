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
    [SerializeField] Image cooldown;
    [SerializeField] Image effectTime;
    [SerializeField] Image lockedImage;
    [SerializeField] SoundEffectSO clickSound;

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
        {
            SoundEffectManager.Instance.PlaySoundEffect(clickSound);
            OnAbilityClicked?.Invoke(this);
        }
    }

    public void UpdateCooldown(float fillAmount)
    {
        cooldown.fillAmount = fillAmount;
    }

    public void ResetCooldown()
    {
        cooldown.fillAmount = 0;
    }

    public void UpdateEffectTime(float value)
    {
        effectTime.fillAmount = 1 - value;
    }

    public void ResetEffectTime()
    {
        effectTime.fillAmount = 0;
    }

    public void LockAbility()
    {
        lockedImage.gameObject.SetActive(true);
    }

    public void UnlockAbility()
    {
        lockedImage.gameObject.SetActive(false);
    }
}
