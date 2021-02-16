using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using UnityEngine.Events;
using Valve.VR;

/// <summary>
/// Invoke RaiseEvent() whenever the settings are changed. This approach decouples any settings interfaces from targets.
/// GameObjects can even exist between different scenes in this approach.
/// https://www.youtube.com/watch?v=WLDgtRNK2VE
/// </summary>
[CreateAssetMenu(menuName = "Events/SteamVR Input Source Event Channel")]
public class SteamVRInputSourcesEventChannel : ScriptableObject {
    public UnityAction<SteamVR_Input_Sources> onEventRaised;

    // Optionally propogate the signal to another channel
    public VoidEventChannel parentChannel;

    public void RaiseEvent(SteamVR_Input_Sources value) {
        if (onEventRaised != null) {
            onEventRaised.Invoke(value);
        }

        if (parentChannel != null) {
            parentChannel.RaiseEvent();
        }
    }
}
