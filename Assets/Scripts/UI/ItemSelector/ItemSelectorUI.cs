using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemSelectorUI : MonoBehaviour
{
    [SerializeField] private ItemOptionUI itemOptionPrefab;
    [SerializeField] private GameObject itemSelectorGO;
    [SerializeField] private RectTransform optionsPanel;
    [SerializeField] SoundEffectSO clickSound;
    List<ItemOptionUI> itemOptions = new List<ItemOptionUI>();

    public event Action<int> OnItemSelected;
    public event Action OnContinue;

    private void Awake()
    {
        itemSelectorGO.SetActive(false);
    }

    public void Show()
    {
        itemSelectorGO.SetActive(true);
    }

    public void Hide()
    {
        ResetAllItems();
        DeselectAllItems();
        itemSelectorGO.SetActive(false);
    }

    public void Initialise(int size)
    {
        for (int i = 0; i < size; i++)
        {
            ItemOptionUI itemOptionUI = Instantiate(itemOptionPrefab, optionsPanel);
            itemOptionUI.OnItemClicked += HandleItemSelected;
            itemOptions.Add(itemOptionUI);
        }
    }

    public void UpdateItemOption(int index, Sprite image, string itemName, string details)
    {
        if (itemOptions.Count > index)
        {
            itemOptions[index].SetItemOption(image, itemName, details);
        }
        DeselectAllItems();
    }

    private void HandleItemSelected(ItemOptionUI itemUI)
    {
        int index = itemOptions.IndexOf(itemUI);
        if (index == -1)
        {
            return;
        }
        DeselectAllItems();
        itemOptions[index].Select();
        OnItemSelected?.Invoke(index);
    }

    public void DeselectAllItems()
    {
        foreach (ItemOptionUI itemOption in itemOptions)
        {
            itemOption.Deselect();
        }
    }

    internal void ResetAllItems()
    {
        foreach (var item in itemOptions)
        {
            item.ResetSlot();
            item.Deselect();
        }
    }

    public void OnButtonClick()
    {
        SoundEffectManager.Instance.PlaySoundEffect(clickSound);
        OnContinue?.Invoke();
    }
}
