using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GameManagementSFX : MonoBehaviour {
    private AudioSource audioSource;
    public GameStateEventChannel gameStateEventChannel;
    public AudioClip beginSFX, gameStoppedSFX, gameOverSFX;

    private void Awake() {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable() {
        gameStateEventChannel.onEventRaised += UpdateGameState;
    }

    private void OnDisable() {
        gameStateEventChannel.onEventRaised -= UpdateGameState;
    }

    public void UpdateGameState(GameStateEventChannel.GameState gameState) {
        switch (gameState) {
            case GameStateEventChannel.GameState.STARTED:
                audioSource.PlayOneShot(beginSFX);
                break;
            case GameStateEventChannel.GameState.STOPPED:
                audioSource.PlayOneShot(gameStoppedSFX);
                break;
            case GameStateEventChannel.GameState.GAME_OVER:
                audioSource.PlayOneShot(gameOverSFX);
                break;
        }
    }
}
