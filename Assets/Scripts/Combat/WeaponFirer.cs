using System.Collections.Generic;
using UnityEngine;

//public class WeaponFirer : MonoBehaviour
//{
//    [Header("References")]
//    [SerializeField] public Weapon equippedWeapon;      // Reference to the equipped weapon
//    [SerializeField] private CharacterStats playerStats;  // Reference to player stats (bonuses, etc.)
//    [SerializeField] private GameObject projectilePrefab; // Prefab for the projectile
//    [SerializeField] private Transform firePoint;         // Where the projectile spawns

//    [Header("Projectile Settings")]
//    [SerializeField] private float projectileSpeed = 10f;   // Base projectile speed
//    [SerializeField] private float defaultProjectileDuration = 1f; // Default projectile duration before modifiers

//    // Call this method (for example, on a mouse click) to fire the weapon.
//    public void FireWeapon()
//    {
//        // Get the mouse position in world space (assuming a 2D game).
//        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//        mousePos.z = 0f;
//        Vector2 baseDirection = ((Vector2)mousePos - (Vector2)firePoint.position).normalized;

//        // Determine the number of projectiles to fire and calculate spread.
//        int projectileCount = equippedWeapon.ProjectileAmount;
//        float spreadAngle = 5f; // degrees between projectiles
//        float totalSpread = (projectileCount - 1) * spreadAngle;
//        float startAngle = -totalSpread / 2f;

//        // Apply the player's projectile speed bonus.
//        // For example, if IncreasedProjectileSpeed is 20, then multiplier is 1 + (20/100) = 1.2.
//        float modifiedSpeed = projectileSpeed * (1f + playerStats.IncreasedProjectileSpeed / 100f);

//        // Compute the projectile duration using the player's increased duration bonus.
//        float modifiedDuration = defaultProjectileDuration * (1f + playerStats.IncreasedDuration / 100f);

//        // Calculate the damage (and crit status) for this attack.
//        AttackData attackData = ComputeAttackDamage();

//        // Fire each projectile in a spread.
//        for (int i = 0; i < projectileCount; i++)
//        {
//            float angleOffset = startAngle + i * spreadAngle;
//            Vector2 finalDirection = RotateVector(baseDirection, angleOffset);

//            // Instantiate the projectile.
//            GameObject projGO = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
//            Projectile projectile = projGO.GetComponent<Projectile>();
//            if (projectile != null)
//            {
//                // Pass in the modified projectile speed and duration.
//                projectile.Initialize(finalDirection, modifiedSpeed, attackData, modifiedDuration);
//            }
//        }
//    }

//    // Computes the attack damage based on the equipped weapon and player stats.
//    private AttackData ComputeAttackDamage()
//    {
//        AttackData data = new AttackData();

//        // For each damage type, roll a random value between min and max.
//        float physical = Random.Range(equippedWeapon.MinPhysicalDamage, equippedWeapon.MaxPhysicalDamage);
//        float fire = Random.Range(equippedWeapon.MinFireDamage, equippedWeapon.MaxFireDamage);
//        float cold = Random.Range(equippedWeapon.MinColdDamage, equippedWeapon.MaxColdDamage);
//        float lightning = Random.Range(equippedWeapon.MinLightningDamage, equippedWeapon.MaxLightningDamage);
//        float chaos = Random.Range(equippedWeapon.MinChaosDamage, equippedWeapon.MaxChaosDamage);

//        // Sum the damage from all types.
//        float totalDamage = physical + fire + cold + lightning + chaos;

//        // Apply the player's overall damage multiplier.
//        totalDamage *= playerStats.DamageMultiplier;

//        // Roll for a critical hit.
//        float totalCritChance = equippedWeapon.BaseCriticalChance + playerStats.CritChanceBonus;
//        bool isCrit = (Random.value < totalCritChance);
//        data.isCriticalHit = isCrit;
//        if (isCrit)
//        {
//            float totalCritMultiplier = equippedWeapon.CritMultiplier + playerStats.CritMultiplierBonus;
//            totalDamage *= totalCritMultiplier;
//        }
//        data.damage = totalDamage;

//        return data;
//    }

//    // Rotates a 2D vector by the given angle (in degrees).
//    private Vector2 RotateVector(Vector2 vector, float degrees)
//    {
//        float rad = degrees * Mathf.Deg2Rad;
//        float cos = Mathf.Cos(rad);
//        float sin = Mathf.Sin(rad);
//        return new Vector2(vector.x * cos - vector.y * sin, vector.x * sin + vector.y * cos);
//    }
//}

//// A simple class to hold attack data.
//public class AttackData
//{
//    public float damage;
//    public bool isCriticalHit;
//}
