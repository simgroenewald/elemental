using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [field: SerializeField]
    public ItemSO ItemSO { get; private set; }
    [field: SerializeField]
    public int Quantity { get; set; } = 1;

    [SerializeField]
    private float duration = 0.3f;

    private bool pickupLocked;

    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = ItemSO.ItemImage;
    }

    public void DestroyItem()
    {
        GetComponent<Collider2D>().enabled = false;
        StartCoroutine(AnimateItemCollect());
    }

    private IEnumerator AnimateItemCollect()
    {
        Vector3 startScale = transform.localScale;
        Vector3 endScale = Vector3.zero;
        float currentTime = 0;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, endScale, currentTime / duration);
            yield return null;
        }
        Destroy(gameObject);
    }

    internal bool TryBeginPickup()
    {
        if (pickupLocked) 
            return false;

        pickupLocked = true;
        SetCollidersEnabled(false);  // prevent re-entry this frame
        return true;
    }

    private void SetCollidersEnabled(bool enabled)
    {
        var collider = GetComponent<Collider2D>();
        if (collider) collider.enabled = enabled;
    }
}
