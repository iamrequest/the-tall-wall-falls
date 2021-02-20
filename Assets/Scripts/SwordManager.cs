using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SwordManager : MonoBehaviour {
    private PhysicsHand physicsHand;
    private AudioSource audioSource;

    // -- Settings menu: Hide the sword/rope if the settings menu is open
    // Can't fire a projectile if the settings menu is open
    public BoolEventChannel settingsMenuOpenedChannel;
    private bool isSettingsMenuOpen;
    public bool hideSwordOnSettingsMenuOpened;

    private RopeProjectileManager ropeProjectileManager;
    public GameObject swordBlade, swordBladeBroken, swordHandle;
    public GameObject settingsMenuInteractor;

    public List<AudioClip> swordImpactSFX;

    private void Awake() {
        ropeProjectileManager = GetComponentInChildren<RopeProjectileManager>();
        physicsHand = GetComponentInParent<PhysicsHand>();
        audioSource = GetComponent<AudioSource>();
        SetSwordEnabled(true);
    }
    private void OnEnable() {
        settingsMenuOpenedChannel.onEventRaised += setIsSettingsMenuOpen;
    }

    private void OnDisable() {
        settingsMenuOpenedChannel.onEventRaised -= setIsSettingsMenuOpen;
    }


    private void setIsSettingsMenuOpen(bool isSettingsMenuOpen) {
        this.isSettingsMenuOpen = isSettingsMenuOpen;

        if (isSettingsMenuOpen) {
            ropeProjectileManager.ReturnProjectileImmediate();
        }

        // Optional: Don't hide the sword if the settings menu is open. Useful for viewing sword angle without closing the menu
        if (hideSwordOnSettingsMenuOpened) {
            SetSwordEnabled(!isSettingsMenuOpen);
        }
    }

    /// <summary>
    /// Alternates between the sword being active, and a hand model being active. The hand model is able to interface with the settings menu
    /// </summary>
    /// <param name="isSwordEnabled"></param>
    public void SetSwordEnabled(bool isSwordEnabled) {
        swordBlade.SetActive(isSwordEnabled);
        swordBladeBroken.SetActive(isSwordEnabled);
        swordHandle.SetActive(isSwordEnabled);

        // This is a hacky workaround to fix a bug where the physics hand touch count gets out of sync when the settings menu gets opened/closed
        //  This is a problem with SetActive() happening, and PhysicsHand.OnCollisionExit() not happening
        physicsHand.ResetTouchCount();

        ropeProjectileManager.enabled = isSwordEnabled;

        settingsMenuInteractor.SetActive(!isSwordEnabled);
    }

    private void OnCollisionEnter(Collision collision) {
        audioSource.PlayOneShot(swordImpactSFX[Random.Range(0, swordImpactSFX.Count)]);
    }
}
