using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Valve.VR;

/// <summary>
/// Handles the actual propogation of settings data
/// </summary>
public class SettingsManager : MonoBehaviour {
    [Header("Channels")]
    public VoidEventChannel settingsUpdatedChannel;
    public VoidEventChannel vignetteChangedChannel;
    public VoidEventChannel speedLinesChangedChannel;
    public SliderEventChannel swordAngleChangedChannel;
    public SliderEventChannel grappleAngleChangedChannel;
    public SliderEventChannel vignetteStrengthChangedChannel;
    public SliderEventChannel ropePullSpeedChangedChannel;
    public SteamVRInputSourcesEventChannel steeringTransformChangedChannel;

    [Header("GUI")]
    public Sprite spriteEnabled, spriteDisabled;
    public Sprite spriteHMD, spriteLeftHand, spriteRightHand;

    // -- Vignette
    public TextMeshProUGUI vignetteEnabledText;
    public Image vignetteEnabledImage;
    private bool vignetteEnabled = true;

    // -- Speed lines
    public TextMeshProUGUI speedLinesEnabledText;
    public Image speedLinesEnabledImage;
    private bool speedLinesEnabled = true;

    // -- Sword Angle
    public TextMeshProUGUI swordAngleText;
    public SettingsMenuSlider swordAngleSlider;
    public float swordAngleScaled { get; private set; }

    // -- GrappleAngle
    public TextMeshProUGUI grappleAngleText;
    public SettingsMenuSlider grappleAngleSlider;
    public float grappleAngleScaled { get; private set; }

    // -- Vignette Strength
    public TextMeshProUGUI vignetteStrengthText;
    public SettingsMenuSlider vignetteStrengthSlider;
    public float vignetteStrengthScaled { get; private set; }

    // -- Rope Pull Speed
    public TextMeshProUGUI ropePullSpeedText;
    public SettingsMenuSlider ropePullSpeedSlider;
    public float ropePullSpeedScaled { get; private set; }

    // -- Steering Transform
    public SteamVR_Input_Sources steeringTransformInputSource;
    public Image steeringTransformImage;

    private void Start() {
        swordAngleSlider.ResetSlider();
        grappleAngleSlider.ResetSlider();
        vignetteStrengthSlider.ResetSlider();
        ropePullSpeedSlider.ResetSlider();

        UpdateGUI();
    }

    private void OnEnable() {
        settingsUpdatedChannel.onEventRaised += UpdateGUI;
        vignetteChangedChannel.onEventRaised += ToggleVignette;
        speedLinesChangedChannel.onEventRaised += ToggleSpeedLines;
        swordAngleChangedChannel.onEventRaised += GetSwordAngle;
        grappleAngleChangedChannel.onEventRaised += GetGrappleAngle;
        vignetteStrengthChangedChannel.onEventRaised += GetVignetteStrength;
        ropePullSpeedChangedChannel.onEventRaised += GetRopePullSpeed;
        steeringTransformChangedChannel.onEventRaised += GetSteeringTransform;
    }

    private void OnDisable() {
        settingsUpdatedChannel.onEventRaised -= UpdateGUI;
        vignetteChangedChannel.onEventRaised -= ToggleVignette;
        speedLinesChangedChannel.onEventRaised -= ToggleSpeedLines;
        swordAngleChangedChannel.onEventRaised -= GetSwordAngle;
        grappleAngleChangedChannel.onEventRaised -= GetGrappleAngle;
        vignetteStrengthChangedChannel.onEventRaised -= GetVignetteStrength;
        ropePullSpeedChangedChannel.onEventRaised -= GetRopePullSpeed;
        steeringTransformChangedChannel.onEventRaised -= GetSteeringTransform;
    }

    public void UpdateGUI() {
        // -- Vignette
        if (vignetteEnabled) {
            vignetteEnabledText.text = "Vignette: Enabled";
            vignetteEnabledImage.sprite = spriteEnabled;
        } else {
            vignetteEnabledText.text = "Vignette: Disabled";
            vignetteEnabledImage.sprite = spriteDisabled;
        }

        // -- Speed lines
        if (speedLinesEnabled) {
            speedLinesEnabledText.text = "Speed Lines: Enabled";
            speedLinesEnabledImage.sprite = spriteEnabled;
        } else {
            speedLinesEnabledText.text = "Speed Lines: Disabled";
            speedLinesEnabledImage.sprite = spriteDisabled;
        }

        // -- Sword angle
        if (swordAngleScaled >= 0f) {
            swordAngleText.text = "Sword Angle: +" + swordAngleScaled.ToString("F0") + " degrees";
        } else {
            swordAngleText.text = "Sword Angle: " + swordAngleScaled.ToString("F0") + " degrees";
        }

        // -- Grapple angle
        if (grappleAngleScaled >= 0f) {
            grappleAngleText.text = "Grapple Angle: +" + grappleAngleScaled.ToString("F0") + " degrees";
        } else {
            grappleAngleText.text = "Grapple Angle: " + grappleAngleScaled.ToString("F0") + " degrees";
        }

        // -- Vignette Strength
        vignetteStrengthText.text = "Vignette Strength: " + (vignetteStrengthScaled * 100).ToString("F0") + "%";

        // -- Rope Pull Speed
        ropePullSpeedText.text = "Rope Pull Speed: " + (ropePullSpeedScaled).ToString("F1");

        // -- Steering Transform 
        switch (steeringTransformInputSource) {
            case SteamVR_Input_Sources.LeftHand:
                steeringTransformImage.sprite = spriteLeftHand;
                break;
            case SteamVR_Input_Sources.RightHand:
                steeringTransformImage.sprite = spriteRightHand;
                break;
            default:
                steeringTransformImage.sprite = spriteHMD;
                break;
        }
    }


    public void ToggleVignette() {
        vignetteEnabled = !vignetteEnabled;
        settingsUpdatedChannel.RaiseEvent();
    }
    public void ToggleSpeedLines() {
        speedLinesEnabled = !speedLinesEnabled;
        settingsUpdatedChannel.RaiseEvent();
    }
    public void GetSwordAngle(float t, float scaledAngle) {
        swordAngleScaled = scaledAngle;
    }
    public void GetGrappleAngle(float t, float scaledAngle) {
        grappleAngleScaled = scaledAngle;
    }
    public void GetVignetteStrength(float t, float vignetteStrength) {
        vignetteStrengthScaled = vignetteStrength;
    }
    public void GetRopePullSpeed(float t, float pullSpeed) {
        ropePullSpeedScaled = pullSpeed;
    }
    public void GetSteeringTransform(SteamVR_Input_Sources inputSource) {
        steeringTransformInputSource = inputSource;
    }
}
