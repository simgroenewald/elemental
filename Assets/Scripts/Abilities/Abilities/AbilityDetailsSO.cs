using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AbilityDetailsSO", menuName = "Scriptable Objects/AbilityDetailsSO")]
public class AbilityDetailsSO : ScriptableObject
{
    [Header("Main")]
    public string abilityName;
    public string description;
    public Sprite icon;
    public string attackAnimationTrigger;
    public float range;

    [Header("Base Ability")]
    public bool isBaseAbility;
    public bool isAutoAttack;

    [Header("Targetable Bools")]
    public bool isEnemyTargetable;
    public bool isDirectionTargetable;
    public bool isGroundTargetable;
    public bool isMultiTarget;

    [Header("Ability Type Bools")]
    public bool isPassive;
    public bool isDamageOverTime;
    public bool isCritical;
    public bool isAreaOfEffect;
    public bool isHurt;
    public bool isCastAtTarget;
    public bool isSelfEffect;

    [Header("Slowdown")]
    public float slowDownBuffer;

    [Header("Cooldown")]
    public bool hasCooldown;
    public float coolDownTime;

    [Header("Effect Time")]
    public bool hasEffectTime;
    public float effectTime;

    [Header("Count")]
    public bool hasCountLimit;
    public float countLimit;

    [Header("Attack Damage")]
    public float damage;
    public bool isMagical;
    public float critChance;

    [Header("Mana")]
    public float manaCost;

    [Header("Ranged Attack")]
    public bool isRanged;
    public Vector3 abilityCastPositionLeft;
    public Vector3 abilityCastPositionRight;
    public Vector3 castPositionOffset;
    public ProjectileDetailsSO abilityProjectileDetails;
    public float projectileSpeed;

    [Header("Modifiers")]
    [SerializeField]
    public List<PassiveModifierData> modifierData = new List<PassiveModifierData>();
    public List<ModifierData> instantModifierData = new List<ModifierData>();

    // Updatable variables
    [HideInInspector] public float _damage;
    [HideInInspector] public float _projectileSpeed;
    [HideInInspector] public float _coolDownTime;
    [HideInInspector] public float _critChance;
    [HideInInspector] public float _range;
    [HideInInspector] public float _manaCost;

    //Trigger key
    public KeyCode triggerKey;

    private void OnEnable()
    {
        _damage = damage;
        _coolDownTime = coolDownTime;
        _manaCost = manaCost;
        _range = range;
        _critChance = critChance;
        if (isRanged)
        {
            _projectileSpeed = projectileSpeed;
        }
    }
}
