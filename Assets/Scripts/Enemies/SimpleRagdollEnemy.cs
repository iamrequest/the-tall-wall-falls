using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Subset of Enemy and EnemyHealthManager, that just dies on damage. No gate/locomotion functionality
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class SimpleRagdollEnemy : MonoBehaviour {
    private AudioSource audioSource;
    public UnityEvent onDeath;
    public List<AudioClip> deathSFX;

    public bool isAlive = true;
    public bool isRagdollActive = false;

    public bool reviveAfterDelay;
    public float reviveDelay;
    private float elapsedReviveTime;

    private Animator animator;
    public List<Rigidbody> limbs;

    private void Awake() {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponentInChildren<Animator>();
        SetRagdollEnabled(isRagdollActive);
    }

    private void Update() {
        if (!isAlive && reviveAfterDelay) {
            // Don't tie this timer to slowmo
            elapsedReviveTime += Time.unscaledDeltaTime;

            if (elapsedReviveTime >= reviveDelay) {
                Revive();
            }
        }
    }

    public void SetRagdollEnabled(bool value) {
        isRagdollActive = value;
        animator.enabled = !value;

        foreach (Rigidbody limb in limbs) {
            limb.isKinematic = !value;
        }
    }

    public void Kill() {
        if (isAlive) {
            isAlive = false;

            SetRagdollEnabled(true);
            onDeath.Invoke();

            if (reviveAfterDelay) {
                elapsedReviveTime = 0f;
            }
        }
    }

    public void Revive() {
        isAlive = true;
        SetRagdollEnabled(false);
    }
}
