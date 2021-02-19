﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathManager))]
public class PathManagerInspector : Editor {
    private PathManager pathManager;

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        pathManager = target as PathManager;

        if (GUILayout.Button("Refresh Path")) {
            Undo.RecordObject(pathManager, "Refresh Path");
            pathManager.GetRandomPath();
            EditorUtility.SetDirty(pathManager);
        }

        if (GUILayout.Button("Re-calculate Spline")) {
            Undo.RecordObject(pathManager, "Re-calculate Spline");
            pathManager.CalculatePathSpline();
            EditorUtility.SetDirty(pathManager);
        }
    }
}
