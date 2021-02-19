using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

/// <summary>
/// Handles the shooting of the RopeProjectile, and the joint management
///
/// TODO: Fix the projectile so that its scale stays the same after un-parenting it from this transform
/// </summary>
public class RopeProjectileManager : MonoBehaviour {
    public SteamVR_Action_Boolean fireRopeAction;
    public SteamVR_Input_Sources inputSource;

    public RopeProjectile ropeProjectile;
    private RopeJointManager ropeJointManager;
    private RopeRenderer ropeRenderer;

    public RopeProjectileState ropeProjectileState = RopeProjectileState.UNFIRED;

    [Range(0.1f, 30f)]
    public float maxProjectileDistance = .1f;
    [Range(0.1f, 2)]
    public float projectileFireDuration = .1f, projectileReturnDuration = .1f;

    // -- Used for lerping back to this transform
    private Vector3 initialReturnPosition;
    private Quaternion initialReturnRotation;
    public float elapsedReturnTime, elapsedFiredTime;

    private void Awake() {
        ropeJointManager = GetComponent<RopeJointManager>();
        ropeRenderer = GetComponentInChildren<RopeRenderer>();
    }

    private void OnEnable() {
        fireRopeAction.AddOnStateDownListener(ShootProjectile, inputSource);
        fireRopeAction.AddOnStateUpListener(DoReturnProjectile, inputSource);
    }

    private void OnDisable() {
        fireRopeAction.RemoveOnStateDownListener(ShootProjectile, inputSource);
        fireRopeAction.RemoveOnStateUpListener(DoReturnProjectile, inputSource);
    }


    /// <summary>
    /// Fires the rope projectile in the direction of transform.forward.
    /// </summary>
    private void ShootProjectile(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
        // Only allow for projectiles to be fired if we're ready to fire
        if (ropeProjectileState != RopeProjectileState.UNFIRED) return;

        ropeProjectileState = RopeProjectileState.FIRED;
        elapsedFiredTime = 0f;

        ropeProjectile.projectileCollider.enabled = true;

        // Un-parent the projectile's transform
        ropeProjectile.transform.parent = null;
        ropeProjectile.transform.localScale = Vector3.one;

        // Add force to the projectile
        // Optional: Add the player's rb velocity too
        ropeProjectile.rb.isKinematic = false;
        ropeProjectile.rb.velocity = Vector3.zero;
        ropeProjectile.rb.AddForce(transform.forward * maxProjectileDistance / projectileFireDuration, ForceMode.VelocityChange);
    }

    /// <summary>
    /// Lerps the rope projectile from it's current position back to transform.position
    /// </summary>
    private void DoReturnProjectile(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
        if (ropeProjectileState == RopeProjectileState.FIRED || ropeProjectileState == RopeProjectileState.ATTACHED) {
            ropeProjectileState = RopeProjectileState.RETURNING;

            // Set the initial return time proportional to how far away the projectile is, compared to the max distance
            elapsedReturnTime = (1 - (ropeProjectile.transform.position - transform.position).magnitude / maxProjectileDistance)
                * projectileReturnDuration;

            initialReturnPosition = ropeProjectile.transform.position;
            initialReturnRotation = ropeProjectile.transform.rotation;

            ropeProjectile.Detach();
            ropeJointManager.DestroyJoint();
        }
    }
    public void ReturnProjectile() {
        DoReturnProjectile(null, SteamVR_Input_Sources.Any);
    }

    public void ReturnProjectileImmediate() {
        ropeProjectile.Detach();
        ropeJointManager.DestroyJoint();

        ropeProjectileState = RopeProjectileState.UNFIRED;

        // Reset projectile transform
        ropeProjectile.transform.parent = transform;
        ropeProjectile.transform.position = transform.position;
        ropeProjectile.transform.rotation = transform.rotation;
    }

    public void OnProjectileConnected() {
        Rigidbody targetRB;

        // Try to find a rigidbody in the gameobject we just collided with
        targetRB = ropeProjectile.transform.parent.GetComponent<Rigidbody>();
        if (targetRB != null) {
            ropeProjectileState = RopeProjectileState.ATTACHED;

            ropeJointManager.AddJoint(targetRB, ropeProjectile.transform);
        } else {
            // If that fails, try to find it in the parent(s)
            targetRB = ropeProjectile.transform.parent.GetComponentInParent<Rigidbody>();
            if (targetRB != null) {
                ropeProjectileState = RopeProjectileState.ATTACHED;

                ropeJointManager.AddJoint(targetRB, ropeProjectile.transform);
            }
        }
    }

    private void Update() {
        if (ropeProjectileState == RopeProjectileState.FIRED) {
            // TODO: Fix the lerp here so that it only goes from 0->0.5. On connect, go from 0.5->1
            ropeRenderer.offsetTime = elapsedFiredTime / projectileFireDuration;
        } else {
            ropeRenderer.offsetTime = 0f;
        }

        SetRopeColor();
    }

    private void FixedUpdate() {
        if (ropeProjectileState == RopeProjectileState.FIRED) {
            // If the projectile has been fired for long enough, then start lerping it back to this transform
            elapsedFiredTime += Time.fixedDeltaTime;

            if (elapsedFiredTime > projectileFireDuration) {
                ReturnProjectile();
            }
        }

        if (ropeProjectileState == RopeProjectileState.RETURNING) {
            // Lerp the projectile back to the transform
            elapsedReturnTime += Time.fixedDeltaTime;
            ropeProjectile.rb.MovePosition(Vector3.Lerp(initialReturnPosition, transform.position, elapsedReturnTime / projectileReturnDuration));
            ropeProjectile.rb.MoveRotation(Quaternion.Lerp(initialReturnRotation, transform.rotation, elapsedReturnTime / projectileReturnDuration));

            // Return to the "Unfired" state
            if (elapsedReturnTime >= projectileReturnDuration) {
                ropeProjectileState = RopeProjectileState.UNFIRED;

                // Reset projectile transform
                ropeProjectile.transform.parent = transform;
                ropeProjectile.transform.position = transform.position;
                ropeProjectile.transform.rotation = transform.rotation;
            }
        }
    }

    private void SetRopeColor() {
        if (ropeProjectileState != RopeProjectileState.UNFIRED) {
            // Lerp the rope color based on rope tension.
            // If a joint exists, then the max distance would be the joint's max distance. Otherwise, just use the max distance that we can shoot a projectile
            ropeRenderer.ropeColorT = (ropeRenderer.transform.position - ropeProjectile.transform.position).magnitude / 
                (ropeJointManager.joint != null ? ropeJointManager.joint.maxDistance : maxProjectileDistance);
        }
    }
}
