using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class WaveManager : MonoBehaviour
{
    public WaveLevelUIManager waveUI; // Reference to the Wave UI manager
    public Transform[] spawnPoints; // Array of spawn points for slimes
    public Transform[] bossSpawnPoints; // Array of spawn points for the boss
    public GameObject[] slimePrefabs; // Array of different slime prefabs
    public GameObject bossPrefab; // Boss prefab
    public float spawnDelay = 1f; // Delay between spawns in seconds
    public GameObject bossHealthBarPrefab; // Prefab for the boss health bar

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
            1 => 1,
            2 => 1,
            3 => 1,
            4 => 1,
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

    // Spawn the boss at a specific boss spawn point
   private void SpawnBoss()
    {
        if (bossSpawnPoints.Length == 0)
        {
            Debug.LogError("No boss spawn points assigned!");
            return;
        }

        // Spawn the boss
        Transform randomBossSpawnPoint = bossSpawnPoints[Random.Range(0, bossSpawnPoints.Length)];
        GameObject boss = Instantiate(bossPrefab, randomBossSpawnPoint.position, Quaternion.identity);
        activeEnemies.Add(boss); // Add the boss to the active enemies list

        // แสดง Health Bar
        GameObject healthBarInstance = Instantiate(bossHealthBarPrefab);
        healthBarInstance.transform.SetParent(GameObject.Find("UIManeger").transform, false); // ทำให้ Health Bar อยู่ใน Canvas
        healthBarInstance.SetActive(true); // เปิดการแสดง Health Bar

        // ลิงก์ HealthBar กับ Boss
        HealthBarBoss healthBarScript = healthBarInstance.GetComponent<HealthBarBoss>();
        DragonAi bossScript = boss.GetComponent<DragonAi>();
        if (healthBarScript != null && bossScript != null)
        {
            healthBarScript.Boss = bossScript; // ลิงก์ DragonAi กับ Health Bar
        }

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
