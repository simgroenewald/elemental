using UnityEngine;

[CreateAssetMenu(fileName = "AbilityDetailsSO", menuName = "Scriptable Objects/AbilityDetailsSO")]
public class AbilityDetailsSO : ScriptableObject
{
    public string abilityName;
    public Sprite icon;

    public bool isAutoAttack;
    public bool isEnemyTargetable;
    public bool isDirectionTargetable;
    public bool isGroundTargetable;

    public bool isPassive;
    public bool isRanged;
    public bool isDamageOverTime;
    public bool isCritical;
    public bool isAreaOfEffect;
    public bool isHurt;
    public bool isCoolingDown;

    public Vector3 abilityCastPositionLeft;
    public Vector3 abilityCastPositionRight;
    public ProjectileDetailsSO abilityProjectileDetails;

    public float slowDownBuffer;

    public float coolDownTime;
    public float manaCost;
    public float magicalDamage;
    public float range;
}
