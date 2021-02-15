using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Invoke RaiseEvent() whenever the settings are changed. This approach decouples any settings interfaces from targets.
/// GameObjects can even exist between different scenes in this approach.
/// https://www.youtube.com/watch?v=WLDgtRNK2VE
/// </summary>
[CreateAssetMenu(menuName = "Events/Bool Event Channel")]
public class BoolEventChannel : ScriptableObject {
    public UnityAction<bool> onEventRaised;

    public void RaiseEvent(bool value) {
        if (onEventRaised != null) {
            onEventRaised.Invoke(value);
        }
    }
}
