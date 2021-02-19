using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

/// <summary>
/// Handles the instantiation/destruction of Spring Joints.
/// </summary>
public class RopeJointManager : MonoBehaviour {
    public SliderEventChannel pullSpeedUpdatedEventChannel;
    public SteamVR_Action_Boolean pullRopeAction;
    public SteamVR_Input_Sources inputSource;

    public Rigidbody playerRB;

    [HideInInspector]
    public SpringJoint joint;

    [HideInInspector]
    [Tooltip("The start transform of the rope")]
    public Transform localAnchorTransform;
    [HideInInspector]
    [Tooltip("The end transform of the rope")]
    public Transform targetAnchorTransform;

    [Tooltip("How fast the joint's max distance gets reduced each physics frame (while pullRopeAction is active)")]
    public float ropePullSpeed;

    public float spring, damper, minDistance;
    public float extraInitialRopeDistance;

    private void Awake() {
        localAnchorTransform = transform;
    }

    private void OnEnable() {
        pullSpeedUpdatedEventChannel.onEventRaised += GetNewRopePullSpeed;
    }

    private void OnDisable() {
        pullSpeedUpdatedEventChannel.onEventRaised -= GetNewRopePullSpeed;
    }
    private void GetNewRopePullSpeed(float t, float pullSpeed) {
        ropePullSpeed = pullSpeed;
    }



    public void AddJoint(Rigidbody target, Transform offsetTransform) {
        // Instantiate joint, and connect it to the target
        joint = playerRB.gameObject.AddComponent<SpringJoint>();
        targetAnchorTransform = offsetTransform;
        joint.connectedBody = target;

        // Apply joint settings
        joint.spring = spring;
        joint.damper = damper;
        joint.minDistance = minDistance;
        joint.enableCollision = true;
        joint.autoConfigureConnectedAnchor = false;

        joint.maxDistance = (targetAnchorTransform.position - localAnchorTransform.position).magnitude + extraInitialRopeDistance;
    }

    public void DestroyJoint() {
        Destroy(joint);
        targetAnchorTransform = null;
    }

    private void UpdateAnchors() {

        // Update the remote anchor
        if (targetAnchorTransform != null) {
            joint.connectedAnchor = GetJointOffset(joint.connectedBody, targetAnchorTransform);
        } else {
            joint.connectedAnchor = Vector3.zero;
        }

        // Update the local anchor
        if (localAnchorTransform != null) {
            joint.anchor = GetJointOffset(playerRB, localAnchorTransform);
        } else {
            joint.anchor = Vector3.zero;
        }
    }

    private Vector3 GetJointOffset(Rigidbody rb, Transform offsetTransform) {
        // Need to divide by the scale of the target, since the spring joint multiplies the connected anchor by the connected body's scale to find the target position.
        // In other words, I want to convert my local offset from rb to transform, into joint space
        Vector3 tmpAnchor = Quaternion.Inverse(rb.rotation) * (offsetTransform.position - rb.position);
        Vector3 offsetVector = rb.transform.lossyScale;

        tmpAnchor.x /= offsetVector.x;
        tmpAnchor.y /= offsetVector.y;
        tmpAnchor.z /= offsetVector.z;

        return tmpAnchor;
    }

    private void FixedUpdate() {
        if (joint != null) {
            if (pullRopeAction.GetState(inputSource)) {
                joint.maxDistance -= (ropePullSpeed * Time.fixedDeltaTime);

                // This is required, otherwise the rigidbody won't produce any motion until the rigidbody recieves motion from some other source (collision, player input, etc)
                playerRB.WakeUp();
            }

            // TODO: Update max distance
            UpdateAnchors();
        }
    }
}
