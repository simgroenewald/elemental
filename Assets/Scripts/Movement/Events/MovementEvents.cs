using System;
using UnityEngine;

[CreateAssetMenu(fileName = "MovementEvents", menuName = "Events/MovementEvents")]
public class MovementEvents : ScriptableObject
{
    public event Action<Vector2, float> OnMoveByVelocity;
    public event Action<Vector3, Vector3, float> OnMoveByPosition;
    public event Action OnIdle;

    public void RaiseMoveByVelocity(Vector2 direction, float speed)
    {
        OnMoveByVelocity?.Invoke(direction, speed);
    }

    public void RaiseMoveByPosition(Vector3 target, Vector3 currentPosition, float speed)
    {
        OnMoveByPosition?.Invoke(target, currentPosition, speed);
    }

    public void RaiseIdle()
    {
        OnIdle?.Invoke();
    }
}