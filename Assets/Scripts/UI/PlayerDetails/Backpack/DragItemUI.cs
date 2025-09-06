using UnityEngine;

public class DragItemUI : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Camera mainCam;

    [SerializeField] private ItemUI itemUI;

    private void Awake()
    {
        canvas = transform.root.GetComponent<Canvas>();
        mainCam = Camera.main;
        itemUI = GetComponentInChildren<ItemUI>();
    }

    public void SetItemSlot(Sprite sprite, int quantity)
    {
        itemUI.SetItemSlot(sprite, quantity);
    }

    private void Update()
    {
        transform.position = Input.mousePosition;
    }

    public void Toggle(bool val)
    {
        gameObject.SetActive(val);
    }
}
