using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;

public class ItemUI : MonoBehaviour
{

    [SerializeField] Image itemIcon;
    [SerializeField] Sprite emptyIcon;
    [SerializeField] TMP_Text controlText;
    [SerializeField] TMP_Text quantityText;
    [SerializeField] Image border;

    public event Action<ItemUI>
        OnItemClicked,
        OnItemDroppedOn,
        OnItemBeginDrag,
        OnItemEndDrag,
        OnRightMouseBtnClick,
        OnDoubleClick;

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
        itemIcon.sprite = emptyIcon;
        quantityText.text = "0";
        isEmpty = true;
    }

    public void Deselect()
    {
        border.enabled = false;
    }

    public void SetItemSlot(Sprite sprite, int quantity)
    {
        itemIcon.sprite = sprite;
        quantityText.text = quantity.ToString();
        isEmpty = false;
    }

    public void Select()
    {
        border.enabled = true;
    }

    public void OnBeginDrag()
    {
        if (isEmpty) {
            return;
        }
        OnItemBeginDrag?.Invoke(this);
    }

    public void OnDrop()
    {
        OnItemDroppedOn?.Invoke(this);
    }

    public void OnEndDrag()
    {
        OnItemEndDrag?.Invoke(this);
    }

    public void OnPointerClick(BaseEventData data)
    {
        PointerEventData pointerData = (PointerEventData)data;
        if (pointerData.button == PointerEventData.InputButton.Right)
        {
            OnRightMouseBtnClick?.Invoke(this);
        } else
        {
            OnItemClicked?.Invoke(this);
        }
    }
}
