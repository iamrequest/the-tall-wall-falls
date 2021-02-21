using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RopeRenderer))]
public class RopeRendererInspector : Editor {
    private RopeRenderer ropeRenderer;

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        ropeRenderer = target as RopeRenderer;

        // Used for taking screenshots
        if (!Application.isPlaying) {
            if (GUILayout.Button("Update Spline")) {
                ropeRenderer.UpdateControlPoints();
            }

            if (GUILayout.Button("Update Render")) {
                ropeRenderer.UpdateRopeRender();
            }
        }
    }
}
