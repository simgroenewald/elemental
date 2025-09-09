using UnityEngine;

[CreateAssetMenu(fileName = "AbilityDetailsSO", menuName = "Scriptable Objects/AbilityDetailsSO")]
public class AbilityDetailsSO : ScriptableObject
{
    [Header("Main")]
    public string abilityName;
    public Sprite icon;
    public float range;

    [Header("Base Ability")]
    public bool isBaseAbility;
    public bool isAutoAttack;

    [Header("Targetable Bools")]
    public bool isEnemyTargetable;
    public bool isDirectionTargetable;
    public bool isGroundTargetable;

    [Header("Ability Type Bools")]
    public bool isPassive;
    public bool isDamageOverTime;
    public bool isCritical;
    public bool isAreaOfEffect;
    public bool isHurt;

    [Header("Slowdown")]
    public float slowDownBuffer;

    [Header("Cooldown")]
    public float coolDownTime;
    public bool isCoolingDown;

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
    public ProjectileDetailsSO abilityProjectileDetails;
    public float projectileSpeed;

    // Updatable variables
    [HideInInspector] public float _damage;
    [HideInInspector] public float _projectileSpeed;
    [HideInInspector] public float _coolDownTime;
    [HideInInspector] public float _critChance;
    [HideInInspector] public float _range;
    [HideInInspector] public float _manaCost;

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
