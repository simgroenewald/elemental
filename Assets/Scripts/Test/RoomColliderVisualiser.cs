using UnityEngine;

[ExecuteInEditMode]
public class RoomColliderVisualiser : MonoBehaviour
{

    private void OnDrawGizmos()
    {
        BoxCollider2D[] colliders = GetComponents<BoxCollider2D>();
        foreach (var col in colliders)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
        }
    }
}