using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public GameObject gate;
    public float gateHealth;

    [field: SerializeField]
    public GameStateEventChannel.GameState gameState { get; private set; } = GameStateEventChannel.GameState.STOPPED;
    public GameStateEventChannel gameStateEventChannel;
    private void OnEnable() {
        gameStateEventChannel.onEventRaised += UpdateGameState;
    }

    private void OnDisable() {
        gameStateEventChannel.onEventRaised -= UpdateGameState;
    }

    private void Awake() {
    }


    public void UpdateGameState(GameStateEventChannel.GameState gameState) {
    }
}
