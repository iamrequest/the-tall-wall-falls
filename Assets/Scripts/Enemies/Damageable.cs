using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class Damageable : MonoBehaviour {
    public UnityEvent onDamaged;
    private AudioSource audioSource;
    public AudioClip damagedSFX;

    private void Awake() {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other) {
        onDamaged.Invoke();
        audioSource.PlayOneShot(damagedSFX);
    }
}
