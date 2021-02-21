using System.Collections;
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
    public SliderEventChannel gateHealthEventChannel;

    public List<Enemy> attackingEnemies;

    [Tooltip("A random place in this area will be where the enemy will walk to in order to attack")]
    public Transform areaToAttack;

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

            if (attackingEnemies.Count > 0) {
                gateHealthEventChannel.RaiseEvent(health / maxHealth, health);
            }

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
            gateHealthEventChannel.RaiseEvent(health / maxHealth, health);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>A world-space position that exists inside of areaToAttack</returns>
    public Vector3 GetAttackablePosition() {
        // TODO: I don't think this would work if I rotated areaToAttack
        Vector3 pos = areaToAttack.position;
        pos.x += Random.Range(-areaToAttack.localScale.x, areaToAttack.localScale.x) / 2;
        pos.z += Random.Range(-areaToAttack.localScale.z, areaToAttack.localScale.z) / 2;
        pos.y = 0f;

        return pos;
    }
}
