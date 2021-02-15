using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

/// <summary>
/// TODO: Fix enabled/disable
/// TODO: Rotate to match locomotion dir
/// </summary>
public class SpeedLinesManager : MonoBehaviour {
    public bool speedLinesEnabled;
    public VoidEventChannel settingsUpdatedChannel;

    private VisualEffect vfx;
    private bool isPlaying;

    public Rigidbody rb;
    public float minSpeed;
    private float elapsedTime;
    public float lerpSpeeed = 0.5f;
    private Quaternion targetRotation;

    private void Awake() {
        vfx = GetComponent<VisualEffect>();
    }

    private void OnEnable() {
        settingsUpdatedChannel.onEventRaised += UpdateSpeedLines;
    }

    private void OnDisable() {
        settingsUpdatedChannel.onEventRaised -= UpdateSpeedLines;
    }
    private void UpdateSpeedLines() {
        speedLinesEnabled = !speedLinesEnabled;
        vfx.Stop();
        elapsedTime = 0f;
    }

    private void FixedUpdate() {
        if (speedLinesEnabled) {
            if (rb.velocity.magnitude > minSpeed) {
                // Rotate the VFX so that the speed lines move away from the rigidbody's velocity
                // TODO: Test this
                targetRotation = Quaternion.LookRotation(transform.position + rb.velocity, Vector3.up);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, lerpSpeeed);

                if (!isPlaying) {
                    vfx.Play();
                    isPlaying = true;

                    transform.rotation = targetRotation;
                }

                //elapsedTime = Mathf.Clamp(elapsedTime + Time.fixedDeltaTime, 0f, lerpDuration);
            } else {
                isPlaying = false;
                vfx.Stop();
                //elapsedTime = Mathf.Clamp(elapsedTime - Time.fixedDeltaTime, 0f, lerpDuration);
            }

            //float targetIntensity = Mathf.Lerp(0f, intensity, elapsedTime / lerpDuration);
        }
    }
}
