using System;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class MeleeAbility : MonoBehaviour
{
    private Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MeleeAttack();
        }
    }

    private void MeleeAttack()
    {
        // Play an attack animation
        player.abilityEvents.RaiseMeleeAttackEvent();
        // Detect enemies in range of the attack

        // Damage enemy
    }

    private void EndAttack()
    {
        // Play an attack animation
        player.abilityEvents.RaiseMeleeEndAttackEvent();
        // Detect enemies in range of the attack

        // Damage enemy
    }
}
