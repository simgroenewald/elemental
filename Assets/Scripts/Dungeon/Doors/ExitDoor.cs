using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    [SerializeField] private Doorway doorway;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StaticEventHandler.CallRoomExitedEvent(doorway.room);
        }
    }
}
