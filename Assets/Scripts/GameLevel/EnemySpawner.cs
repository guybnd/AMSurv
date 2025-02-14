using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Wave[] waves;
    public float startDelay = 1f;
    public float waveDelay = 1f;
    public float spawnRadius = 20f;
    private Transform player;
    private Camera mainCamera;
    private int currentWaveIndex = 0;
    private bool isSpawning = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        mainCamera = Camera.main;
        StartCoroutine(StartSpawning());
    }

    IEnumerator StartSpawning()
    {
        yield return new WaitForSeconds(startDelay);

        while (currentWaveIndex < waves.Length)
        {
            if (!isSpawning)
            {
                isSpawning = true;
                StartCoroutine(SpawnWave(waves[currentWaveIndex]));
            }
            yield return new WaitForSeconds(waveDelay);
            yield return new WaitUntil(() => !isSpawning);
            currentWaveIndex++;
        }
    }

    IEnumerator SpawnWave(Wave wave)
    {
        foreach (EnemyGroup enemyGroup in wave.enemyGroups)
        {
            for (int i = 0; i < enemyGroup.totalCount; i += enemyGroup.groupSize)
            {
                SpawnEnemyGroup(enemyGroup);
                yield return new WaitForSeconds(wave.groupSpawnDelay);
            }
        }
        yield return new WaitForSeconds(wave.duration);
        isSpawning = false;
    }

    void SpawnEnemyGroup(EnemyGroup enemyGroup)
    {
        for (int i = 0; i < enemyGroup.groupSize; i++)
        {
            Vector2 spawnPosition = GetSpawnPosition();
            Instantiate(enemyGroup.enemyPrefab, spawnPosition, Quaternion.identity);
        }
    }

    Vector2 GetSpawnPosition()
    {
        Vector2 position;
        // Generate a random position on the unit circle, then scale it by the spawnRadius
        Vector2 randomCircle = Random.insideUnitCircle.normalized * spawnRadius;
        position = (Vector2)player.position + randomCircle;
        return position;
    }

    void OnValidate()
    {
        foreach (Wave wave in waves)
        {
            if (wave != null && wave.enemyGroups == null)
            {
                wave.enemyGroups = new EnemyGroup[0];
            }
        }
    }
}
