using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Invoke RaiseEvent() whenever the settings are changed. This approach decouples any settings interfaces from targets.
/// GameObjects can even exist between different scenes in this approach.
/// https://www.youtube.com/watch?v=WLDgtRNK2VE
/// </summary>
[CreateAssetMenu(menuName = "Events/Void Event Channel")]
public class VoidEventChannel : ScriptableObject {
    public UnityAction onEventRaised;

    // Optionally propogate the signal to another channel
    public VoidEventChannel parentChannel;

    public void RaiseEvent() {
        if (onEventRaised != null) {
            Debug.Log(name + " is invoking");
            onEventRaised.Invoke();
        }

        if (parentChannel != null) {
            parentChannel.RaiseEvent();
        }
    }
}
