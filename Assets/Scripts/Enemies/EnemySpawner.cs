using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TODO: Object pool
/// </summary>
public class EnemySpawner : MonoBehaviour {
    public GameObject enemyPrefab;
    public List<Enemy> enemies;
    public List<PathNode> startNodes;

    [Range(0f, 10f)]
    public float spawnRatePerMinute;
    public float timeSinceLastSpawn;

    [Tooltip("Measured in seconds")]
    public float intialSpawnDelay;
    public bool isSpawning;

    [Range(0, 50)]
    public int maxNumEnemies;

    public float medianPathWalkTime;
    public float pathWalkTimeVariance;

    private void Awake() {
        if (startNodes.Count == 0) {
            Debug.LogWarning("No start spawn nodes defined in the enemy spawner. Did you forget to assign them?");
        }
    }

    // Update is called once per frame
    void Update() {
        if (isSpawning && enemies.Count < maxNumEnemies) {
            timeSinceLastSpawn += Time.deltaTime;

            if (timeSinceLastSpawn > spawnRatePerMinute * 60) {
                SpawnEnemy();
                timeSinceLastSpawn = 0f;
            }
        }
    }



    public void StartSpawning() {
        DespawnAllEnemies();
        isSpawning = true;
        timeSinceLastSpawn = Mathf.Clamp(spawnRatePerMinute * 60 - intialSpawnDelay, 0, spawnRatePerMinute * 60);
    }

    public void EndSpawning() {
        isSpawning = false;
    }

    public void SpawnEnemy() {
        GameObject enemyGameobject = Instantiate(enemyPrefab);
        Enemy enemy = enemyGameobject.GetComponent<Enemy>();
        enemies.Add(enemy);

        // Pick a random start node from our list, and send the enemy on its way
        enemy.pathWalker.pathManager.startNode = startNodes[Random.Range(0, startNodes.Count)];
        enemy.pathWalker.pathWalkDuration = medianPathWalkTime + Random.Range(-pathWalkTimeVariance, pathWalkTimeVariance);

        enemy.Setup();
    }

    public void DespawnEnemy(Enemy enemy) {
        enemies.Remove(enemy);
        Destroy(enemy.gameObject);
    }

    public void DespawnAllEnemies() {
        for (int i = 0; i < enemies.Count; i++) {
            Destroy(enemies[i].gameObject);
        }

        enemies.Clear();
    }
    public void KillAllEnemies() {
        foreach (Enemy enemy in enemies) {
            enemy.Kill();
        }
    }
}
