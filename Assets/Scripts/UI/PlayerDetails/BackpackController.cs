using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BackpackController : MonoBehaviour
{
    [SerializeField] public BackpackUI backpackUI;
    [SerializeField] public ItemDetailsUI itemDetails;
    [SerializeField] public BackpackSO backpack;

    public List<BackpackItem> initialItems = new List<BackpackItem>();

    private void Start()
    {
        SetUpUI();
        SetUpBackpack();
        UpdateBackpack();
    }

    private void SetUpBackpack()
    {
        backpack.Initialise();
        backpack.OnBackpackUpdated += UpdateBackpackUI;
        foreach (BackpackItem item in initialItems)
        {
            if (item.isEmpty)
                continue;
            backpack.AddItem(item);
        }
    }

    private void UpdateBackpackUI(Dictionary<int, BackpackItem> backpackDict)
    {
        backpackUI.ResetAllItems();
        foreach (var item in backpackDict)
        {
            backpackUI.UpdateItemSlot(item.Key, item.Value.item.ItemImage, item.Value.quantity);
        }
    }

    private void SetUpUI()
    {
        backpackUI.Initialise(backpack.Size);
        this.backpackUI.OnDescriptionRequested += HandleDescriptionRequested;
        this.backpackUI.OnSwapItems += HandleSwapItems;
        this.backpackUI.OnStartDragging += HandleDragging;
        this.backpackUI.OnItemActionRequested += HandleItemActionRequested;
    }

    private void HandleItemActionRequested(int index)
    {
    }

    private void HandleDragging(int index)
    {
        BackpackItem backpackItem = backpack.GetItemAtIndex(index);
        if (backpackItem.isEmpty)
            return;
        backpackUI.SetDragItem(backpackItem.item.ItemImage, backpackItem.quantity);
    }

    private void HandleSwapItems(int index1, int index2)
    {
        backpack.SwapItems(index1, index2);
    }

    private void HandleDescriptionRequested(int index)
    {
        BackpackItem backpackItem = backpack.GetItemAtIndex(index);

        if (backpackItem.isEmpty)
        {
            backpackUI.Unfocus();
            return;
        }
        ItemSO item = backpackItem.item;
        itemDetails.UpdateItemDetails(item.ItemImage, item.Name, item.Description);
        backpackUI.DeselectAllItems();
        backpackUI.Focus(index);
    }

    private void UpdateBackpack()
    {
        foreach (var item in backpack.GetCurrentBackpackState())
        {
            backpackUI.UpdateItemSlot(
                item.Key,
                item.Value.item.ItemImage,
                item.Value.quantity
                );
        }
    }
}
