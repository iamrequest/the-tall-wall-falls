using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Listens for collisions, and sends updates to RopeProjectileManager
/// Assumption: The rope manager component exists in the parent gameobject
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class RopeProjectile : MonoBehaviour {
    [HideInInspector]
    public AudioSource audioSource;
    private Rigidbody m_rb;
    public Rigidbody rb {
        get {
            return m_rb;
        }
    }
    private Collider m_collider;
    public Collider projectileCollider {
        get {
            return m_collider;
        }
    }
    private RopeProjectileManager m_ropeManager;
    public RopeProjectileManager ropeManager {
        get {
            return m_ropeManager;
        }
    }

    [Range(0f, 1f)]
    [Tooltip("The total duration it takes for the projectile to lerp to match the collision normal")]
    public float attachRotationLerpDuration;
    // The lerp time between the initial rotation on collision, and the final collisionNormalRotation.
    private float attachRotationLerpTime;

    private Quaternion collisionNormalRotation, initialCollisionRotation;

    private void Awake() {
        m_rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        m_collider = GetComponent<Collider>();
        m_ropeManager = GetComponentInParent<RopeProjectileManager>();
    }

    private void FixedUpdate() {
        // When the projectile is attached to something, lerp its rotation to match the target's collision normal
        if (ropeManager.ropeProjectileState == RopeProjectileState.ATTACHED && attachRotationLerpTime < attachRotationLerpDuration) {
            attachRotationLerpTime += Time.fixedDeltaTime;
            transform.rotation = Quaternion.Slerp(initialCollisionRotation, collisionNormalRotation, attachRotationLerpTime / attachRotationLerpDuration);
        }
    }

    private void OnCollisionEnter(Collision collision) {
        // Only can hook onto something if this projectile is in the air and firing
        if (ropeManager.ropeProjectileState != RopeProjectileState.FIRED) return;

        
        // TODO: Confirm that this won't cause any issues with nested rigidbodies
        Attach(collision.collider.transform, collision.GetContact(0).normal);
    }

    public void Attach(Transform target, Vector3 collisionNormal) {
        projectileCollider.enabled = false;

        // Prepare to lerp from the current rotation to the collision normal rotation
        attachRotationLerpTime = 0f;
        initialCollisionRotation = transform.rotation;
        collisionNormalRotation = Quaternion.LookRotation(-collisionNormal, Vector3.up);

        // Parent to the collider
        transform.parent = target;

        // TODO: This calculation is wrong?
        //transform.localScale = transform.parent.lossyScale * .1f;

        // Stop all motion
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;

        ropeManager.OnProjectileConnected();
    }

    public void Detach() {
        transform.parent = null;
        transform.localScale = Vector3.one;

        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
    }
}
