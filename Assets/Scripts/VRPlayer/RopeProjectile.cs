using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Awake() {
        m_rb = GetComponent<Rigidbody>();
        m_collider = GetComponent<Collider>();
    }

    private void OnCollisionEnter(Collision collision) {
        // Stop all motion
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;

        // Parent to the collider
        // TODO: Confirm that this won't cause any issues with nested rigidbodies
        transform.parent = collision.collider.transform;
        projectileCollider.enabled = false;
    }
}
