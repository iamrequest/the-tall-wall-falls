using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

/// <summary>
/// Simple smooth locomotion using a rigidbody.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class SmoothLocomotionRB : MonoBehaviour {
    private Rigidbody rb;

    public Transform head;
    public CapsuleCollider bodyCollider;
    public Transform steeringTransform;
    public SteamVR_Action_Vector2 smoothLocomotionAction;

    // Note: onGroundMoveSpeed will still be applied if the player's feet don't touch the ground, but they're hugging a wall. Easy wall run I guess?
    [Range(0f, 10f)]
    public float onGroundMoveSpeed, inAirMoveSpeed;

    private int groundCount;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    private Vector3 GetInput() {
        Vector3 input = Vector3.zero;

        // Fetch the basis vector for the steering transform, projected onto player's local space
        Vector3 forward = Vector3.ProjectOnPlane(steeringTransform.forward, Vector3.up);
        Vector3 right = Vector3.ProjectOnPlane(steeringTransform.right, Vector3.up);

        // Apply player input
        input += forward * smoothLocomotionAction.axis.y;
        input += right * smoothLocomotionAction.axis.x;

        return input;
    }

    private void UpdateMotion(float moveSpeed) {
        // Remove the last frame's velocity
        Vector3 motion = -rb.velocity;

        // Likely unnecessary?
        motion.y = 0f;

        // Apply player input
        motion += GetInput() * moveSpeed;

        rb.AddForce(motion, ForceMode.VelocityChange);
    }

    /// <summary>
    /// Update the body collider to align with the camera
    /// </summary>
    private void UpdateBodySize() {
        bodyCollider.height = head.localPosition.y;
        bodyCollider.center = new Vector3(head.localPosition.x, 
            head.localPosition.y / 2, 
            head.localPosition.z);
    }

    void FixedUpdate() {
        UpdateBodySize();

        if (smoothLocomotionAction.axis.sqrMagnitude > 0f) {
            // -- Player is applying motion
            // TODO: Set physics mat

            // Determine the player's move speed, based on whether or not they're touching ground (or walls)
            if (groundCount > 0) {
                UpdateMotion(onGroundMoveSpeed);
            } else {
                // TODO: This likely won't work, since I'm removing the old velocity
                UpdateMotion(inAirMoveSpeed);
            }
        } else {
            // -- Player isn't applying input

            if (groundCount > 0) {
                // TODO: Set physics mat
            }
        }
    }
}
