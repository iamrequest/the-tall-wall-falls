using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Listens for collisions, and sends updates to RopeProjectileManager
/// Assumption: The rope manager component exists in the parent gameobject
/// </summary>
public class RopeProjectile : MonoBehaviour {
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

    private void Awake() {
        m_rb = GetComponent<Rigidbody>();
        m_collider = GetComponent<Collider>();
        m_ropeManager = GetComponentInParent<RopeProjectileManager>();
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

        // TODO: This calculation is wrong
        Vector3 tmp;
        tmp.x = 1 / transform.parent.lossyScale.x;
        tmp.y = 1 / transform.parent.lossyScale.y;
        tmp.z = 1 / transform.parent.lossyScale.z;
        transform.localScale = tmp;

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
