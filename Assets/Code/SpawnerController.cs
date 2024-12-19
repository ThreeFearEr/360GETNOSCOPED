using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnerController : MonoBehaviour {

    [Header("Prefab Settings")]
    private GameObject enemyPrefab;

    [Header("Spawn Settings")]
    public float spawnInterval = 3f; // Interval between waves in seconds
    public float adjust = 1f; // Adjustment beyond the camera
    public float gap = 1f; // Distance between spawn points within a spawn set

    private List<(Vector2[], Vector2)> spawnSets = new List<(Vector2[], Vector2)>(); // List of spawn sets with directions

    private Transform enemyBin;
    List<EnemyController> enemies = new List<EnemyController>();

    public int Wave = 1;
    public int EnemiesPerSpawn = 1;

    private void Awake() {
        GameManager.isPlaying = true;
        enemyPrefab = Resources.Load<GameObject>("Prefabs/Enemy");

        enemyBin = new GameObject("EnemyBin").transform;
        enemyBin.parent = GameObject.Find("Environment").transform;

        InitializeSpawnPoints();
    }

    void Start() {
        InitializeSpawnPoints();
        StartCoroutine(SpawnWaveCycle());
    }

    void InitializeSpawnPoints() {
        Vector3 cameraPosition = Camera.main.transform.position;
        float cameraWidth = Camera.main.orthographicSize * Camera.main.aspect;
        float cameraHeight = Camera.main.orthographicSize;

        // Define spawn points for the 4 sides and 4 corners
        Vector2[] sides = new Vector2[4]
        {
            new Vector2(cameraPosition.x - cameraWidth - adjust, cameraPosition.y), // Left
            new Vector2(cameraPosition.x + cameraWidth + adjust, cameraPosition.y), // Right
            new Vector2(cameraPosition.x, cameraPosition.y + cameraHeight + adjust), // Top
            new Vector2(cameraPosition.x, cameraPosition.y - cameraHeight - adjust) // Bottom
        };

        Vector2[] corners = new Vector2[4]
        {
            new Vector2(cameraPosition.x - cameraWidth - adjust, cameraPosition.y + cameraHeight + adjust), // Top-left
            new Vector2(cameraPosition.x + cameraWidth + adjust, cameraPosition.y + cameraHeight + adjust), // Top-right
            new Vector2(cameraPosition.x - cameraWidth - adjust, cameraPosition.y - cameraHeight - adjust), // Bottom-left
            new Vector2(cameraPosition.x + cameraWidth + adjust, cameraPosition.y - cameraHeight - adjust) // Bottom-right
        };

        // Create spawn sets by expanding around each main spawn point
        foreach(var point in sides) {
            Vector2 direction = (Vector2.zero - point).normalized;
            spawnSets.Add((GenerateSpawnSet(point, direction), direction));
        }
        foreach(var point in corners) {
            Vector2 direction = (Vector2.zero - point).normalized;
            spawnSets.Add((GenerateSpawnSet(point, direction), direction));
        }
    }

    Vector2[] GenerateSpawnSet(Vector2 centerPoint, Vector2 direction) {
        Vector2 rotatedDirection = new Vector2(-direction.y, direction.x); // Rotate the direction by 90 degrees
        Vector2[] spawnSet = new Vector2[5];
        for(int i = 0; i < 5; i++) {
            int offset = (i % 2 == 0) ? -(i / 2) : (i / 2 + 1); // Alternate offsets: 0, -1, 1, -2, 2
            spawnSet[i] = centerPoint + rotatedDirection.normalized * offset * gap;
        }
        return spawnSet;
    }

    IEnumerator SpawnWaveCycle() {
        yield return new WaitForSeconds(1f);
        while(GameManager.isPlaying) {
            SpawnWave();
            yield return new WaitForSeconds(spawnInterval);
            Wave++;
            if(Mathf.Pow(2, EnemiesPerSpawn) <= Wave && EnemiesPerSpawn < 16) {
                EnemiesPerSpawn++;
            }
        }
    }

    void SpawnWave() {
        int enemiesLeft = EnemiesPerSpawn;
        while(enemiesLeft > 0) {
            // Choose a random spawn set
            var chosenSet = spawnSets[Random.Range(0, spawnSets.Count)];
            Vector2[] spawnPoints = chosenSet.Item1;
            Vector2 direction = chosenSet.Item2;

            // Determine how many enemies to spawn in this set
            int spawnCount = Mathf.Min(enemiesLeft, spawnPoints.Length);

            // Spawn enemies at random points within the chosen set
            HashSet<int> usedIndices = new HashSet<int>();
            for(int i = 0; i < spawnCount; i++) {
                int index;
                do {
                    index = Random.Range(0, spawnPoints.Length);
                } while(usedIndices.Contains(index));

                usedIndices.Add(index);
                Spawn(spawnPoints[index], direction);
                //GameObject enemy = Instantiate(enemyPrefab, spawnPoints[index], Quaternion.identity);
                // Set direction to the spawned object (example usage)
                // enemy.GetComponent<EnemyScript>().SetDirection(direction);
            }

            enemiesLeft -= spawnCount;
        }
    }


    //IEnumerator spawnCycle() {
    //    int wave = 1;
    //    yield return new WaitForSeconds(1f);
    //    GameManager.isPlaying = true;//temp
    //    while(GameManager.isPlaying) {
    //        int rnd = Random.Range(0, 4);
    //        Vector3 direction = -spawnSets[rnd][0].normalized;
    //        for(int i = 0; i < wave && i < spawnSets[rnd].Length ; i++) {
    //            Spawn(spawnSets[rnd][i], direction);
    //        }
    //        yield return new WaitForSeconds(3f);
    //        wave++;
    //    }
    //}


    public void Spawn(Vector3 position, Vector3 direction) {
        EnemyController curEnemy = GetActiveEnemy();
        curEnemy.transform.position = position;
        StartCoroutine(curEnemy.Born(direction));
    }

    private EnemyController GetActiveEnemy() {
        for(int i = 0; i < enemies.Count; i++) {
            if(enemies[i].gameObject.activeSelf) continue;
            return enemies[i];
        }
        enemies.Add(Instantiate(enemyPrefab).GetComponent<EnemyController>());    
        enemies.LastOrDefault().transform.parent = enemyBin;
        return enemies.LastOrDefault();
    }
}
