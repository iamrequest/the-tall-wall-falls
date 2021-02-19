using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

/// <summary>
/// Mostly used for interacting with the settings menu slider
/// </summary>
public class SettingsMenuInteractor : MonoBehaviour {
    public SteamVR_Action_Boolean menuInteractionAction;
    public SteamVR_Input_Sources inputSource;

    [HideInInspector]
    public SettingsMenuSlider hoveredSlider, grabbedSlider;

    [TextArea]
    [Tooltip("Read-only note. No effect on gameplay.")]
    public String README = "Settings interactors should be on the Water layer. I'm re-purposing this layer, since it has no usage in this game";

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.TryGetComponent(out SettingsMenuSlider slider)) {
            hoveredSlider = slider;
        }
    }

    private void OnTriggerExit(Collider other) {
        hoveredSlider = null;
    }

    private void FixedUpdate() {
        // If we're hovering a slider
        if (grabbedSlider) {
            // If we let go of the slider just now
            if (menuInteractionAction.GetStateUp(inputSource)) {
                ReleaseSliderGrab();
            }

            // Lerp slider to hand
            if (grabbedSlider) {
                grabbedSlider.LerpSlider(transform);
                grabbedSlider.SendUpdatesToEventChannel();
            }
        } else if (hoveredSlider) {
            // If we're hovering a slider, then start grabbing it
            if (menuInteractionAction.GetStateDown(inputSource)) {
                // Only start grabbing it if we're not currently holding something
                if (hoveredSlider.interactor == null) {
                    StartSliderGrab(hoveredSlider);
                }
            }
        }
    }

    private void StartSliderGrab(SettingsMenuSlider slider) {
        grabbedSlider = slider;
        slider.interactor = this;
    }

    private void ReleaseSliderGrab() {
        if (grabbedSlider) {
            grabbedSlider.Release();
        }

        grabbedSlider = null;
    }
}
