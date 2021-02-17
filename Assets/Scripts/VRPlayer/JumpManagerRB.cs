﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

/// <summary>
/// Source: https://catlikecoding.com/unity/tutorials/movement/physics/
///     https://bitbucket.org/catlikecodingunitytutorials/movement-02-physics/src/master/Assets/Scripts/MovingSphere.cs
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class JumpManagerRB : MonoBehaviour {
    private Rigidbody rb;
    public SteamVR_Action_Boolean jumpAction;

    [Range(0f, 10f)]
    public float jumpHeight;
    private bool jumpBuffered;
    private int maxJumpCount = 1;
    private int jumpCount;

    private int groundCount = 0, nonGroundCount = 0;

    public bool isTouching {
        get {
            return nonGroundCount > 0;
        }
    } 
    public bool isTouchingGround {
        get {
            return groundCount > 0;
        }
    }
    public bool isInCoyoteTime{
        get {
            return elapsedCoyoteTime < coyoteTime;
        }
    }

    [HideInInspector]
    // Non-normalized
    private Vector3 groundNormal, nonGroundNormal, coyoteNormal;

    [Range(0f, 90f)]
    public float maxGroundAngle;
    private float minGroundDotProduct;

    [Range(0f, 2f)]
    [Tooltip("Extra delay before unsetting isGrounded")]
    public float coyoteTime;
    private float elapsedCoyoteTime;

    private void OnValidate() {
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
    }

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        OnValidate();
    }

    private void Update() {
        // Check if the player pressed the jump button this frame. Process it later in FixedUpdate()
        jumpBuffered |= jumpAction.GetState(SteamVR_Input_Sources.Any);
    }

    private void FixedUpdate() {
        if (jumpBuffered && jumpCount < maxJumpCount) {
            jumpCount++;

            // Cache the normal for coyote time
            coyoteNormal = GetJumpNormal();

            float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
            rb.velocity += coyoteNormal * jumpSpeed;
        }

        // -- Coyote Time
        // TODO: Need to decouple coyote time from isTouchingGround
        if (isTouchingGround) {
            elapsedCoyoteTime = 0f;
            jumpCount = 0;
        } else {
            elapsedCoyoteTime += Time.fixedDeltaTime;
        }

        UnsetGroundCheck();

        jumpBuffered = false;
    }

    /// <summary>
    /// Each physics frame we unset the calculated normals from the previous frame. During coyote time, we may want to delay this for a brief moment.
    /// </summary>
    private void UnsetGroundCheck() {
        groundCount = 0;
        nonGroundCount = 0;
        groundNormal = Vector3.zero;
        nonGroundNormal = Vector3.zero;
    }



    public Vector3 GetJumpNormal() {
        if (isTouchingGround) {
            // -- Touching ground
            // Normalize if there's more than 1 ground collider we're touching
            if (groundCount > 1) {
                return groundNormal.normalized;
            } else {
                return groundNormal;
            }

        } else if (isInCoyoteTime) {
            // If we're in the middle of coyote time, just use the normal that we cached from last frame
            return coyoteNormal;

        } else if (isTouching) {
            // -- Not touching ground, but we're touching a wall
            // Normalize if there's more than 1 collider we're touching
            if (nonGroundCount > 1) {
                return groundNormal.normalized;
            } else {
                return nonGroundNormal;
            }

        } else {
            // -- In air
            return Vector3.up;
        }
    }

    private void OnCollisionEnter(Collision collision) {
        EvaluateCollision(collision);
    }

    private void OnCollisionStay(Collision collision) {
        EvaluateCollision(collision);
    }

    private void EvaluateCollision(Collision collision) {
        for (int i = 0; i < collision.contactCount; i++) {
            Vector3 normal = collision.GetContact(i).normal;

            if (normal.y >= minGroundDotProduct) {
                groundCount++;
                groundNormal += normal;
            } else {
                nonGroundCount++;
                nonGroundNormal += normal;
            }
        }
    }
}
