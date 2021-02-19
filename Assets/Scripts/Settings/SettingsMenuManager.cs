using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

/// <summary>
/// Whereas SettingsManager handles the actual settings, this class handles the opening/closing/positioning of the actual settings menu
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class SettingsMenuManager : MonoBehaviour {
    [Header("General Menu Management")]
    public BoolEventChannel menuOpenedChannel;
    private bool isPlacingMenu = false, isMenuOpen = false;
    private Animator animator;
    public SteamVR_Action_Boolean toggleSettingsMenuAction;

    public Transform targetTransform;
    public float lerpSpeed;
    public float closeMenuDistance;


    private AudioSource audioSource;
    public AudioClip placingMenuSFX, menuOpenSFX, menuClosedSFX;

    [Header("Tabs")]
    public List<SettingsTab> tabs;
    public int intialTabIndex;
    public Sprite leftActiveTabSprite, middleActiveTabSprite, rightActiveTabSprite;
    public Sprite leftInactiveTabSprite, middleInactiveTabSprite, rightInactiveTabSprite;

    private void Awake() {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        SetTab(intialTabIndex);
    }

    private void OnEnable() {
        toggleSettingsMenuAction.AddOnChangeListener(ToggleSettingsMenu, SteamVR_Input_Sources.Any);
    }

    private void OnDisable() {
        toggleSettingsMenuAction.RemoveOnChangeListener(ToggleSettingsMenu, SteamVR_Input_Sources.Any);
    }

    private void ToggleSettingsMenu(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState) {
        // This can probably be optimized, but it's late and I don't wanna
        if (isMenuOpen && newState) {
            CloseMenu();
        } else if (isPlacingMenu && !newState) {
            OpenMenu();
        } else if(!isPlacingMenu && !isMenuOpen && newState) {
            BeginPlacingMenu();
        }
    }

    private void UpdateAnimator() {
        animator.SetBool("isPlacingMenu", isPlacingMenu);
        animator.SetBool("isMenuOpen", isMenuOpen);
    }

    public void BeginPlacingMenu() {
        // Begin placing the menu
        isPlacingMenu = true;
        isMenuOpen = false;

        audioSource.PlayOneShot(placingMenuSFX);

        UpdateAnimator();
    }
    public void OpenMenu() {
        // Finish placing the menu, and open it
        isPlacingMenu = false;
        isMenuOpen = true;

        audioSource.PlayOneShot(menuOpenSFX);

        menuOpenedChannel.RaiseEvent(true);
        UpdateAnimator();
    }
    public void CloseMenu() {
        isPlacingMenu = false;
        isMenuOpen = false;

        audioSource.PlayOneShot(menuClosedSFX);

        menuOpenedChannel.RaiseEvent(false);
        UpdateAnimator();
    }

    private void FixedUpdate() {
        // If we're placing the menu, lerp towards the target transform
        if (isPlacingMenu) {
            // TODO: Also rotate to face the player
            transform.position = Vector3.Lerp(transform.position, targetTransform.position, lerpSpeed);
        }

        // Close the menu if we're far away from it
        if (isMenuOpen) {
            if (closeMenuDistance < (transform.position - targetTransform.position).magnitude) {
                CloseMenu();
            }
        }
    }

    public void SetTab(int tabIndex) {
        foreach (SettingsTab tab in tabs) {
            tab.page.SetActive(false);
            tab.SetTabSprite(leftInactiveTabSprite, middleInactiveTabSprite, rightInactiveTabSprite);
        }

        tabs[tabIndex].page.SetActive(true);
        tabs[tabIndex].SetTabSprite(leftActiveTabSprite, middleActiveTabSprite, rightActiveTabSprite);
    }
}
