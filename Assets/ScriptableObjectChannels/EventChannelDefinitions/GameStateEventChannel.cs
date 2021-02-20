using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

/// <summary>
/// Invoke RaiseEvent() whenever the settings are changed. This approach decouples any settings interfaces from targets.
/// GameObjects can even exist between different scenes in this approach.
/// https://www.youtube.com/watch?v=WLDgtRNK2VE
/// </summary>
[CreateAssetMenu(menuName = "Events/GameState Event Channel")]
public class GameStateEventChannel : ScriptableObject {
    public enum GameState { STOPPED, STARTED, GAME_OVER};

    public UnityAction<GameState> onEventRaised;

    // Optionally propogate the signal to another channel
    public VoidEventChannel parentChannel;

    public void RaiseEvent(GameState value) {
        if (onEventRaised != null) {
            onEventRaised.Invoke(value);
        }

        if (parentChannel != null) {
            parentChannel.RaiseEvent();
        }
    }
}
