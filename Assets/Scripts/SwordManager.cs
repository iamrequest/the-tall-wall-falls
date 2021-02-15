using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordManager : MonoBehaviour
{
    // Can't fire a projectile if the settings menu is open
    public BoolEventChannel settingsMenuOpenedChannel;
    private bool isSettingsMenuOpen;

    private RopeProjectileManager ropeProjectileManager;
    public GameObject swordBlade, swordBladeBroken, swordHandle;
    public GameObject settingsMenuInteractor;

    private void Awake() {
        ropeProjectileManager = GetComponentInChildren<RopeProjectileManager>();
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

        SetSwordEnabled(!isSettingsMenuOpen);
    }

    /// <summary>
    /// Alternates between the sword being active, and a hand model being active. The hand model is able to interface with the settings menu
    /// </summary>
    /// <param name="isSwordEnabled"></param>
    public void SetSwordEnabled(bool isSwordEnabled) {
        swordBlade.SetActive(isSwordEnabled);
        swordBladeBroken.SetActive(isSwordEnabled);
        swordHandle.SetActive(isSwordEnabled);

        ropeProjectileManager.enabled = isSwordEnabled;

        settingsMenuInteractor.SetActive(!isSwordEnabled);
    }
}
