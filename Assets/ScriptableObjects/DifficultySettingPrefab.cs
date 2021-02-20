using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Difficulty Settings")]
public class DifficultySettingPrefab : ScriptableObject {
    public float spawnRate;
    public float maxNumEnemies;

    [Tooltip("Measured in mean full traversal duration per minute.")]
    public float enemySpeed;
}
