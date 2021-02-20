﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Config:
///     - Put this gameobject on a layer that can only interact with the enemy.
///     - Set the collider type to isTrigger.
///     
/// TODO: Consider adding a physical gate
/// </summary>
public class Gate : MonoBehaviour {
    public GameStateEventChannel.GameState gameState { get; private set; } = GameStateEventChannel.GameState.STOPPED;
    public GameStateEventChannel gameStateEventChannel;

    public List<Enemy> attackingEnemies;

    [field:SerializeField]
    public float health { get; private set; }
    public float maxHealth = 100f;
    public float damagePerEnemyPerSecond = 1f;

    private void OnEnable() {
        gameStateEventChannel.onEventRaised += UpdateGameState;
    }

    private void OnDisable() {
        gameStateEventChannel.onEventRaised -= UpdateGameState;
    }


    private void Update() {
        if (gameState == GameStateEventChannel.GameState.STARTED) {
            health -= damagePerEnemyPerSecond * attackingEnemies.Count * Time.deltaTime;

            if (health <= 0) {
                gameStateEventChannel.RaiseEvent(GameStateEventChannel.GameState.GAME_OVER);
            }
        }
    }

    public void UpdateGameState(GameStateEventChannel.GameState gameState) {
        this.gameState = gameState;
        attackingEnemies.Clear();

        if (gameState == GameStateEventChannel.GameState.STARTED) {
            health = maxHealth;
        }
    }
}
