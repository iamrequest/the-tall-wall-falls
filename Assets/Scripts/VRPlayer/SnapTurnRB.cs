using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

/// <summary>
/// Simple snap turn, untied to SteamVR's Player prefab
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class SnapTurnRB : MonoBehaviour {
    private Rigidbody rb;
    public Transform headTransform;
    public SteamVR_Action_Boolean snapTurnLeftAction, snapTurnRightAction;

    [Range(0f, 90f)]
    public float angle = 45f;

    [Range(0f, 1f)]
    public float cooldownDuration = .25f;
    [HideInInspector]
    public bool isWaitingForCooldown = true;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() {
        if (isWaitingForCooldown) {
            if (snapTurnLeftAction.GetStateDown(SteamVR_Input_Sources.Any)) {
                RotatePlayer(-angle);
            } else if (snapTurnRightAction.GetStateDown(SteamVR_Input_Sources.Any)) {
                RotatePlayer(angle);
            }
        }
    }

    // Source: https://answers.unity.com/questions/10093/rigidbody-rotating-around-a-point-instead-on-self.html
    private void RotatePlayer(float degrees) {
        // Make sure we don't add weird upwards rotation
        Vector3 projectedHeadPosition = Vector3.ProjectOnPlane(headTransform.position, Vector3.up);

        Quaternion q = Quaternion.AngleAxis(degrees, Vector3.up);
        rb.MovePosition(q * (rb.transform.position - projectedHeadPosition) + projectedHeadPosition);
        rb.MoveRotation(rb.transform.rotation * q);

        // Wait a little bit before we can turn again
        StartCoroutine(WaitForCooldown());
    }

    private IEnumerator WaitForCooldown() {
        isWaitingForCooldown = false;
        yield return new WaitForSeconds(cooldownDuration);
        isWaitingForCooldown = true;
    }
}
