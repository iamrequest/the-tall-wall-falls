using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Listens for collisions, and sends updates to RopeProjectileManager
/// Assumption: The rope manager component exists in the parent gameobject
/// </summary>
public class RopeProjectile : MonoBehaviour {
    [HideInInspector]
    public Vector3 originalLocalScale, originalLossyScale;

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
    private RopeManager m_ropeManager;
    public RopeManager ropeManager {
        get {
            return m_ropeManager;
        }
    }

    private void Awake() {
        m_rb = GetComponent<Rigidbody>();
        m_collider = GetComponent<Collider>();
        m_ropeManager = GetComponentInParent<RopeManager>();
        originalLocalScale = transform.localScale;
        originalLossyScale = transform.lossyScale;
    }

    private void OnCollisionEnter(Collision collision) {
        // Only can hook onto something if this projectile is in the air and firing
        if (ropeManager.ropeProjectileState != RopeProjectileState.FIRED) return;

        // TODO: Confirm that this won't cause any issues with nested rigidbodies
        Attach(collision.collider.transform);
    }

    // TODO: Lerp towards the collision normal rotation
    public void Attach(Transform target) {
        projectileCollider.enabled = false;

        // Parent to the collider
        transform.parent = target;

        // Stop all motion
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;

        ropeManager.OnProjectileConnected();
    }

    public void Detach() {
        transform.localScale = originalLocalScale;
        transform.parent = null;

        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
    }
}
