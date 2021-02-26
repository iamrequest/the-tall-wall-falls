using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

/// <summary>
/// Mostly used for interacting with the settings menu slider
/// </summary>
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(LineRenderer))]
public class SettingsMenuInteractor : MonoBehaviour {
    private MeshRenderer meshRenderer;
    private LineRenderer lineRenderer;
    public SteamVR_Action_Boolean menuInteractionAction;
    public SteamVR_Input_Sources inputSource;

    public bool hideMeshWhenNotPointingAtUI, hideLineRendererWhenNotPointingAtUI;
    public bool allowRaycastSliderInteractions;
    public float raycastDistance;
    public LayerMask uiLayer;

    [HideInInspector]
    public SettingsMenuSlider hoveredSlider, grabbedSlider, pointingAtSlider;

    [TextArea]
    [Tooltip("Read-only note. No effect on gameplay.")]
    public String README = "Settings interactors should be on the Water layer. I'm re-purposing this layer, since it has no usage in this game";
    private void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();
        lineRenderer = GetComponent<LineRenderer>();

        if (hideMeshWhenNotPointingAtUI) {
            meshRenderer.enabled = false;
        }
        lineRenderer.enabled = false;
    }


    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.TryGetComponent(out SettingsMenuSlider slider)) {
            if (slider.isActiveAndEnabled) {
                hoveredSlider = slider;
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        hoveredSlider = null;
    }

    private void FixedUpdate() {
        if (hideMeshWhenNotPointingAtUI || hideLineRendererWhenNotPointingAtUI) {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, raycastDistance, uiLayer)) {
                if (hideMeshWhenNotPointingAtUI) {
                    meshRenderer.enabled = true;
                }

                if (hideLineRendererWhenNotPointingAtUI) {
                    // Drawing in world space, since drawing in local space applies this transform's scale
                    lineRenderer.SetPosition(0, transform.position);
                    lineRenderer.SetPosition(1, hit.point);
                    lineRenderer.enabled = true;
                }

                hit.collider.TryGetComponent(out pointingAtSlider);
            } else {
                meshRenderer.enabled = false;
                lineRenderer.enabled = false;
            }
        }

        // If we're holding a slider
        if (grabbedSlider) {
            // If we let go of the slider just now
            if (menuInteractionAction.GetStateUp(inputSource)) {
                ReleaseSliderGrab();
            }

            // Lerp slider to hand
            if (grabbedSlider) {
                grabbedSlider.LerpSlider(transform);
                grabbedSlider.SendUpdatesToEventChannel();
            }
        } else if (hoveredSlider) {
            // If we're hovering a slider, then start grabbing it
            if (menuInteractionAction.GetStateDown(inputSource)) {
                // Only start grabbing it if we're not currently holding something
                if (hoveredSlider.interactor == null) {
                    StartSliderGrab(hoveredSlider);
                }
            }
        } else if (allowRaycastSliderInteractions && pointingAtSlider) {
            // If we're pointing at a slider, then start grabbing it
            if (menuInteractionAction.GetStateDown(inputSource)) {
                // Only start grabbing it if we're not currently holding something
                if (pointingAtSlider.interactor == null) {
                    // TODO: Slider position isn't calculated properly for raycast slider grabs, since I'm projecting the midway point onto the settings menu plane, instead of using the raycast hit point.
                    StartSliderGrab(pointingAtSlider);
                }
            }
        }
    }

    private void StartSliderGrab(SettingsMenuSlider slider) {
        grabbedSlider = slider;
        slider.interactor = this;
    }

    public void ReleaseSliderGrab() {
        if (grabbedSlider) {
            grabbedSlider.Release();
        }

        grabbedSlider = null;
    }
}
