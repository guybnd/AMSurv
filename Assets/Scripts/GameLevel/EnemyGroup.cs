using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyGroup", menuName = "EnemyGroup")]
public class EnemyGroup : ScriptableObject
{
    public GameObject enemyPrefab;
    public int groupSize = 3;
    public int totalCount = 50;
}
