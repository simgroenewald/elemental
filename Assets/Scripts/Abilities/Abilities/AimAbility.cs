using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AimAbilityEvent))]
[DisallowMultipleComponent]
public class AimAbility : MonoBehaviour
{

    private AimAbilityEvent aimAbilityEvent;
    [SerializeField] private Transform abilityRotationPointTransfrom;

    private void Awake()
    {
        aimAbilityEvent = GetComponent<AimAbilityEvent>();
    }

    private void OnEnable()
    {
        aimAbilityEvent.OnAbilityAim += OnAbilityAim;
    }

    private void OnDisable()
    {
        aimAbilityEvent.OnAbilityAim -= OnAbilityAim;
    }

    private void OnAbilityAim(AimAbilityEvent aimAbilityEvent, AimAbilityEventArgs aimAbilityEventArgs)
    {
        Aim(aimAbilityEventArgs.direction, aimAbilityEventArgs.aimAngle);
    }

    private void Aim(TargetDirection direction, float aimAngle)
    {
        abilityRotationPointTransfrom.eulerAngles = new Vector3(0f, 0f, aimAngle);

        switch (direction)
        {
            case TargetDirection.Left:
                abilityRotationPointTransfrom.localScale = new Vector3(1f, -1f, 0f);
                break;
            case TargetDirection.Right:
                abilityRotationPointTransfrom.localScale = new Vector3(1f, 1f, 0f);
                break;
        }

    }

}
