using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Player))]
[DisallowMultipleComponent]
public class AnimatePlayer : MonoBehaviour
{
    private Player player;
    private bool isMoving = false;
    private Vector3 lastPosition;
    [SerializeField] private float directionThreshold = 0.05f;

    private void Awake()
    {
        // Load components
        player = GetComponent<Player>();
    }

    private void Start()
    {
        GameEventManager.Instance.movementEvents.OnMoveByVelocity += HandleMoveByVelocityAnimation;
        GameEventManager.Instance.movementEvents.OnMoveByPosition += HandleMoveByPositionAnimation;
        GameEventManager.Instance.movementEvents.OnIdle += HandleIdleAnimation;
    }

    private void OnDisable()
    {
        GameEventManager.Instance.movementEvents.OnMoveByVelocity -= HandleMoveByVelocityAnimation;
        GameEventManager.Instance.movementEvents.OnMoveByPosition -= HandleMoveByPositionAnimation;
        GameEventManager.Instance.movementEvents.OnIdle -= HandleIdleAnimation;
    }

    private void HandleMoveByVelocityAnimation(Vector2 direction, float speed)
    {
        //InitializeDirectionParameters();
        SetMovementDirectionParameters_Velocity(direction);
        SetMovementAnimationParameters();
    }

    private void HandleMoveByPositionAnimation(Vector3 position)
    {
        //InitializeDirectionParameters();
        SetMovementDirectionParameters_Position(position);
    }

    private void HandleIdleAnimation()
    {
        SetIdleAnimationParameters();
    }

    private void InitialiseDirectionParameters()
    {
        player.animator.SetBool(Settings.posTargetRight, false);
        player.animator.SetBool(Settings.posTargetLeft, false);
    }

    private void SetMovementAnimationParameters()
    {
        player.animator.SetBool(Settings.isRunning, true);
        player.animator.SetBool(Settings.isIdle, false);
    }

    private void SetIdleAnimationParameters()
    {
        player.animator.SetBool(Settings.isRunning, false);
        player.animator.SetBool(Settings.isIdle, true);
    }

    private void SetMovementDirectionParameters_Velocity(Vector2 direction)
    {

        // Decide dominant direction
        if (direction == Vector2.zero)
            return;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            // Horizontal movement dominates
            if (direction.x > 0)
            {
                player.animator.SetBool(Settings.posTargetRight, true);
                player.animator.SetBool(Settings.posTargetLeft, false);
            }
            else
            {
                player.animator.SetBool(Settings.posTargetLeft, true);
                player.animator.SetBool(Settings.posTargetRight, false);
            }
        }
    }

    private void SetMovementDirectionParameters_Position(Vector3 position)
    {
        StartTracking(position);
    }

    private void StartTracking(Vector3 destination)
    {
        isMoving = true;
        lastPosition = transform.position;
        SetMovementAnimationParameters();
    }

    private void StopTracking()
    {
        isMoving = false;
        SetIdleAnimationParameters();
    }

    private void Update()
    {
        if (!isMoving || player.playerAgent.pathPending) return;

        Vector3 currentPosition = transform.position;
        Vector3 delta = currentPosition - lastPosition;

        if (currentPosition.x >= lastPosition.x)
        {
            player.animator.SetBool(Settings.posTargetRight, true);
            player.animator.SetBool(Settings.posTargetLeft, false);
        }
        else if (currentPosition.x < lastPosition.x)
        {
            player.animator.SetBool(Settings.posTargetLeft, true);
            player.animator.SetBool(Settings.posTargetRight, false);
        }

        lastPosition = currentPosition;

        // Optional: Stop animation early if somehow movement stops unexpectedly
        if (player.playerAgent.remainingDistance <= player.playerAgent.stoppingDistance && player.playerAgent.velocity.sqrMagnitude < 0.01f)
        {
            StopTracking();
        }
    }

}
