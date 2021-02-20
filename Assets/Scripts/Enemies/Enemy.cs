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
    public Animator animator;

    [Range(0f, 3f)]
    public float animationSpeed = 1f;

    public GameObject model;

    [Header("Despawn")]
    [Range(0f, 500f)]
    public float despawnDelay;
    private float elapsedDespawnTime;

    public bool isDead, isDespawned, isAttacking;


    private void Update() {
        animator.speed = animationSpeed;

        if (isDead && !isDespawned) {
            elapsedDespawnTime += Time.deltaTime;

            if (elapsedDespawnTime > despawnDelay) {
                Despawn();
            }
        }

        if (isAttacking) {
            // Rotate model to face the gate
            model.transform.rotation = Quaternion.Lerp(Quaternion.LookRotation(gate.transform.position - transform.position, Vector3.up), transform.rotation, pathWalker.turnSpeed);
        }
    }
    private void Awake() {
        pathWalker = GetComponent<EnemyPathWalker>();
        healthManager = GetComponent<EnemyHealthManager>();
    }

    public void Setup() {
        isDead = false;
        isDespawned = false;
        isAttacking = false;
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
        isAttacking = true;
        gate.attackingEnemies.Add(this);
        animator.SetTrigger("isAttacking");

        animator.speed = 1f;
        animationSpeed = 1f;
    }
}
