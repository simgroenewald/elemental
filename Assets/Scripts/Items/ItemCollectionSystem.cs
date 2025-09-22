using UnityEngine;

public class ItemCollectionSystem : MonoBehaviour
{
    [SerializeField] private BackpackSO backpack;
    [SerializeField] private SoundEffectSO itemCollectSound;
    [SerializeField] private PolygonCollider2D polygonCollider;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent<Item>(out var item)) return;
        if (!item.TryBeginPickup()) return;
        AddItemToBackpack(item);
    }

    public void AddItemToBackpack(Item item)
    {

        if (item != null)
        {
            int remainder = backpack.AddItem(item.ItemSO, item.Quantity);
            if (remainder == -1)
            {
                item.EnablePickup();
            }
            if (remainder == 0)
            {
                // Prevent double-trigger by locking & disabling colliders immediately
                item.DestroyItem();
                SoundEffectManager.Instance.PlaySoundEffect(itemCollectSound);
            }
            else
            {
                item.Quantity = remainder;
                item.EnablePickup();
                SoundEffectManager.Instance.PlaySoundEffect(itemCollectSound);
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
