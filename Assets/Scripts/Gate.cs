using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Config:
///     - Put this gameobject on a layer that can only interact with the enemy.
///     - Set the collider type to isTrigger.
///     
/// TODO: Consider adding a physical gate
/// </summary>
[RequireComponent(typeof(Animator))]
public class Gate : MonoBehaviour {
    private Animator animator;
    public GameStateEventChannel.GameState gameState { get; private set; } = GameStateEventChannel.GameState.STOPPED;
    public GameStateEventChannel gameStateEventChannel;
    public SliderEventChannel gateHealthEventChannel;
    public TextMeshProUGUI healthText;


    [Tooltip("A random place in this area will be where the enemy will walk to in order to attack")]
    public Transform areaToAttack;

    [field:SerializeField]
    public float health { get; private set; }
    public float maxHealth = 100f;
    public float damagePerEnemyPerSecond = 1f;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void OnEnable() {
        gameStateEventChannel.onEventRaised += UpdateGameState;
    }

    private void OnDisable() {
        gameStateEventChannel.onEventRaised -= UpdateGameState;
    }


    private void Update() {
        if (gameState == GameStateEventChannel.GameState.STARTED) {
            // Update GUI
            healthText.text = "Health: " + (health / maxHealth * 100f).ToString("F0") + "%";

            // Check for game over
            if (health <= 0) {
                gameStateEventChannel.RaiseEvent(GameStateEventChannel.GameState.GAME_OVER);
            }
        }
    }

    public void UpdateGameState(GameStateEventChannel.GameState gameState) {
        this.gameState = gameState;

        if (gameState == GameStateEventChannel.GameState.STARTED) {
            health = maxHealth;
            animator.SetTrigger("GameStarted");
            gateHealthEventChannel.RaiseEvent(health / maxHealth, health);
        } else if (gameState == GameStateEventChannel.GameState.STOPPED) {
            animator.SetTrigger("GameStopped");
        } else {
            animator.SetTrigger("GameOver");
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

    public void ApplyDamage(float damage) {
        health -= damage;
        gateHealthEventChannel.RaiseEvent(health / maxHealth, health);
    }
}
