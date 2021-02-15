using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour {
    [Header("Channels")]
    public VoidEventChannel settingsUpdatedChannel;
    public VoidEventChannel vignetteChangedChannel;

    [Header("GUI")]
    public Sprite spriteEnabled;
    public Sprite spriteDisabled;

    public TextMeshProUGUI vignetteEnabledText;
    public Image vignetteEnabledImage;
    private bool vignetteEnabled = true;

    //[Range(0f, 1f)]
    //public float vignetteStrength;

    private void Awake() {
        UpdateGUI();
    }

    private void OnEnable() {
        settingsUpdatedChannel.onEventRaised += UpdateGUI;
        vignetteChangedChannel.onEventRaised += ToggleVignette;
    }
    private void OnDisable() {
        settingsUpdatedChannel.onEventRaised -= UpdateGUI;
        vignetteChangedChannel.onEventRaised -= ToggleVignette;
    }

    public void UpdateGUI() {
        if (vignetteEnabled) {
            vignetteEnabledText.text = "Vignette: Enabled";
            vignetteEnabledImage.sprite = spriteEnabled;
        } else {
            vignetteEnabledText.text = "Vignette: Disabled";
            vignetteEnabledImage.sprite = spriteDisabled;
        }
    }


    public void ToggleVignette() {
        vignetteEnabled = !vignetteEnabled;
        settingsUpdatedChannel.RaiseEvent();
    }
}
