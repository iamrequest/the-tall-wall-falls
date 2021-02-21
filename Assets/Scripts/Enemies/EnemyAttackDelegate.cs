using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Necessary delegate class to apply damage to the gate, since I can only call events from an animator event from components that are on the same gameobject as the animator component
///
/// https://forum.unity.com/threads/adding-events-to-an-animation.1000200/
/// </summary>
public class EnemyAttackDelegate : MonoBehaviour {
    public AudioSource stepAudioSource, attackGateAudioSource;
    private Enemy enemy;

    public List<AudioClip> stepSFX, attackGateSFX;

    private void Start() {
        enemy = GetComponentInParent<Enemy>();
        if (enemy == null) {
            Debug.LogError("No enemy component found!");
        }
    }

    public void AttackGate() {
        enemy.gate.ApplyDamage(enemy.damagePerAttack);
        attackGateAudioSource.PlayOneShot(attackGateSFX[Random.Range(0, attackGateSFX.Count)]);
    }

    public void PlayStepSFX() {
        stepAudioSource.PlayOneShot(stepSFX[Random.Range(0, stepSFX.Count)]);
    }
}
