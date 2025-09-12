using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDetailsUI : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] TMP_Text itemName;
    [SerializeField] TMP_Text itemDescription;
    [SerializeField] GameObject itemDetailsGO;

    public void Start()
    {
        //ResetItemDetails();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;
            itemDetailsGO.SetActive(false);
        }
    }

    public void ResetItemDetails()
    {
        itemImage.gameObject.SetActive(false);
        itemName.text = string.Empty;
        itemDescription.text = string.Empty;
        itemDetailsGO.SetActive(false);
    }

    public void SetItemDetails(Sprite sprite, string name, string description)
    {
        itemImage.gameObject.SetActive(true);
        itemImage.sprite = sprite;
        itemName.text = name;
        itemDescription.text = description;
        itemDetailsGO.SetActive(true);
    }

    // Can potentially just be replaced with SetItemDetails
    internal void UpdateItemDetails(Sprite itemImage, string name, string description)
    {
        SetItemDetails(itemImage, name, description);
    }
}
