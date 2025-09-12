using UnityEngine;

public interface ICastable
{
    void InitialiseProjectile(
        ProjectileDetailsSO projectileDetails,
        Ability ability,
        float aimAngle,
        float castPointAngle,
        Vector3 targetDirectionVector,
        Character characterCaster,
        Character characterTarget,
        bool overrideProjectileMovement = false
        );

    void Cast();

    GameObject GetGameObject();

    void InitialiseProjectile(ProjectileDetailsSO currentProjectile, Ability ability, Character characterCaster, Character characterTarget);
}
