using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Freya;

public class SettingsMenuSlider : MonoBehaviour {
    public SliderEventChannel sliderEventChannel;
    public Transform settingsBoard;

    [HideInInspector]
    public SettingsMenuInteractor interactor;

    //private LinearDrive linearDrive;
    public Transform startTransform, endTransform;
    public float minValue, maxValue, initialValue;
    public float t;


    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(startTransform.position, .05f);
        Gizmos.DrawWireSphere(endTransform.position, .05f);
        Gizmos.DrawLine(startTransform.position, endTransform.position);
    }


    public void LerpSlider(Transform target) {
        Vector3 handPosition = Vector3.ProjectOnPlane(target.position, settingsBoard.forward);
        t = InverseLerp(startTransform.position, endTransform.position, handPosition);
        t = Mathf.Clamp01(t);

        // Lerp to the desired spot, since the projected hand position can exist outside of the bounds of the two target transforms
        transform.position = Vector3.Lerp(startTransform.position, endTransform.position, t);
    }

    // https://answers.unity.com/questions/1271974/inverselerp-for-vector3.html
    private float InverseLerp(Vector3 a, Vector3 b, Vector3 v) {
        return Vector3.Dot(v - a, b - a) / Vector3.Dot(b - a, b - a);
    }

    public float GetScaledValue() {
        return Mathfs.Remap(0f, 1f, minValue, maxValue, t);
    }

    public void Release() {
        interactor = null;
    }

    public void SendUpdatesToEventChannel() {
        sliderEventChannel.RaiseEvent(t, GetScaledValue());
    }

    public void ResetSlider() {
        if (interactor == null) {
            t = Mathf.Clamp01(Mathf.InverseLerp(minValue, maxValue, initialValue));

            // Lerp to the desired spot, since the projected hand position can exist outside of the bounds of the two target transforms
            transform.position = Vector3.Lerp(startTransform.position, endTransform.position, t);

            SendUpdatesToEventChannel();
        }
    }
}
