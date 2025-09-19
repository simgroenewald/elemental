using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
    // Room settings
    public const float fadeInTime = 0.5f;


    // Player Animator parameters
    public static int posTargetLeft = Animator.StringToHash("posTargetLeft");
    public static int posTargetRight = Animator.StringToHash("posTargetRight");
    public static int isIdle = Animator.StringToHash("isIdle");
    public static int isMoving = Animator.StringToHash("isMoving");
    public static int isAttacking = Animator.StringToHash("isAttacking");
    public static int isAttacking2 = Animator.StringToHash("isAttacking2");
    public static int isDying = Animator.StringToHash("isDying");
    public static int isTriggered = Animator.StringToHash("isTriggered");
    public static int isHurting = Animator.StringToHash("isHurting");

    public static float baseSpeedForPlayerAnimations = 2f;

    public const string playerTag = "Player";

    public const float useTargetAngleDistance = 0.5f;

    // Audio
    public const float musicFadeOutTime = 0.5f;
    public const float musicFadeInTime = 0.5f;


}
