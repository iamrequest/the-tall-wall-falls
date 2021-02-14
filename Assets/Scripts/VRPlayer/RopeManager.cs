using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class RopeManager : MonoBehaviour {
    private SpringJoint joint;
    private Rigidbody rb;
    public SteamVR_Action_Boolean fireRopeAction;
    public SteamVR_Input_Sources inputSource;

    [Tooltip("The start and end transforms of the rope")]
    public Transform localAnchorTransform, targetAnchorTransform;

    public RopeProjectile ropeProjectile;
    [Range(0f, 10f)]
    public float projectileSpeed;

    private void Awake() {
        rb = GetComponentInParent<Rigidbody>();
    }

    private void OnEnable() {
        fireRopeAction.AddOnStateDownListener(ShootProjectile, inputSource);
    }

    private void OnDisable() {
        fireRopeAction.RemoveOnStateDownListener(ShootProjectile, inputSource);
    }

    private void ShootProjectile(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
        // Reset transform
        // TODO: Perform this in the reset method
        ropeProjectile.transform.position = transform.position;
        ropeProjectile.transform.rotation = transform.rotation;
        ropeProjectile.projectileCollider.enabled = true;

        // Un-parent the projectile's transform
        ropeProjectile.transform.parent = null;

        // Add force to the projectile
        ropeProjectile.rb.isKinematic = false;
        ropeProjectile.rb.velocity = Vector3.zero;

        ropeProjectile.rb.AddForce(transform.forward * projectileSpeed, ForceMode.VelocityChange);
    }
}
