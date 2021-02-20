using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyHealthManager : MonoBehaviour {
    private Enemy enemy;

    public bool isAlive = true;
    public bool isRagdollActive = false;

    private Animator animator;
    public List<Rigidbody> limbs;

    private void Awake() {
        animator = GetComponentInChildren<Animator>();
        enemy = GetComponent<Enemy>();
        SetRagdollEnabled(isRagdollActive);
    }

    public void SetRagdollEnabled(bool value) {
        isRagdollActive = value;
        animator.enabled = !value;

        foreach (Rigidbody limb in limbs) {
            limb.isKinematic = !value;
        }
    }

    // Optional: Make damage more interesting than a 1-shot kill. 
    public void TakeDamage() {
        enemy.Kill(true);
    }
}
