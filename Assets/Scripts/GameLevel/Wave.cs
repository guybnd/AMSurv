using UnityEngine;

[CreateAssetMenu(fileName = "NewWave", menuName = "Wave")]
public class Wave : ScriptableObject
{
    public EnemyGroup[] enemyGroups;
    public float duration = 60f;
    public float groupSpawnDelay = 0.5f;
}
