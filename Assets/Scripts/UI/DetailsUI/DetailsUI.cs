using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DetailsUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] TMP_Text descName;
    [SerializeField] TMP_Text description;
    [SerializeField] GameObject detailsGO;

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
            detailsGO.SetActive(false);
        }
    }

    public void ResetDetails()
    {
        image.gameObject.SetActive(false);
        descName.text = string.Empty;
        description.text = string.Empty;
        detailsGO.SetActive(false);
    }

    public void SetDetails(Sprite sprite, string name, string description)
    {
        image.gameObject.SetActive(true);
        image.sprite = sprite;
        descName.text = name;
        this.description.text = description;
        detailsGO.SetActive(true);
    }

    // Can potentially just be replaced with SetItemDetails
    internal void UpdateDetails(Sprite itemImage, string name, string description)
    {
        SetDetails(itemImage, name, description);
    }
}
