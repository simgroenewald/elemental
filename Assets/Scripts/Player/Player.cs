using System;
using UnityEngine;
using UnityEngine.Rendering;

[DisallowMultipleComponent]

[RequireComponent(typeof(SortingGroup))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]

public class Player : MonoBehaviour
{
    public CharacterDetailSO characterDetail;
    public Health health;
    public SpriteRenderer spriteRenderer;
    public Animator animator;

    private void Awake()
    {
        health = GetComponent<Health>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    public void Initialise(CharacterDetailSO characterDetail)
    {
        this.characterDetail = characterDetail;

        SetPlayerHealth();
    }

    private void SetPlayerHealth()
    {
        health.SetMaxHealth(characterDetail.health);
    }
}
