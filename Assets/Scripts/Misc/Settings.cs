using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
    // Room settings
    public const float fadeInTime = 0.5f;


    // Player Animator parameters
    public static int posTargetUp = Animator.StringToHash("posTargetUp");
    public static int posTargetLeft = Animator.StringToHash("posTargetLeft");
    public static int posTargetRight = Animator.StringToHash("posTargetRight");
    public static int posTargetDown = Animator.StringToHash("posTargetDown");
    public static int posTargetUpLeft = Animator.StringToHash("posTargetUpLeft");
    public static int posTargetUpRight = Animator.StringToHash("posTargetUpRight");
    public static int isIdle = Animator.StringToHash("isIdle");
    public static int isRunning = Animator.StringToHash("isRunning");

    public static float baseSpeedForPlayerAnimations = 12f;

    public const string playerTag = "Player";

    public const float useTargetAngleDistance = 3.5f;

}
