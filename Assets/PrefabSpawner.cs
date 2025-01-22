using UnityEngine;

public class PrefabSpawner : MonoBehaviour
{
    [Header("Prefab Settings")]
    public GameObject prefab; // The prefab to instantiate.
    public Vector3 spawnPosition = Vector3.zero; // The position where the prefab will be instantiated.
    public Quaternion spawnRotation = Quaternion.identity; // The rotation for the prefab.
 [ContextMenu("Spawn Prefab at Mouse Position")]
    public void SpawnPrefab()
    {
        if (prefab != null)
        {
            // Instantiate the prefab at the specified position and rotation.
            GameObject instance = Instantiate(prefab, spawnPosition, spawnRotation);

            // Optionally log the result.
            Debug.Log($"Spawned prefab: {prefab.name} at {spawnPosition}");
        }
        else
        {
            Debug.LogError("Prefab is not assigned. Please assign a prefab to spawn.");
        }
    }
}
