using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnerController : MonoBehaviour {

    private GameObject enemyPrefab;
    private Transform enemyBin;

    List<EnemyController> enemies = new List<EnemyController>();

    Vector2[][] spawnSets;
    private void InitSpawnSets() {
        float orthoSize = Camera.main.orthographicSize;
        float aspectRatio = Camera.main.aspect;
        float worldHeight = orthoSize * 2;
        float worldWidth = worldHeight * aspectRatio;
        float expander = 8;
        
        //Up
        float gap = 10;
        spawnSets = new Vector2[8][];
        List<Vector2> curSet = new List<Vector2>();
        curSet.Add(new Vector2(0 , orthoSize + expander));
        curSet.Add(new Vector2(-1 * gap , orthoSize + expander));
        curSet.Add(new Vector2(1 * gap, orthoSize + expander));
        curSet.Add(new Vector2(-2 * gap, orthoSize + expander));
        curSet.Add(new Vector2(-2 * gap, orthoSize + expander));
        spawnSets[0] = curSet.ToArray();

        //down
        curSet = new List<Vector2>();
        curSet.Add(new Vector2(0, -orthoSize - expander));
        curSet.Add(new Vector2(-1 * gap, - orthoSize - expander));
        curSet.Add(new Vector2(1 * gap, -orthoSize - expander));
        curSet.Add(new Vector2(-2 * gap, -orthoSize - expander));
        curSet.Add(new Vector2(-2 * gap, -orthoSize - expander));
        spawnSets[1] = curSet.ToArray();

        //left
        curSet = new List<Vector2>();
        curSet.Add(new Vector2(-worldWidth / 2 - expander, 0));
        curSet.Add(new Vector2(-worldWidth / 2 - expander, -1 * expander));
        curSet.Add(new Vector2(-worldWidth / 2 - expander, 1 * expander));
        curSet.Add(new Vector2(-worldWidth / 2 - expander, -2 * expander));
        curSet.Add(new Vector2(-worldWidth / 2 - expander, -2 * expander));
        spawnSets[2] = curSet.ToArray();

        //right
        curSet = new List<Vector2>();
        curSet.Add(new Vector2(worldWidth / 2 + expander, 0));
        curSet.Add(new Vector2(worldWidth / 2 + expander, -1 * expander));
        curSet.Add(new Vector2(worldWidth / 2 + expander, 1 * expander));
        curSet.Add(new Vector2(worldWidth / 2 + expander, -2 * expander));
        curSet.Add(new Vector2(worldWidth / 2 + expander, -2 * expander));
        spawnSets[3] = curSet.ToArray();
    }

    private void Awake() {
        enemyPrefab = Resources.Load<GameObject>("Prefabs/Enemy");

        enemyBin = new GameObject("EnemyBin").transform;
        enemyBin.parent = GameObject.Find("Environment").transform;

        InitSpawnSets();
    }


    void Start() {
        StartCoroutine(spawnCycle());
    }

    IEnumerator spawnCycle() {
        int wave = 1;
        yield return new WaitForSeconds(1f);
        GameManager.isPlaying = true;//temp
        while(GameManager.isPlaying) {
            int rnd = Random.Range(0, 4);
            Vector3 direction = -spawnSets[rnd][0].normalized;
            for(int i = 0; i < wave && i < spawnSets[rnd].Length ; i++) {
                Spawn(spawnSets[rnd][i], direction);
            }
            yield return new WaitForSeconds(3f);
            wave++;
        }
    }

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
        return enemies.LastOrDefault();
    }
}
