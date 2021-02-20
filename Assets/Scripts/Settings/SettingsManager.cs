using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Valve.VR;

/// <summary>
/// Handles the actual propogation of settings data
/// </summary>
public class SettingsManager : MonoBehaviour {
    [Header("Channels")]
    public VoidEventChannel settingsUpdatedChannel;
    public VoidEventChannel vignetteChangedChannel;
    public VoidEventChannel speedLinesChangedChannel;
    public VoidEventChannel enemyKillsChangedChannel;
    public SliderEventChannel swordAngleChangedChannel;
    public SliderEventChannel grappleAngleChangedChannel;
    public SliderEventChannel vignetteStrengthChangedChannel;
    public SliderEventChannel ropePullSpeedChangedChannel;
    public SteamVRInputSourcesEventChannel steeringTransformChangedChannel;
    public GameStateEventChannel gameStateChangedChannel;

    [Header("Sprites")]
    public Sprite spriteEnabled;
    public Sprite spriteDisabled;
    public Sprite spritePlay, spriteStop;
    public Sprite spriteHMD, spriteLeftHand, spriteRightHand;

    // ----------------------
    // -- Page 1: Settings -- 
    // ----------------------
    // -- Vignette
    [Header("GUI - Page 1")]
    public TextMeshProUGUI vignetteEnabledText;
    public Image vignetteEnabledImage;
    private bool vignetteEnabled = true;

    // -- Speed lines
    public TextMeshProUGUI speedLinesEnabledText;
    public Image speedLinesEnabledImage;
    private bool speedLinesEnabled = true;

    // -- Sword Angle
    public TextMeshProUGUI swordAngleText;
    public SettingsMenuSlider swordAngleSlider;
    public float swordAngleScaled { get; private set; }

    // -- GrappleAngle
    public TextMeshProUGUI grappleAngleText;
    public SettingsMenuSlider grappleAngleSlider;
    public float grappleAngleScaled { get; private set; }

    // -- Vignette Strength
    public TextMeshProUGUI vignetteStrengthText;
    public SettingsMenuSlider vignetteStrengthSlider;
    public float vignetteStrengthScaled { get; private set; }

    // -- Rope Pull Speed
    public TextMeshProUGUI ropePullSpeedText;
    public SettingsMenuSlider ropePullSpeedSlider;
    public float ropePullSpeedScaled { get; private set; }

    // -- Steering Transform
    public SteamVR_Input_Sources steeringTransformInputSource;
    public Image steeringTransformImage;



    // ------------------
    // -- Page 2: Game -- 
    // ------------------
    [Header("GUI - Page 2")]
    public Image gameStartStopButton;
    public Image gameStartStopButtonBG;
    public TextMeshProUGUI gameStateText, gameStartStopButtonText;
    public Color gameStartedColor, gameStoppedColor;
    public GameStateEventChannel.GameState gameState { get; private set; }


    public TextMeshProUGUI numKillsText;
    public int numEnemiesKilled { get; private set; }

    public TextMeshProUGUI timeAliveText;
    public float timeAlive { get; private set; }

    private void Start() {
        swordAngleSlider.ResetSlider();
        grappleAngleSlider.ResetSlider();
        vignetteStrengthSlider.ResetSlider();
        ropePullSpeedSlider.ResetSlider();

        UpdateGUI();
    }

    private void Update() {
        if (gameState == GameStateEventChannel.GameState.STARTED) {
            // Using in-game time, rather than unscaled time
            timeAlive += Time.deltaTime;
        }
        UpdateTimeAliveText();
    }

    private void OnEnable() {
        settingsUpdatedChannel.onEventRaised += UpdateGUI;
        vignetteChangedChannel.onEventRaised += ToggleVignette;
        speedLinesChangedChannel.onEventRaised += ToggleSpeedLines;
        swordAngleChangedChannel.onEventRaised += GetSwordAngle;
        grappleAngleChangedChannel.onEventRaised += GetGrappleAngle;
        vignetteStrengthChangedChannel.onEventRaised += GetVignetteStrength;
        ropePullSpeedChangedChannel.onEventRaised += GetRopePullSpeed;
        steeringTransformChangedChannel.onEventRaised += GetSteeringTransform;
        gameStateChangedChannel.onEventRaised += UpdateGameState;
        enemyKillsChangedChannel.onEventRaised += UpdateEnemiesKilled;
    }

    private void OnDisable() {
        settingsUpdatedChannel.onEventRaised -= UpdateGUI;
        vignetteChangedChannel.onEventRaised -= ToggleVignette;
        speedLinesChangedChannel.onEventRaised -= ToggleSpeedLines;
        swordAngleChangedChannel.onEventRaised -= GetSwordAngle;
        grappleAngleChangedChannel.onEventRaised -= GetGrappleAngle;
        vignetteStrengthChangedChannel.onEventRaised -= GetVignetteStrength;
        ropePullSpeedChangedChannel.onEventRaised -= GetRopePullSpeed;
        steeringTransformChangedChannel.onEventRaised -= GetSteeringTransform;
        gameStateChangedChannel.onEventRaised -= UpdateGameState;
        enemyKillsChangedChannel.onEventRaised -= UpdateEnemiesKilled;
    }

    public void UpdateGUI() {
        // -- Vignette
        if (vignetteEnabled) {
            vignetteEnabledText.text = "Vignette: Enabled";
            vignetteEnabledImage.sprite = spriteEnabled;
        } else {
            vignetteEnabledText.text = "Vignette: Disabled";
            vignetteEnabledImage.sprite = spriteDisabled;
        }

        // -- Speed lines
        if (speedLinesEnabled) {
            speedLinesEnabledText.text = "Speed Lines: Enabled";
            speedLinesEnabledImage.sprite = spriteEnabled;
        } else {
            speedLinesEnabledText.text = "Speed Lines: Disabled";
            speedLinesEnabledImage.sprite = spriteDisabled;
        }

        // -- Sword angle
        if (swordAngleScaled >= 0f) {
            swordAngleText.text = "Sword Angle: +" + swordAngleScaled.ToString("F0") + " degrees";
        } else {
            swordAngleText.text = "Sword Angle: " + swordAngleScaled.ToString("F0") + " degrees";
        }

        // -- Grapple angle
        if (grappleAngleScaled >= 0f) {
            grappleAngleText.text = "Grapple Angle: +" + grappleAngleScaled.ToString("F0") + " degrees";
        } else {
            grappleAngleText.text = "Grapple Angle: " + grappleAngleScaled.ToString("F0") + " degrees";
        }

        // -- Vignette Strength
        vignetteStrengthText.text = "Vignette Strength: " + (vignetteStrengthScaled * 100).ToString("F0") + "%";

        // -- Rope Pull Speed
        ropePullSpeedText.text = "Rope Pull Speed: " + (ropePullSpeedScaled).ToString("F1");

        // -- Steering Transform 
        switch (steeringTransformInputSource) {
            case SteamVR_Input_Sources.LeftHand:
                steeringTransformImage.sprite = spriteLeftHand;
                break;
            case SteamVR_Input_Sources.RightHand:
                steeringTransformImage.sprite = spriteRightHand;
                break;
            default:
                steeringTransformImage.sprite = spriteHMD;
                break;
        }

        // -- Game State
        switch (gameState) {
            case GameStateEventChannel.GameState.STARTED:
                gameStateText.text = "Game in progress";
                gameStartStopButtonText.text = "Stop Game";
                gameStartStopButtonBG.color = gameStartedColor;
                gameStartStopButton.sprite = spriteStop;
                break;
            case GameStateEventChannel.GameState.STOPPED:
                gameStateText.text = "Game not in progress";
                gameStartStopButtonText.text = "Start Game";
                gameStartStopButtonBG.color = gameStoppedColor;
                gameStartStopButton.sprite = spritePlay;
                break;
            case GameStateEventChannel.GameState.GAME_OVER:
                gameStateText.text = "Game over";
                gameStartStopButtonText.text = "Restart Game";
                gameStartStopButtonBG.color = gameStoppedColor;
                gameStartStopButton.sprite = spritePlay;
                break;
            default:
                Debug.LogWarning("Unexpected game state received: " + gameState);
                break;
        }

        numKillsText.text = "Kills - " + numEnemiesKilled;

        // TODO: Gate health
    }

    private void UpdateTimeAliveText() {
        // https://forum.unity.com/threads/convert-float-to-time-minutes-and-seconds.676414/
        System.TimeSpan ts = System.TimeSpan.FromSeconds(timeAlive);
        timeAliveText.text = "Time Alive - " + string.Format("{0:00}:{1:00}", ts.TotalMinutes, ts.Seconds);
    }


    public void ToggleVignette() {
        vignetteEnabled = !vignetteEnabled;
        settingsUpdatedChannel.RaiseEvent();
    }
    public void ToggleSpeedLines() {
        speedLinesEnabled = !speedLinesEnabled;
        settingsUpdatedChannel.RaiseEvent();
    }
    public void GetSwordAngle(float t, float scaledAngle) {
        swordAngleScaled = scaledAngle;
    }
    public void GetGrappleAngle(float t, float scaledAngle) {
        grappleAngleScaled = scaledAngle;
    }
    public void GetVignetteStrength(float t, float vignetteStrength) {
        vignetteStrengthScaled = vignetteStrength;
    }
    public void GetRopePullSpeed(float t, float pullSpeed) {
        ropePullSpeedScaled = pullSpeed;
    }
    public void GetSteeringTransform(SteamVR_Input_Sources inputSource) {
        steeringTransformInputSource = inputSource;
    }

    public void UpdateGameState(GameStateEventChannel.GameState gameState) {
        this.gameState = gameState;

        if (gameState == GameStateEventChannel.GameState.STARTED) {
            numEnemiesKilled = 0;
            timeAlive = 0f;
        }
    }
    public void ToggleGameState() {
        if (gameState == GameStateEventChannel.GameState.STARTED) {
            this.gameState = GameStateEventChannel.GameState.STOPPED;
        } else {
            this.gameState = GameStateEventChannel.GameState.STARTED;
        }

        this.gameStateChangedChannel.RaiseEvent(this.gameState);
    }

    public void UpdateEnemiesKilled() {
        numEnemiesKilled++;
    }
}
