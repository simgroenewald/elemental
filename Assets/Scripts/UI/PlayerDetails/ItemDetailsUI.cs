using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDetailsUI : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] TMP_Text itemName;
    [SerializeField] TMP_Text itemDescription;

    public void Awake()
    {
        ResetItemDetails();
    }

    public void ResetItemDetails()
    {
        itemImage.gameObject.SetActive(false);
        itemName.text = string.Empty;
        itemDescription.text = string.Empty;
    }

    public void SetItemDetails(Sprite sprite, string name, string description)
    {
        itemImage.gameObject.SetActive(true);
        itemImage.sprite = sprite;
        itemName.text = name;
        itemDescription.text = description;
    }

    // Can potentially just be replaced with SetItemDetails
    internal void UpdateItemDetails(Sprite itemImage, string name, string description)
    {
        SetItemDetails(itemImage, name, description);
    }
}
