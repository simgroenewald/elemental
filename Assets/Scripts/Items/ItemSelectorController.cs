using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemSelectorController : MonoBehaviour
{
    public ItemSelectorUI itemSelectorUI;
    [SerializeField] public ItemOptionsSO itemOptions;
    public ItemSO selectedItem;

    public List<BackpackItem> initialItems = new List<BackpackItem>();

    private void Awake()
    {
        itemSelectorUI = GameObject.FindWithTag("ItemSelector").GetComponent<ItemSelectorUI>();
    }

    private void Start()
    {
        itemOptions.OnItemOptionsUpdated += UpdateItemSelectorUI;
        itemSelectorUI.OnItemSelected += HandleItemSelected;
        itemSelectorUI.OnContinue += HandleContinue;
        SetUpUI();
    }

    private void OnDestroy()
    {
        itemOptions.OnItemOptionsUpdated -= UpdateItemSelectorUI;
        itemSelectorUI.OnItemSelected -= HandleItemSelected;
        itemSelectorUI.OnContinue -= HandleContinue;
    }

    private void SetUpUI()
    {
        itemSelectorUI.Initialise(3);
    }

    public void SetUpItemSelection(List<ItemSO> items)
    {
        itemOptions.AddItems(items);
    }

    private void UpdateItemSelectorUI(Dictionary<int, ItemSO> itemSelectionDict)
    {
        foreach (var item in itemSelectionDict)
        {
            itemSelectorUI.UpdateItemOption(item.Key, item.Value.ItemImage, item.Value.Name, item.Value.Description);
        }
        itemSelectorUI.Show();
        GameManager.Instance.state = GameState.paused;
    }

    public void HandleContinue()
    {
        GameEventManager.Instance.itemEvents.RaiseDropSelectedItemEvent(GameManager.Instance.currentDungeonRoom, selectedItem.ItemPrefab.GetComponent<Item>());
        itemSelectorUI.Hide();
        if (GameManager.Instance.previousState == GameState.bossRoom)
        {
            GameManager.Instance.player.abilityController.UnlockAbility(true, GameManager.Instance.currentLevelIndex + 1);
        } else
        {
            GameManager.Instance.state = GameState.playing;
        }
    }

    private void HandleItemSelected(int index)
    {
        selectedItem = itemOptions.GetItemAtIndex(index);
    }
}
