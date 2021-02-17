using System.Collections;
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

    [HideInInspector]
    // Non-normalized
    private Vector3 groundNormal, nonGroundNormal;

    [Range(0f, 90f)]
    public float maxGroundAngle;
    private float minGroundDotProduct;

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
        if (jumpBuffered && isTouchingGround) {
            float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
            rb.velocity += GetJumpNormal() * jumpSpeed;
        }

        jumpBuffered = false;
        groundCount = 0;
        nonGroundCount = 0;
        groundNormal = Vector3.zero;
        nonGroundNormal = Vector3.zero;
    }



    public Vector3 GetJumpNormal() {
        if (isTouchingGround) {
            // -- Touching ground
            if (groundCount > 1) {
                return groundNormal.normalized;
            } else {
                return groundNormal;
            }
        } else if (isTouching) {
            // -- Not touching ground, but we're touching a wall
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
