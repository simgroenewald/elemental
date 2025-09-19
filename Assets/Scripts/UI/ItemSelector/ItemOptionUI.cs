using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;

public class ItemOptionUI : MonoBehaviour
{

    [SerializeField] Image itemIcon;
    [SerializeField] TMP_Text itemName;
    [SerializeField] TMP_Text itemDescription;
    [SerializeField] Image border;
    [SerializeField] SoundEffectSO hoverSound;
    [SerializeField] SoundEffectSO clickSound;

    public event Action<ItemOptionUI> OnItemClicked;

    private bool isEmpty = true;

    public void ResetSlot()
    {
        itemIcon.sprite = null;
        itemName.text = "";
        itemDescription.text = "";
        isEmpty = true;
    }

    public void Deselect()
    {
        border.enabled = false;
    }

    public void SetItemOption(Sprite sprite, string name, string description)
    {
        itemIcon.sprite = sprite;
        itemName.text = name;
        itemDescription.text = description;
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
            OnItemClicked?.Invoke(this);
        }
    }

    public void OnPointerHover(BaseEventData data)
    {
        SoundEffectManager.Instance.PlaySoundEffect(hoverSound);
    }

}
