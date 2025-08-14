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
        GameEventManager.Instance.movementEvents.OnIdle += HandleIdle;
        GameEventManager.Instance.movementEvents.OnAttack += HandleAttack;
    }

    private void OnDisable()
    {
        // Unsubscribe from movement event
        GameEventManager.Instance.movementEvents.OnIdle -= HandleIdle;
        GameEventManager.Instance.movementEvents.OnAttack -= HandleAttack;
    }

    private void HandleIdle()
    {
        rigidBody2D.linearVelocity = Vector2.zero;
    }

    private void HandleAttack()
    {
        rigidBody2D.linearVelocity = Vector2.zero;
    }

}
