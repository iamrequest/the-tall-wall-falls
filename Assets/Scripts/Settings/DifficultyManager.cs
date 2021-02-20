using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Similar functionality to SettingsManager, separating difficulty since it's a bit more complicated
/// </summary>
public class DifficultyManager : MonoBehaviour {
    public GameStateEventChannel.GameState gameState { get; private set; } = GameStateEventChannel.GameState.STOPPED;

    public GameStateEventChannel gameStateEventChannel;
    public VoidEventChannel settingsUpdatedEventChannel;
    public SliderEventChannel spawnRateEventChannel, numEnemiesEventChannel, enemySpeedEventChannel;

    public SettingsMenuSlider spawnRateSlider, numEnemiesSlider, enemySpeedSlider;
    public TextMeshProUGUI spawnRateText, maxNumEnemiesText, enemySpeedText;
    public Image customDifficultyButtonImage;
    public List<Image> difficultyButtonsImages;
    public int selectedDifficultyIndex;
    public Color selectedColor, deselectedColor;

    public float spawnRateScaled { get; private set; }
    public float numEnemiesScaled { get; private set; }
    public float enemySpeedScaled { get; private set; }

    [SerializeField]
    public List<DifficultySettingPrefab> difficultyPrefabs;


    // Start is called before the first frame update
    void Start() {
        SetDifficulty(0);
    }
    private void OnEnable() {
        settingsUpdatedEventChannel.onEventRaised += UpdateGUI;
        gameStateEventChannel.onEventRaised += SetGameState;
        spawnRateEventChannel.onEventRaised += SetSpawnRate;
        numEnemiesEventChannel.onEventRaised += SetNumEnemies;
        enemySpeedEventChannel.onEventRaised += SetEnemySpeed;
    }

    private void OnDisable() {
        settingsUpdatedEventChannel.onEventRaised -= UpdateGUI;
        gameStateEventChannel.onEventRaised -= SetGameState;
        spawnRateEventChannel.onEventRaised -= SetSpawnRate;
        numEnemiesEventChannel.onEventRaised -= SetNumEnemies;
        enemySpeedEventChannel.onEventRaised -= SetEnemySpeed;
    }

    public void UpdateGUI() {
        if (selectedDifficultyIndex > difficultyButtonsImages.Count) {
            Debug.LogError("Invalid difficulty selected: " + selectedDifficultyIndex);
        }

        foreach (Image difficultyButtonImage in difficultyButtonsImages) {
            difficultyButtonImage.color = deselectedColor;
        }

        // Custom difficulty
        if (selectedDifficultyIndex != -1) {
            difficultyButtonsImages[selectedDifficultyIndex].color = selectedColor;
            customDifficultyButtonImage.color = deselectedColor;
        } else {
            customDifficultyButtonImage.color = selectedColor;
        }

        // -- Disable difficulty colliders & sliders while game is active
        foreach (Image difficultyButtonImage in difficultyButtonsImages) {
            difficultyButtonImage.GetComponent<BoxCollider>().enabled = (gameState != GameStateEventChannel.GameState.STARTED);
        }
        customDifficultyButtonImage.GetComponent<BoxCollider>().enabled = (gameState != GameStateEventChannel.GameState.STARTED);

        spawnRateSlider.enabled = (gameState != GameStateEventChannel.GameState.STARTED);
        numEnemiesSlider.enabled = (gameState != GameStateEventChannel.GameState.STARTED);
        enemySpeedSlider.enabled = (gameState != GameStateEventChannel.GameState.STARTED);


        // TODO: Update lerp values
        spawnRateText.text = "Spawn Rate: " + (spawnRateScaled * 100).ToString("F0") + "%";
        maxNumEnemiesText.text = "Max Enemies: " + Mathf.RoundToInt(numEnemiesScaled);
        enemySpeedText.text = "Enemy Speed: " + (enemySpeedScaled * 100).ToString("F0") + "%";
    }

    public void SetDifficultyCustom() {
        selectedDifficultyIndex = -1;
        UpdateGUI();
    }

    public void SetDifficulty(int difficultyIndex) {
        if (selectedDifficultyIndex > difficultyPrefabs.Count) {
            Debug.LogError("Invalid difficulty selected: " + selectedDifficultyIndex);
        }

        spawnRateSlider.SetValue(difficultyPrefabs[difficultyIndex].spawnRate);
        numEnemiesSlider.SetValue(difficultyPrefabs[difficultyIndex].maxNumEnemies);
        enemySpeedSlider.SetValue(difficultyPrefabs[difficultyIndex].enemySpeed);

        selectedDifficultyIndex = difficultyIndex;

        UpdateGUI();
    }


    // -- Manually updated difficulty settings
    public void SetSpawnRate(float t, float value) {
        spawnRateScaled = value;
        SetDifficultyCustom();
    }
    public void SetNumEnemies(float t, float value) {
        numEnemiesScaled = value;
        SetDifficultyCustom();
    }
    public void SetEnemySpeed(float t, float value) {
        enemySpeedScaled = value;
        SetDifficultyCustom();
    }
    public void SetGameState(GameStateEventChannel.GameState gameState) {
        this.gameState = gameState;
    }
}
