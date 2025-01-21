using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // Reference to the enemy prefab
    public Transform player; // Reference to the player
    public float spawnRadiusMin = 5f; // Minimum distance from the player to spawn
    public float spawnRadiusMax = 10f; // Maximum distance from the player to spawn
    public float spawnInterval = 2f; // Time between spawns

    void Start()
    {
        // Start spawning enemies
        InvokeRepeating(nameof(SpawnEnemy), 1f, spawnInterval);
    }

    void SpawnEnemy()
    {
        if (player == null) return;

        // Generate a random angle in radians
        float angle = Random.Range(0f, 2f * Mathf.PI);

        // Generate a random distance within the spawn range
        float distance = Random.Range(spawnRadiusMin, spawnRadiusMax);

        // Calculate the spawn position relative to the player
        Vector2 spawnPosition = (Vector2)player.position + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;

        // Instantiate the enemy at the random position
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
}
