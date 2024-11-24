using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public WaveLevelUIManager waveUI; // Reference to the Wave UI manager
    public Transform[] spawnPoints; // Array of spawn points
    public GameObject[] slimePrefabs; // Array of different slime prefabs
    public GameObject bossPrefab; // Boss prefab
    public float spawnDelay = 1f; // Delay between spawns in seconds

    private int currentWave = 0; // Tracks the current wave
    private int maxWave = 5; // Maximum wave count
    private List<GameObject> activeEnemies = new List<GameObject>(); // Tracks active enemies

    void Start()
    {
        StartWave(); // Automatically start the first wave
    }

    // Start the wave
    void StartWave()
    {
        currentWave++; // Increment the wave number
        waveUI.SetWave(currentWave); // Update the wave UI

        if (currentWave <= maxWave)
        {
            StartCoroutine(SpawnWave(currentWave));
        }
        else
        {
            Debug.Log("All waves completed!");
        }
    }

    // Coroutine to spawn slimes or boss
    private IEnumerator SpawnWave(int wave)
    {
        int slimeCount = 0;

        if (wave == 5)
        {
            // Boss wave
            Debug.Log("Boss wave started!");
            SpawnBoss();
        }
        else
        {
            // Calculate slime count based on wave
            slimeCount = CalculateSlimeCount(wave);

            Debug.Log($"Spawning {slimeCount} slimes for wave {wave}...");
            for (int i = 0; i < slimeCount; i++)
            {
                SpawnSlime();
                yield return new WaitForSeconds(spawnDelay); // Wait between spawns
            }
        }

        Debug.Log($"Wave {wave} started!");
    }

    // Calculate number of slimes per wave
    private int CalculateSlimeCount(int wave)
    {
        return wave switch
        {
            1 => 3,
            2 => 6,
            3 => 10,
            4 => 15,
            _ => 0
        };
    }

    // Spawn a random slime at a random spawn point
    private void SpawnSlime()
    {
        Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject randomSlime = Instantiate(slimePrefabs[Random.Range(0, slimePrefabs.Length)], randomSpawnPoint.position, Quaternion.identity);
        activeEnemies.Add(randomSlime); // Add the spawned slime to the active enemies list
    }

    // Spawn the boss at a random spawn point
    private void SpawnBoss()
    {
        Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject boss = Instantiate(bossPrefab, randomSpawnPoint.position, Quaternion.identity);
        activeEnemies.Add(boss); // Add the boss to the active enemies list
    }

    void Update()
    {
        // Check if all enemies are defeated
        CheckWaveClear();
    }

    // Check if all enemies are cleared
    private void CheckWaveClear()
    {
        // Remove null (destroyed) objects from the activeEnemies list
        activeEnemies.RemoveAll(enemy => enemy == null);

        if (activeEnemies.Count == 0 && currentWave < maxWave) // If all enemies are cleared and more waves remain
        {
            Debug.Log("Wave cleared! Starting next wave...");
            StartWave();
        }
    }
}
