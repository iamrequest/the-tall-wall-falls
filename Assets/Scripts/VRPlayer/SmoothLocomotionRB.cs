﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

/// <summary>
/// Simple smooth locomotion using a rigidbody.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(JumpManagerRB))]
public class SmoothLocomotionRB : MonoBehaviour {
    public SteamVRInputSourcesEventChannel steeringTransformChangedEventChannel;
    public Transform steeringTransform;
    public Transform hmdTransform, leftHandTransform, rightHandTransform;

    private Rigidbody rb;
    private JumpManagerRB jumpManagerRB;

    public CapsuleCollider bodyCollider;
    public SteamVR_Action_Vector2 smoothLocomotionAction;

    // Note: onGroundMoveSpeed will still be applied if the player's feet don't touch the ground, but they're hugging a wall. Easy wall run I guess?
    [Range(0f, 10f)]
    public float onGroundMoveSpeed, inAirMoveSpeed;
    public ForceMode inAirForceMode;

    public PhysicMaterial highFrictionMat, lowFrictionMat;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        jumpManagerRB = GetComponent<JumpManagerRB>();
    }

    private void OnEnable() {
        steeringTransformChangedEventChannel.onEventRaised += UpdateSteeringTransform;
    }

    private void OnDisable() {
        steeringTransformChangedEventChannel.onEventRaised -= UpdateSteeringTransform;
    }


    private Vector3 GetInput() {
        Vector3 input = Vector3.zero;

        // Fetch the basis vector for the steering transform, projected onto player's local space
        Vector3 forward = Vector3.ProjectOnPlane(steeringTransform.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(steeringTransform.right, Vector3.up).normalized;

        // Apply player input
        input += forward * smoothLocomotionAction.axis.y;
        input += right * smoothLocomotionAction.axis.x;

        return input;
    }

    private void UpdateMotion(float moveSpeed, bool overwritePreviousVelocity, ForceMode forceMode = ForceMode.VelocityChange) {
        Vector3 motion;

        // Remove the last frame's velocity. Useful for grounded locomotion, for snappier motion
        if (overwritePreviousVelocity) {
            motion = -rb.velocity;
            motion.y = 0f;
        } else {
            motion = Vector3.zero;
        }

        // Apply player input
        motion += GetInput() * moveSpeed;

        rb.AddForce(motion, forceMode);
    }

    /// <summary>
    /// Update the body collider to align with the camera
    /// </summary>
    private void UpdateBodySize() {
        bodyCollider.height = hmdTransform.localPosition.y;
        bodyCollider.center = new Vector3(hmdTransform.localPosition.x,
            hmdTransform.localPosition.y / 2,
            hmdTransform.localPosition.z);
    }

    void FixedUpdate() {
        UpdateBodySize();

        if (smoothLocomotionAction.axis.sqrMagnitude > 0f) {
            // -- Player is applying motion
            bodyCollider.material = lowFrictionMat;

            // Determine the player's move speed, based on whether or not they're touching ground (or walls)
            if (jumpManagerRB.isTouchingGround) {
                UpdateMotion(onGroundMoveSpeed, true);
            } else {
                UpdateMotion(inAirMoveSpeed, false, inAirForceMode);
            }
        } else {
            // -- Player isn't applying input
            if (jumpManagerRB.isTouchingGround) {
                bodyCollider.material = highFrictionMat;
            }
        }
    }


    // -- Steering transform
    private void UpdateSteeringTransform(SteamVR_Input_Sources inputSource) {
        switch (inputSource) {
            case SteamVR_Input_Sources.LeftHand:
                steeringTransform = leftHandTransform;
                return;
            case SteamVR_Input_Sources.RightHand:
                steeringTransform = rightHandTransform;
                return;
            default:
                steeringTransform = hmdTransform;
                return;
        }
    }
}
