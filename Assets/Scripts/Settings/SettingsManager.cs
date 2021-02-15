using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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

    [Header("GUI")]
    public Sprite spriteEnabled;
    public Sprite spriteDisabled;

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

    // -- GrappleAngle
    public TextMeshProUGUI vignetteStrengthText;
    public SettingsMenuSlider vignetteStrengthSlider;
    public float vignetteStrengthScaled { get; private set; }

    private void Start() {
        swordAngleSlider.ResetSlider();
        grappleAngleSlider.ResetSlider();
        vignetteStrengthSlider.ResetSlider();

        UpdateGUI();
    }

    private void OnEnable() {
        settingsUpdatedChannel.onEventRaised += UpdateGUI;
        vignetteChangedChannel.onEventRaised += ToggleVignette;
        speedLinesChangedChannel.onEventRaised += ToggleSpeedLines;
        swordAngleChangedChannel.onEventRaised += GetSwordAngle;
        grappleAngleChangedChannel.onEventRaised += GetGrappleAngle;
        vignetteStrengthChangedChannel.onEventRaised += GetVignetteStrength;
    }

    private void OnDisable() {
        settingsUpdatedChannel.onEventRaised -= UpdateGUI;
        vignetteChangedChannel.onEventRaised -= ToggleVignette;
        speedLinesChangedChannel.onEventRaised -= ToggleSpeedLines;
        swordAngleChangedChannel.onEventRaised -= GetSwordAngle;
        grappleAngleChangedChannel.onEventRaised -= GetGrappleAngle;
        vignetteStrengthChangedChannel.onEventRaised -= GetVignetteStrength;
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
}
