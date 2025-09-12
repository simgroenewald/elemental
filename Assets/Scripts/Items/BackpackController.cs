using DarkPixelRPGUI.Scripts.UI.Equipment;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class BackpackController : MonoBehaviour
{
    public BackpackUI backpackUI;
    public ItemDetailsUI itemDetails;
    [SerializeField] public BackpackSO backpack;

    public List<BackpackItem> initialItems = new List<BackpackItem>();

    private void Awake()
    {
        backpackUI = GameManager.Instance.backpackUI;
        itemDetails = GameManager.Instance.itemDetailsUI;
    }

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
        backpack.OnNewItemAdded += ApplyEffects;
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
        this.backpackUI.OnItemActionsRequested += HandleItemActionRequested;
        this.backpackUI.OnItemPerformAction += PerformAction;
    }

    private void HandleItemActionRequested(int index)
    {
        BackpackItem backpackItem = backpack.GetItemAtIndex(index);
        if (backpackItem.isEmpty)
            return;
        backpackUI.ShowItemAction(index);
        IItemAction itemAction = backpackItem.item as IItemAction;
        if (itemAction != null)
        {
            backpackUI.AddAction(itemAction.ActionName, () => PerformAction(index));
        }
        IDestroyableItem destroyableItem = backpackItem.item as IDestroyableItem;
        if (destroyableItem != null)
        {
            backpackUI.AddAction("Drop", () => DropItem(index, backpackItem.quantity));
        }
        IDroppable droppableItem = backpackItem.item as IDroppable;
        if (droppableItem != null)
        {
            backpackUI.AddAction("Drop", () => RemoveEffects(index, backpackItem.quantity));
        }
    }

    private void DropItem(int index, int quantity)
    {
        backpack.RemoveItem(index, quantity);
        backpackUI.Unfocus();
    }


    public void PerformAction(int index)
    {
        BackpackItem backpackItem = backpack.GetItemAtIndex(index);
        if (backpackItem.isEmpty)
            return;
        IDestroyableItem destroyableItem = backpackItem.item as IDestroyableItem;
        if (destroyableItem != null)
        {
            backpack.RemoveItem(index, 1);
        }
        IItemAction itemAction = backpackItem.item as IItemAction;
        if (itemAction != null)
        {
            itemAction.PerformAction(gameObject, backpackItem.itemParameters);
            if (backpack.GetItemAtIndex(index).isEmpty)
            {
                backpackUI.Unfocus();
            }
        }
    }

    public void ApplyEffects(int index)
    {
        BackpackItem backpackItem = backpack.GetItemAtIndex(index);
        IItemPassive itemPassive = backpackItem.item as IItemPassive;
        if (itemPassive != null)
        {
            itemPassive.ApplyEffects(gameObject, backpackItem.itemParameters);
            if (backpack.GetItemAtIndex(index).isEmpty)
            {
                backpackUI.Unfocus();
            }
        }
    }

    public void RemoveEffects(int index, int quantity)
    {
        BackpackItem backpackItem = backpack.GetItemAtIndex(index);
        IItemPassive itemPassive = backpackItem.item as IItemPassive;
        if (itemPassive != null)
        {
            itemPassive.RemoveEffects(gameObject, backpackItem.itemParameters);
            backpack.RemoveItem(index, quantity);
            backpackUI.Unfocus();
        }
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
            itemDetails.ResetItemDetails();
            return;
        }
        ItemSO item = backpackItem.item;
        string description = SetUpDescription(backpackItem);
        itemDetails.UpdateItemDetails(item.ItemImage, item.Name, description);
        backpackUI.DeselectAllItems();
        backpackUI.Focus(index);
    }

    public string SetUpDescription(BackpackItem backpackItem)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(backpackItem.item.Description);
        sb.AppendLine();
        for (int i = 0; i < backpackItem.itemParameters.Count; i++)
        {
            sb.Append($"{backpackItem.itemParameters[i].itemParameterSO.ParameterName} : {backpackItem.itemParameters[i].value}/{backpackItem.item.DefaultParametersList[i].value}");
            sb.AppendLine();
        }
        return sb.ToString();
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
