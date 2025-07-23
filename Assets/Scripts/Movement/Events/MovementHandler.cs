using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody2D))]
[DisallowMultipleComponent]
public class MovementHandler : MonoBehaviour
{
    private Rigidbody2D rigidBody2D;

    private void Awake()
    {
        // Load components
        rigidBody2D = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // Subscribe to movement event
        GameEventManager.Instance.movementEvents.OnMoveByVelocity += HandleMovebyVelocity;
        GameEventManager.Instance.movementEvents.OnMoveByPosition += HandleMovebyPostion;
        GameEventManager.Instance.movementEvents.OnIdle += HandleIdle;
    }

    private void OnDisable()
    {
        // Unsubscribe from movement event
        GameEventManager.Instance.movementEvents.OnMoveByVelocity -= HandleMovebyVelocity;
        GameEventManager.Instance.movementEvents.OnMoveByPosition -= HandleMovebyPostion;
        GameEventManager.Instance.movementEvents.OnIdle -= HandleIdle;
    }

    private void HandleMovebyVelocity(Vector2 direction, float speed)
    {
        rigidBody2D.linearVelocity = direction * speed;
    }

    private void HandleMovebyPostion(Vector3 target, Vector3 currentPosition, float moveSpeed)
    {
        Vector2 unitVector = Vector3.Normalize(target - currentPosition);
        rigidBody2D.MovePosition(rigidBody2D.position + (unitVector * moveSpeed * Time.fixedDeltaTime));
    }

    private void HandleIdle()
    {
        rigidBody2D.linearVelocity = Vector2.zero;
    }

}
