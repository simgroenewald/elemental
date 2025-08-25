using UnityEngine;

public class ItemCollectionSystem : MonoBehaviour
{
    [SerializeField]
    private BackpackSO backpack;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Item item = collision.GetComponent<Item>();
        if (item != null)
        {
            int remainder = backpack.AddItem(item.ItemSO, item.Quantity);
            if (remainder == 0)
                item.DestroyItem();
            else
                item.Quantity = remainder;
        }
    }
}
