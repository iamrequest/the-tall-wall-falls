using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class NPCHealthManager : MonoBehaviour {
    public bool isAlive = true;
    public bool isRagdollActive = false;

    private Animator animator;
    public List<Rigidbody> limbs;

    private void Awake() {
        animator = GetComponent<Animator>();

        limbs.AddRange(GetComponentsInChildren<Rigidbody>());
        SetRagdollEnabled(isRagdollActive);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            SetRagdollEnabled(!isRagdollActive);
        }
    }

    public void SetRagdollEnabled(bool value) {
        isRagdollActive = value;
        animator.enabled = !value;

        foreach (Rigidbody limb in limbs) {
            limb.isKinematic = !value;
        }
    }
}
