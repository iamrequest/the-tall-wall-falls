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

    private void Awake() {
        UpdateGUI();
    }

    private void OnEnable() {
        settingsUpdatedChannel.onEventRaised += UpdateGUI;
        vignetteChangedChannel.onEventRaised += ToggleVignette;
        speedLinesChangedChannel.onEventRaised += ToggleSpeedLines;
    }

    private void OnDisable() {
        settingsUpdatedChannel.onEventRaised -= UpdateGUI;
        vignetteChangedChannel.onEventRaised -= ToggleVignette;
        speedLinesChangedChannel.onEventRaised -= ToggleSpeedLines;
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
    }


    public void ToggleVignette() {
        vignetteEnabled = !vignetteEnabled;
        settingsUpdatedChannel.RaiseEvent();
    }
    public void ToggleSpeedLines() {
        speedLinesEnabled = !speedLinesEnabled;
        settingsUpdatedChannel.RaiseEvent();
    }
}
