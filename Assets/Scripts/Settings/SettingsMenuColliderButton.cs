using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple collider gameobject that raises an event on an SO channel.
/// </summary>
public class SettingsMenuColliderButton : MonoBehaviour {
    public VoidEventChannel OnColliderPressed;
    public string targetTag = "Player";

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag(targetTag)) {
            OnColliderPressed.RaiseEvent();
        }
    }
}
