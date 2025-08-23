using DarkPixelRPGUI.Scripts.UI.Equipment;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackpackUI : MonoBehaviour
{
    [SerializeField] private ItemUI itemUIPrefab;
    [SerializeField] private RectTransform backpack;
    [SerializeField] private ItemDetailsUI itemDetailsUI;
    [SerializeField] private DragItemUI dragItemUI;
    List<ItemUI> itemUIList = new List<ItemUI>();

    private int currentDraggedIndex = -1;

    public event Action<int> OnDescriptionRequested;
    public event Action<int> OnItemActionRequested;
    public event Action<int> OnStartDragging;
    public event Action<int, int> OnSwapItems;

    private void Awake()
    {
        dragItemUI.Toggle(false);
        itemDetailsUI.ResetItemDetails();
    }

    public void Initialise(int size)
    {
        for (int i = 0; i < size; i++)
        {
            ItemUI itemUI = Instantiate(itemUIPrefab, backpack);
            itemUI.InitialiseControl(i.ToString());
            itemUIList.Add(itemUI);
            itemUI.OnItemClicked += HandleItemSelected;
            itemUI.OnItemBeginDrag += HandleBeginDrag;
            itemUI.OnItemDroppedOn += HandleSwap;
            itemUI.OnItemEndDrag += HandleEndDrag;
            itemUI.OnRightMouseBtnClick += HandleShowItemActionMenu;
        }
    }

    public void UpdateItemSlot(int index, Sprite image, int quantity)
    {
        if (itemUIList.Count > index)
        {
            itemUIList[index].SetItemSlot(image, quantity);
        }
    }

    private void HandleShowItemActionMenu(ItemUI itemUI)
    {
    }

    private void HandleEndDrag(ItemUI itemUI)
    {
        ResetDragItem();
    }

    private void HandleSwap(ItemUI itemUI)
    {
        int index = itemUIList.IndexOf(itemUI);
        if (index == -1)
        {
            return;
        }
        OnSwapItems?.Invoke(currentDraggedIndex, index);
        HandleItemSelected(itemUI);
    }

    private void ResetDragItem()
    {
        dragItemUI.Toggle(false);
        currentDraggedIndex = -1;
    }

    private void HandleBeginDrag(ItemUI itemUI)
    {
        int index = itemUIList.IndexOf(itemUI);
        if (index == -1)
            return;
        currentDraggedIndex = index;
        HandleItemSelected(itemUI);
        OnStartDragging?.Invoke(currentDraggedIndex);
    }

    public void SetDragItem(Sprite sprite, int quantity)
    {
        dragItemUI.Toggle(true);
        dragItemUI.SetItemSlot(sprite, quantity);
    }

    private void HandleItemSelected(ItemUI itemUI)
    {
        int index = itemUIList.IndexOf(itemUI);
        if (index == -1)
        {
            return;
        }
        OnDescriptionRequested?.Invoke(index);

    }

    public void Focus(int index)
    {
        DeselectAllItems();
        itemUIList[index].Select();
    }

    public void Unfocus()
    {
        ResetDragItem();
        DeselectAllItems();
        itemDetailsUI.ResetItemDetails();
    }

    public void DeselectAllItems()
    {
        foreach (ItemUI itemUI in itemUIList)
        {
            itemUI.Deselect();
        }
    }

    internal void ResetAllItems()
    {
        foreach (var item in itemUIList)
        {
            item.ResetSlot();
            item.Deselect();
        }
    }
}
