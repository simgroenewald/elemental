using System.ComponentModel;
using UnityEngine;

public enum StatType
{
    [Description("Health")]
    Health,
    [Description("Mana")]
    Mana,
    [Description("Physical Dmg Bonus")]
    PhysicalDamageBonus,
    [Description("Magical Dmg Bonus")]
    MagicalDamageBonus,
    [Description("Attack Speed")]
    AttackSpeed,
    [Description("Range Bonus")]
    RangeBonus,
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
    ProjectileSpeed,
    [Description("Evasion")]
    Evasion,
    [Description("Cooldown")]
    Cooldown
}
