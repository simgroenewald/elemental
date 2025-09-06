using System.ComponentModel;
using UnityEngine;

public enum StatType
{
    [Description("Health")]
    Health,
    [Description("Mana")]
    Mana,
    [Description("Physical Damage")]
    PhysicalDamage,
    [Description("Magical Damage Bonus")]
    MagicalDamageBonus,
    [Description("Attack Speed")]
    AttackSpeed,
    [Description("Range")]
    Range,
    [Description("Line Of Sight")]
    LineOfSight,
    [Description("Movement Speed")]
    MovementSpeed,
    [Description("Armor")]
    Armor,
    [Description("Health Regen")]
    HealthRegen,
    [Description("Mana Regen")]
    ManaRegen,
    [Description("Crit Chance")]
    CritChance,
    [Description("Projectile Speed")]
    ProjectileSpeed
}
