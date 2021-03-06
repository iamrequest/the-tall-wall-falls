﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TODO: Object pool
/// </summary>
public class EnemySpawner : MonoBehaviour {
    [field:SerializeField]
    public GameStateEventChannel.GameState gameState { get; private set; } = GameStateEventChannel.GameState.STOPPED;
    public GameStateEventChannel gameStateEventChannel;
    public SliderEventChannel spawnRateEventChannel, numEnemiesEventChannel, enemySpeedEventChannel;

    public GameObject enemyPrefab;
    public Gate gate;
    public List<Enemy> aliveEnemies, deadEnemies;
    public List<PathNode> startNodes;

    [Range(0f, 500f)]
    [Tooltip("Measured in seconds")]
    public float spawnDelay;
    public float timeSinceLastSpawn;

    [Tooltip("Measured in seconds")]
    public float intialSpawnDelay;
    public bool isSpawning;

    [Range(0, 50)]
    public int maxNumEnemies;
    [Range(0, 10)]
    public int maxNumDeadEnemies = 10;

    [Range(10, 300)]
    public float medianPathWalkTime;
    [Range(0f, 1f)]
    public float pathWalkTimeVariancePercentage;

    private void Awake() {
        if (startNodes.Count == 0) {
            Debug.LogWarning("No start spawn nodes defined in the enemy spawner. Did you forget to assign them?");
        }
    }
    private void OnEnable() {
        gameStateEventChannel.onEventRaised += UpdateGameState;
        spawnRateEventChannel.onEventRaised += SetSpawnRate;
        numEnemiesEventChannel.onEventRaised += SetNumEnemies;
        enemySpeedEventChannel.onEventRaised += SetEnemySpeed;
    }

    private void OnDisable() {
        gameStateEventChannel.onEventRaised -= UpdateGameState;
        spawnRateEventChannel.onEventRaised -= SetSpawnRate;
        numEnemiesEventChannel.onEventRaised -= SetNumEnemies;
        enemySpeedEventChannel.onEventRaised -= SetEnemySpeed;
    }


    public void UpdateGameState(GameStateEventChannel.GameState gameState) {
        switch (gameState) {
            case GameStateEventChannel.GameState.STARTED:
                StartSpawning();
                break;
            case GameStateEventChannel.GameState.STOPPED:
            case GameStateEventChannel.GameState.GAME_OVER:
                KillAllEnemies();
                isSpawning = false;
                break;
        }
    }

    // Update is called once per frame
    void Update() {
        if (isSpawning && aliveEnemies.Count < maxNumEnemies) {
            timeSinceLastSpawn += Time.deltaTime;

            if (timeSinceLastSpawn > spawnDelay) {
                SpawnEnemy();
                timeSinceLastSpawn = 0f;
            }
        }
    }



    public void StartSpawning() {
        DespawnAllEnemies();
        isSpawning = true;
        timeSinceLastSpawn = Mathf.Clamp(spawnDelay - intialSpawnDelay, 0, spawnDelay);
    }

    public void EndSpawning() {
        isSpawning = false;
    }

    public void SpawnEnemy() {
        GameObject enemyGameobject = Instantiate(enemyPrefab);
        Enemy enemy = enemyGameobject.GetComponent<Enemy>();
        aliveEnemies.Add(enemy);

        // -- Configure enemy
        enemy.enemySpawner = this;
        enemy.gate = gate;

        // Pick a random start node from our list, and send the enemy on its way
        enemy.pathWalker.pathManager.startNode = startNodes[Random.Range(0, startNodes.Count)];
        enemy.pathWalker.pathWalkDuration = medianPathWalkTime + Random.Range(-pathWalkTimeVariancePercentage * medianPathWalkTime, pathWalkTimeVariancePercentage * medianPathWalkTime);

        enemy.Setup();
    }

    public void DespawnEnemy(Enemy enemy) {
        aliveEnemies.Remove(enemy);
        deadEnemies.Remove(enemy);
        Destroy(enemy.gameObject);
    }

    public void DespawnAllEnemies() {
        for (int i = 0; i < aliveEnemies.Count; i++) {
            Destroy(aliveEnemies[i].gameObject);
        }
        for (int i = 0; i < deadEnemies.Count; i++) {
            Destroy(deadEnemies[i].gameObject);
        }

        aliveEnemies.Clear();
        deadEnemies.Clear();
    }
    public void KillAllEnemies() {
        // Gotta do this weird loop + indexing because I'm modifying the list in spaghetti fashion
        int numEnemies = aliveEnemies.Count;
        for (int i = 0; i < numEnemies; i++) {
            aliveEnemies[aliveEnemies.Count - 1].Kill(false);
        }
        aliveEnemies.Clear();
    }

    public void OnEnemyKilled(Enemy enemy) {
        aliveEnemies.Remove(enemy);
        deadEnemies.Add(enemy);

        // If we have too many dead enemies, remove the oldest one
        if (deadEnemies.Count > maxNumDeadEnemies) {
            DespawnEnemy(deadEnemies[0]);
        }
    }

    // -- TODO: Update values
    public void SetSpawnRate(float t, float value) {
        spawnDelay = value;
    }
    public void SetNumEnemies(float t, float value) {
        maxNumEnemies = Mathf.RoundToInt(value);
    }
    public void SetEnemySpeed(float t, float value) {
        medianPathWalkTime = value;
    }
}
