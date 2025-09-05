using UnityEngine;

public class EntryDoor : MonoBehaviour
{
    [SerializeField] private Doorway doorway;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StaticEventHandler.CallRoomEnteredEvent(doorway.room);
        }
    }
}
