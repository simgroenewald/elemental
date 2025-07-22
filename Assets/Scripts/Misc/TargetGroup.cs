using System;
using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineTargetGroup))]
public class TargetGroup : MonoBehaviour
{
    private CinemachineTargetGroup targetGroup;

    private void Awake()
    {
        targetGroup = GetComponent<CinemachineTargetGroup>();
    }

    private void Start()
    {
        SetTargetGroup();
    }

    private void SetTargetGroup()
    {
        Transform playerTransform = GameManager.Instance.GetPlayer().transform;

        targetGroup.RemoveMember(playerTransform);
        targetGroup.AddMember(playerTransform, weight: 1f, radius: 1f);
    }
}
