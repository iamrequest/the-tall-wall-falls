using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SwordManager : MonoBehaviour {
    private PhysicsHand physicsHand;
    private AudioSource audioSource;

    public GameStateEventChannel gameStateEventChannel;

    // -- Settings menu: Hide the sword/rope if the settings menu is open
    // Can't fire a projectile if the settings menu is open
    public BoolEventChannel settingsMenuOpenedChannel;
    private bool isSettingsMenuOpen;
    public bool hideSwordOnSettingsMenuOpened;

    private RopeProjectileManager ropeProjectileManager;
    public GameObject swordBlade, swordBladeBroken, swordHandle;
    public GameObject settingsMenuInteractor;

    public List<AudioClip> swordImpactSFX;

    [Header("Sword Health")]
    [Range(1, 10f)]
    public int swordMaxHealth;
    public int swordHealth;
    public LayerMask swordDamageLayers;

    [Range(0f, 5f)]
    public float swordIFrames;
    [HideInInspector]
    public float elapsedSwordIFrames;

    private void Awake() {
        ropeProjectileManager = GetComponentInChildren<RopeProjectileManager>();
        physicsHand = GetComponentInParent<PhysicsHand>();
        audioSource = GetComponent<AudioSource>();

        swordHealth = swordMaxHealth;
        elapsedSwordIFrames = swordIFrames;
        SetSwordEnabled(true);
    }
    private void OnEnable() {
        settingsMenuOpenedChannel.onEventRaised += setIsSettingsMenuOpen;
        gameStateEventChannel.onEventRaised += RepairSwordOnGameStart;
    }

    private void OnDisable() {
        settingsMenuOpenedChannel.onEventRaised -= setIsSettingsMenuOpen;
        gameStateEventChannel.onEventRaised -= RepairSwordOnGameStart;
    }

    private void Update() {
        if (elapsedSwordIFrames <= swordIFrames) {
            elapsedSwordIFrames += Time.deltaTime;
        }
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
        if (isSwordEnabled) {
            swordBlade.SetActive(swordHealth > 0);
            swordBladeBroken.SetActive(swordHealth <= 0);
        } else {
            swordBlade.SetActive(false);
            swordBladeBroken.SetActive(false);
        }
        swordHandle.SetActive(isSwordEnabled);

        // This is a hacky workaround to fix a bug where the physics hand touch count gets out of sync when the settings menu gets opened/closed
        //  This is a problem with SetActive() happening, and PhysicsHand.OnCollisionExit() not happening
        physicsHand.ResetTouchCount();

        ropeProjectileManager.enabled = isSwordEnabled;

        settingsMenuInteractor.SetActive(!isSwordEnabled);
    }

    private void OnCollisionEnter(Collision collision) {
        audioSource.PlayOneShot(swordImpactSFX[Random.Range(0, swordImpactSFX.Count)]);

        /*
        // If the sword is no longer invincible
        if (elapsedSwordIFrames >= swordIFrames) {
            Debug.Log("Sword impact");
            if(((1<<collision.collider.gameObject.layer) & swordDamageLayers) == 0) {
                Debug.Log("Sword damage");
                swordHealth--;
                elapsedSwordIFrames = 0f;

                if (swordHealth <= 0) {
                    BreakSword();
                }
            }
        }
        */
    }

    public void BreakSword() {
        swordBlade.SetActive(false);
        swordBladeBroken.SetActive(true);

        // TODO: Play SFX
    }
    public void RepairSword() {
        swordBlade.SetActive(true);
        swordBladeBroken.SetActive(false);

        swordHealth = swordMaxHealth;
        elapsedSwordIFrames = swordIFrames;

        // TODO: Play SFX
    }

    private void RepairSwordOnGameStart(GameStateEventChannel.GameState gameState) {
        if (gameState == GameStateEventChannel.GameState.STARTED) {
            //RepairSword();
        }
    }

    public void ApplyDamage() {
        swordHealth--;
        elapsedSwordIFrames = 0f;

        if (swordHealth <= 0) {
            //BreakSword();
        }
    }
}
