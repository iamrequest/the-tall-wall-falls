using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Adapted from WireWhiz's tutorials 
/// - https://wirewhiz.com/making-half-life-alyx-physics-hands-in-unity/
/// - https://wirewhiz.com/vr-grabbing-tutorial/
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ConfigurableJoint))]
public class PhysicsHand : MonoBehaviour {
    private Rigidbody rb;
    private ConfigurableJoint configurableJoint;
    private Quaternion startRotation;
    public Transform targetTransform;

    [Tooltip("The maximum distance that this transform can be from the true hand, before it snaps directly to the true hand")]
    [Range(.02f, 1f)]
    public float snapDistance = 1f;
    public bool snapDistanceEnabled;


    // -- Extra help to fix config joint jitter
    //  Since the config joint tends to wobble a lot when idle, this will smooth the rotation out a bit.
    [Range(0f, 1f)]
    public float rotationLerpSpeed;
    private int touchCount = 0;

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
        configurableJoint = GetComponent<ConfigurableJoint>();

        // Need to cache the config joint's initial rotation on Start(), because joints have their own rotation space
        startRotation = transform.rotation;
    }

    // Update is called once per frame
    void FixedUpdate() {
        // Update the target rotation for this gameobject
        // TODO: This isn't taking into account the rotation of the player RB
        configurableJoint.SetTargetRotation(Quaternion.Inverse(configurableJoint.connectedBody.transform.rotation) * targetTransform.rotation, startRotation);

        // Update the remote anchor to point to the target transform
        configurableJoint.connectedAnchor = Quaternion.Inverse(configurableJoint.connectedBody.transform.rotation) * (targetTransform.position - configurableJoint.connectedBody.position);

        if (snapDistanceEnabled) {
            if ((transform.position - targetTransform.position).magnitude > snapDistance) {
                SnapToTrueHand();
            }
        }

        if (touchCount <= 0) {
            rb.MoveRotation(Quaternion.Lerp(transform.rotation, targetTransform.rotation, rotationLerpSpeed));
        }
    }

    private void SnapToTrueHand() {
        transform.position = targetTransform.position;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private void OnCollisionEnter(Collision collision) {
        touchCount++;
    }

    private void OnCollisionExit(Collision collision) {
        touchCount = Mathf.Clamp(touchCount - 1, 0, 99999);
    }

    public void ResetTouchCount() {
        touchCount = 0;
    }
}
