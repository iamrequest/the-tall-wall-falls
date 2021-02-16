using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

/// <summary>
/// Simple collider gameobject that raises an event on an SO channel.
///
/// I've added a bunch of options for what type of event channels to raise events on. All are optional
/// </summary>
public class SettingsMenuColliderButton : MonoBehaviour {
    public string targetTag = "Player";
    private AudioSource audioSource;
    public AudioClip buttonClickedSFX;

    [Header("Events to be invoked on trigger (Optional)")]
    public UnityEvent OnPressed;
    public VoidEventChannel voidEvent;
    public SteamVRInputSourcesEventChannel steamVRInputSourcesEvent;
    public SteamVR_Input_Sources steamVRInputSourcePayload;


    private void Awake() {
        audioSource = GetComponentInParent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag(targetTag)) {
            if (voidEvent) {
                voidEvent.RaiseEvent();
            }

            if (steamVRInputSourcesEvent) {
                steamVRInputSourcesEvent.RaiseEvent(steamVRInputSourcePayload);
            }

            OnPressed.Invoke();

            if (buttonClickedSFX != null) {
                audioSource.PlayOneShot(buttonClickedSFX);
            }
        }
    }
}
