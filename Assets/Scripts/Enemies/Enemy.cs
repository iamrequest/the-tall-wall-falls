using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    public AudioSource audioSource;
    public VoidEventChannel enemyKilledEventChannel;

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
    public List<AudioClip> deathSFX;

    [Header("Despawn")]
    [Range(0f, 500f)]
    public float despawnDelay;
    private float elapsedDespawnTime;

    public bool isDead, isDespawned, isAttacking;

    [Range(1f, 10f)]
    public float damagePerAttack;

    [Range(0f, 5f)]
    public float animationSpeedMultiplier = 1f;


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
            animator.speed = 1f;
        } else {
            animator.speed = (animationSpeedMultiplier / pathWalker.pathWalkDuration * 100);
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
        pathWalker.Setup(gate.GetAttackablePosition() - transform.position);
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

    public void Kill(bool wasKilledByPlayer) {
        if (!isDead) {
            elapsedDespawnTime = 0f;
            isDead = true;

            pathWalker.isWalkingPath = false;
            healthManager.SetRagdollEnabled(true);
            enemySpawner.OnEnemyKilled(this);

            if (wasKilledByPlayer) {
                enemyKilledEventChannel.RaiseEvent();
            }

            audioSource.PlayOneShot(deathSFX[Random.Range(0, deathSFX.Count)]);
        }
    }


    public void StartAttackingGate() {
        isAttacking = true;
        animator.SetTrigger("isAttacking");

        animator.speed = 1f;
        animationSpeed = 1f;
    }

    /// <summary>
    /// Called via animation event
    /// </summary>
    public void AttackGate() {
        gate.ApplyDamage(damagePerAttack);
    }
}
