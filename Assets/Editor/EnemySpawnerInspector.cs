using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemySpawner))]
public class EnemySpawnerInspector : Editor {
    private EnemySpawner enemySpawner;

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        enemySpawner = target as EnemySpawner;

        if (Application.isPlaying) {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Start Spawning")) {
                enemySpawner.StartSpawning();
            }

            if (GUILayout.Button("End Spawning")) {
                enemySpawner.EndSpawning();
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Kill All")) {
                enemySpawner.KillAllEnemies();
            }

            if (GUILayout.Button("Despawn All")) {
                enemySpawner.DespawnAllEnemies();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
