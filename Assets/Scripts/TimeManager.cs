using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class TimeManager : MonoBehaviour {
    public SteamVR_Action_Boolean slowMoAction;
    public bool isTimeSlowed;

    [Range(0f, 1f)]
    public float slowdownFactor = 0.5f;
    public AnimationCurve timeLerpCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    // -- Used for calculating lerp
    [Range(0.01f, 2f)]
    public float lerpDurationUnscaled;
    private float elapsedLerpDurationUnscaled;
    private float originalTimeScale;

    // -- Audio SFX
    private AudioSource audioSource;
    public AudioClip slowTimeSFX, restoreTimeSFX;

    [Range(0f, 1f)]
    public float slowTimeSFXPitch, restoreTimeSFXPitch;

    private void Awake() {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {
        // Get input
        if (slowMoAction.GetStateDown(SteamVR_Input_Sources.Any) && elapsedLerpDurationUnscaled >= lerpDurationUnscaled) {
            isTimeSlowed = !isTimeSlowed;

            // Play SFX
            if (isTimeSlowed) {
                audioSource.pitch = slowTimeSFXPitch;
                audioSource.PlayOneShot(slowTimeSFX);
            } else {
                audioSource.pitch = restoreTimeSFXPitch;
                audioSource.PlayOneShot(restoreTimeSFX);
            }

            originalTimeScale = Time.timeScale;
            elapsedLerpDurationUnscaled = 0f;
        }

        // Lerp to the target time scale
        if (elapsedLerpDurationUnscaled < lerpDurationUnscaled) {
            elapsedLerpDurationUnscaled = Mathf.Clamp(elapsedLerpDurationUnscaled + Time.unscaledDeltaTime, 0f, lerpDurationUnscaled);

            if (isTimeSlowed) {
                Time.timeScale = Mathf.Lerp(originalTimeScale, slowdownFactor, timeLerpCurve.Evaluate(elapsedLerpDurationUnscaled / lerpDurationUnscaled));
                Time.fixedDeltaTime = Time.timeScale * .02f;
            } else {
                Time.timeScale = Mathf.Lerp(originalTimeScale, 1f, timeLerpCurve.Evaluate(elapsedLerpDurationUnscaled / lerpDurationUnscaled));
            }
        }
    }
}
