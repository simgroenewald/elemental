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
        GameEventManager.Instance.movementEvents.OnFaceLeft += HandleFaceLeftAnimation;
        GameEventManager.Instance.movementEvents.OnFaceRight += HandleFaceRightAnimation;
        GameEventManager.Instance.movementEvents.OnMoveByPosition += HandleMovementAnimation;
        GameEventManager.Instance.movementEvents.OnIdle += HandleIdleAnimation;
        player.abilityEvents.OnCastAbility += HandleAbilityCastAnimation;
    }

    private void OnDisable()
    {
        GameEventManager.Instance.movementEvents.OnFaceLeft -= HandleFaceLeftAnimation;
        GameEventManager.Instance.movementEvents.OnFaceRight -= HandleFaceRightAnimation;
        GameEventManager.Instance.movementEvents.OnMoveByPosition -= HandleMovementAnimation;
        GameEventManager.Instance.movementEvents.OnIdle -= HandleIdleAnimation;
        player.abilityEvents.OnCastAbility -= HandleAbilityCastAnimation;
    }

    private void ResetAnimationBools()
    {
        player.animator.SetBool(Settings.isMoving, false);
        player.animator.SetBool(Settings.isIdle, false);
        player.animator.SetBool(Settings.isAttacking, false);
    }

    private void HandleFaceLeftAnimation()
    {
        player.animator.SetBool(Settings.posTargetRight, false);
        player.animator.SetBool(Settings.posTargetLeft, true);
    }

    private void HandleFaceRightAnimation()
    {
        player.animator.SetBool(Settings.posTargetRight, true);
        player.animator.SetBool(Settings.posTargetLeft, false);
    }

    private void HandleMovementAnimation(float speed)
    {
        ResetAnimationBools();
        player.animator.SetBool(Settings.isMoving, true);
    }

    private void HandleIdleAnimation()
    {
        ResetAnimationBools();
        player.animator.SetBool(Settings.isMoving, false);
        player.animator.SetBool(Settings.isIdle, true);
    }

    private void HandleAbilityCastAnimation()
    {
        ResetAnimationBools();
        player.animator.SetBool(Settings.isAttacking, true);
    }

}
