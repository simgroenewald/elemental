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
    [SerializeField] private DetailsUI detailsUI;
    [SerializeField] private DragItemUI dragItemUI;
    [SerializeField] private ItemActionUI itemActionUI;
    [SerializeField] private ActionKeysSO actionKeys;
    List<ItemUI> itemUIList;

    private int currentDraggedIndex = -1;

    public event Action<int> OnDescriptionRequested;
    public event Action<int> OnItemActionsRequested;
    public event Action<int> OnItemPerformAction;
    public event Action<int> OnStartDragging;
    public event Action<int, int> OnSwapItems;

    private void Awake()
    {
        itemUIList = new List<ItemUI>();
        dragItemUI.Toggle(false);
        detailsUI.ResetDetails();
    }

    public void ResetBackpackUI()
    {
        dragItemUI.Toggle(false);
        detailsUI.ResetDetails();
        ResetAllItems();
    }

    private void Update()
    {
        foreach (var binding in actionKeys.actionKeyBindings)
        {
            if (Input.GetKeyDown(binding.key))
            {
                OnItemPerformAction?.Invoke(binding.index);
                return;
            }
        }
    }

    private void OnDestroy()
    {
        foreach (ItemUI itemUI in itemUIList)
        {
            itemUI.OnItemClicked -= HandleItemSelected;
            itemUI.OnItemBeginDrag -= HandleBeginDrag;
            itemUI.OnItemDroppedOn -= HandleSwap;
            itemUI.OnItemEndDrag -= HandleEndDrag;
            itemUI.OnRightMouseBtnClick -= HandleShowItemActionMenu;
        }

    }

    public void Initialise(int size)
    {
        for (int i = 0; i < size; i++)
        {
            string actionBtnTxt = GetActionButton(i);
            ItemUI itemUI = Instantiate(itemUIPrefab, backpack);
            itemUI.InitialiseControl(actionBtnTxt);
            itemUIList.Add(itemUI);
            itemUI.OnItemClicked += HandleItemSelected;
            itemUI.OnItemBeginDrag += HandleBeginDrag;
            itemUI.OnItemDroppedOn += HandleSwap;
            itemUI.OnItemEndDrag += HandleEndDrag;
            itemUI.OnRightMouseBtnClick += HandleShowItemActionMenu;
        }
    }

    private string GetActionButton(int i)
    {
        string actionBtnTxt;
        if (i == 9)
            actionBtnTxt = "0";
        else
            actionBtnTxt = (i + 1).ToString();
        return actionBtnTxt;
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
        int index = itemUIList.IndexOf(itemUI);
        if (index == -1)
        {
            return;
        }
        OnItemActionsRequested?.Invoke(index);
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
        detailsUI.ResetDetails();
    }

    public void DeselectAllItems()
    {
        foreach (ItemUI itemUI in itemUIList)
        {
            itemUI.Deselect();
        }
        itemActionUI.Toggle(false);
    }

    internal void ResetAllItems()
    {
        foreach (var item in itemUIList)
        {
            item.ResetSlot();
            item.Deselect();
        }
        itemActionUI.Toggle(false);
    }

    public void ShowItemAction(int itemIndex)
    {
        itemActionUI.Toggle(true);
        itemActionUI.transform.position = itemUIList[itemIndex].transform.position;
    }

    public void AddAction(string actionName, Action performAction)
    {
        itemActionUI.AddButton(actionName, performAction);
    }
}
