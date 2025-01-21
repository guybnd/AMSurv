using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public CharacterStats Stats;

    void Start()
    {
        Stats = new CharacterStats();
        Stats.AddStat("Life", 50);
        Stats.AddStat("PhysicalDamage", 15);
    }

    public void TakeDamage(float amount)
    {
        var health = Stats.GetStat("Life");
        health.BaseValue -= amount;

        if (health.BaseValue <= 0)
            Die();
    }

    private void Die()
    {
        Debug.Log("Enemy defeated!");
        Destroy(gameObject);
    }
}
