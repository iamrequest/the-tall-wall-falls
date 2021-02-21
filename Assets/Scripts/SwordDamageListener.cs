using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordDamageListener : MonoBehaviour {
    private SwordManager swordManager;

    private void Awake() {
        swordManager = GetComponentInParent<SwordManager>();
    }

    private void OnCollisionEnter(Collision collision) {
        Debug.Log(collision.collider);
        // If the sword is no longer invincible
        if (swordManager.elapsedSwordIFrames >= swordManager.swordIFrames) {
                Debug.Log("Applying damage 1");
            if(((1<<collision.collider.gameObject.layer) & swordManager.swordDamageLayers) == 0) {
                Debug.Log("Applying damage 2");
                swordManager.ApplyDamage();
            }
        }
    }

}
