using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    [HideInInspector]
    public EnemyPathWalker pathWalker;
    [HideInInspector]
    public EnemyHealthManager healthManager;
    [HideInInspector]
    public EnemySpawner enemySpawner ;
    [HideInInspector]
    public Gate gate;

    public GameObject model;

    [Header("Despawn")]
    [Range(0f, 500f)]
    public float despawnDelay;
    private float elapsedDespawnTime;

    public bool isDead, isDespawned;


    private void Update() {
        if (isDead && !isDespawned) {
            elapsedDespawnTime += Time.deltaTime;

            if (elapsedDespawnTime > despawnDelay) {
                Despawn();
            }
        }
    }
    private void Awake() {
        pathWalker = GetComponent<EnemyPathWalker>();
        healthManager = GetComponent<EnemyHealthManager>();
    }

    public void Setup() {
        isDead = false;
        isDespawned = false;
        elapsedDespawnTime = 0f;

        model.SetActive(true);
        pathWalker.Setup();
        healthManager.SetRagdollEnabled(false);
    }

    public void Despawn() {
        // -- Make sure that we detach the projectile 
        RopeProjectile projectile = GetComponentInChildren<RopeProjectile>();
        if (projectile != null) {
            projectile.ropeManager.ReturnProjectile();
        }

        // TODO: Is this part necessary?
        model.SetActive(false);

        enemySpawner.DespawnEnemy(this);
    }

    public void Kill() {
        elapsedDespawnTime = 0f;
        isDead = true;

        pathWalker.isWalkingPath = false;
        healthManager.SetRagdollEnabled(true);
    }


    // TODO: Lerp into position?
    public void StartAttackingGate() {
        gate.attackingEnemies.Add(this);
    }
}
