using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Displays a simple Post-Processing vignette, with intensity relative to rb speed
/// </summary>
public class VignetteManager : MonoBehaviour {
    public VoidEventChannel vignetteEnabledEventChannel;
    public SliderEventChannel vignetteIntensityChangedEventChannel;

    public bool vignetteEnabled;
    public Volume volume;
    private Vignette vignette;

    public Rigidbody rb;
    public float minSpeed;
    private float elapsedTime;

    public float intensity = .75f;
    public float lerpDuration = 0.5f;

    private void Awake() {
        if (volume.sharedProfile.TryGet(out Vignette vignette)) {
            this.vignette = vignette;
        }
    }

    private void OnEnable() {
        vignetteEnabledEventChannel.onEventRaised += UpdateVignette;
        vignetteIntensityChangedEventChannel.onEventRaised += UpdateVignetteIntensity;
    }

    private void OnDisable() {
        vignetteEnabledEventChannel.onEventRaised -= UpdateVignette;
        vignetteIntensityChangedEventChannel.onEventRaised -= UpdateVignetteIntensity;
    }

    private void UpdateVignette() {
        vignetteEnabled = !vignetteEnabled;
        elapsedTime = 0f;
    }
    private void UpdateVignetteIntensity(float t, float intensity) {
        this.intensity = intensity;
    }


    private void FixedUpdate() {
        if (vignetteEnabled) {
            if (rb.velocity.magnitude > minSpeed) {
                elapsedTime = Mathf.Clamp(elapsedTime + Time.fixedDeltaTime, 0f, lerpDuration);
            } else {
                elapsedTime = Mathf.Clamp(elapsedTime - Time.fixedDeltaTime, 0f, lerpDuration);
            }

            float targetIntensity = Mathf.Lerp(0f, intensity, elapsedTime / lerpDuration);
            vignette.intensity.Override(targetIntensity);
        }
    }
}
