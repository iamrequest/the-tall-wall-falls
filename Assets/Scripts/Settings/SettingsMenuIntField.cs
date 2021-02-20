using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenuIntField: MonoBehaviour {
    public IntEventChannel intEventChannel;

    [Tooltip("Range is from 0 (inclusive) to maxValue (inclusive)")]
    public int value, maxValue;

    private void OnEnable() {
        intEventChannel.onEventRaised += GetValueFromEventChannel;
    }
    private void OnDisable() {
        intEventChannel.onEventRaised -= GetValueFromEventChannel;
    }

    private void GetValueFromEventChannel (int value) {
        this.value = value;
    }
    public void GetValue(int value) {
        this.value = value;
        intEventChannel.onEventRaised(value);
    }
    public void NextValue() {
        value = (value + 1) % maxValue;
        intEventChannel.RaiseEvent(value);
    }
    public void PreviousValue() {
        value--;
        if (value < 0) {
            value = maxValue;
        }

        intEventChannel.RaiseEvent(value);
    }
}
