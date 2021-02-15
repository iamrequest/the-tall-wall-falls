using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class AngleManager : MonoBehaviour {
    public SliderEventChannel sliderEventChannel;
    private Quaternion defaultAngle;

    public float offsetAngle;
    public enum Axis { X, Y, Z };
    public Axis targetAxis;

    private void Awake() {
        defaultAngle = transform.localRotation;
    }
    private void OnEnable() {
        sliderEventChannel.onEventRaised += GetNewAngle;
    }
    private void OnDisable() {
        sliderEventChannel.onEventRaised -= GetNewAngle;
    }

    private void FixedUpdate() {
        SetAngle();
    }

    public float GetAngle() {
        switch (targetAxis) {
            case Axis.X:
                return transform.rotation.x;
            case Axis.Y:
                return transform.rotation.y;
            default:
                return transform.rotation.z;
        }
    }

    public void SetAngle() {
        Vector3 rot = Vector3.zero; ;
        switch (targetAxis) {
            case Axis.X:
                rot.x = offsetAngle;
                break;
            case Axis.Y:
                rot.y = offsetAngle;
                break;
            default:
                rot.z = offsetAngle;
                break;
        }

        transform.localRotation = defaultAngle * Quaternion.Euler(rot);
    }

    private void GetNewAngle(float t, float newAngle) {
        offsetAngle = newAngle;
    }
}
