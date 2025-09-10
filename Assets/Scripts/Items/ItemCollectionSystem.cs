using UnityEngine;

public class ItemCollectionSystem : MonoBehaviour
{
    [SerializeField]
    private BackpackSO backpack;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent<Item>(out var item)) return;

        // Prevent double-trigger by locking & disabling colliders immediately
        if (!item.TryBeginPickup()) return;

        AddItemToBackpack(item);
    }

    public void AddItemToBackpack(Item item)
    {
        if (item != null)
        {
            int remainder = backpack.AddItem(item.ItemSO, item.Quantity);
            if (remainder == 0)
            {
                item.DestroyItem();
            }
            else
            {
                item.Quantity = remainder;
            }
        }
    }

    public void AddItemToBackpack(ItemSO item)
    {
        if (item != null)
        {
            backpack.AddItem(item, 1);
        }
    }
}
