using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Player))]
[DisallowMultipleComponent]
public class AnimatePlayer : MonoBehaviour
{
    private Player player;

    private void Awake()
    {
        // Load components
        player = GetComponent<Player>();
    }

    private void Start()
    {
        GameEventManager.Instance.movementEvents.OnMoveByVelocity += HandleMoveByVelocityAnimation;
        //movementEvents.OnMoveByPosition += HandleMoveByPostionanimation;
        GameEventManager.Instance.movementEvents.OnIdle += HandleIdleAnimation;
    }

    private void OnDisable()
    {
        GameEventManager.Instance.movementEvents.OnMoveByVelocity -= HandleMoveByVelocityAnimation;
        //movementEvents.OnMoveByPosition -= HandleMoveByPostionanimation;
        GameEventManager.Instance.movementEvents.OnIdle -= HandleIdleAnimation;
    }

    private void HandleMoveByVelocityAnimation(Vector2 direction, float speed)
    {
        InitializeDirectionParameters();
        SetMovementDirectionParameters(direction);
        SetMovementAnimationParameters();
    }

    private void HandleIdleAnimation()
    {
        SetIdleAnimationParameters();
    }

    private void InitializeDirectionParameters()
    {
        player.animator.SetBool(Settings.posTargetUp, false);
        player.animator.SetBool(Settings.posTargetDown, false);
        player.animator.SetBool(Settings.posTargetRight, false);
        player.animator.SetBool(Settings.posTargetLeft, false);
        player.animator.SetBool(Settings.posTargetUpLeft, false);
        player.animator.SetBool(Settings.posTargetUpRight, false);
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

    private void SetMovementDirectionParameters(Vector2 direction)
    {

        // Decide dominant direction
        if (direction == Vector2.zero)
            return;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            // Horizontal movement dominates
            if (direction.x > 0)
                player.animator.SetBool(Settings.posTargetRight, true);
            else
                player.animator.SetBool(Settings.posTargetLeft, true);
        }
        else
        {
            // Vertical movement dominates
            if (direction.y > 0)
                player.animator.SetBool(Settings.posTargetUp, true);
            else
                player.animator.SetBool(Settings.posTargetDown, true);
        }
    }

}
