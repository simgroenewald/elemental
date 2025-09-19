using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Canvas))]
public class UISortFollow : MonoBehaviour
{
    public SortingGroup sourceGroup;     // assign your player's SortingGroup
    public SpriteRenderer sourceSprite;  // fallback if no group
    public int orderOffset = 10;         // put UI above the sprite

    Canvas cv;

    void Awake()
    {
        cv = GetComponent<Canvas>();
        cv.renderMode = RenderMode.WorldSpace;
        if (!sourceGroup) sourceGroup = GetComponentInParent<SortingGroup>();
        if (!sourceGroup && !sourceSprite) sourceSprite = GetComponentInParent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        if (sourceGroup)
        {
            cv.sortingLayerID = sourceGroup.sortingLayerID;
            cv.sortingOrder = sourceGroup.sortingOrder + orderOffset;
        }
        else if (sourceSprite)
        {
            cv.sortingLayerID = sourceSprite.sortingLayerID;
            cv.sortingOrder = sourceSprite.sortingOrder + orderOffset;
        }
    }
}