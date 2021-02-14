using System;
using UnityEngine;
using UnityEditor;

// TODO: Clean up bezier spline addCurve function (when length == 0)
// TODO: Add removecurve function
[CustomEditor(typeof(BezierSpline))]
public class BezierSplineInspector : Editor {
    private BezierSpline spline;
    private Transform handleTransform;
    private Quaternion handleRotation;

    private const int lineSteps = 10;
    private const float directionScale = 0.5f;

    private const float handleSize = 0.04f;
    private const float pickSize = 0.06f;
    private int selectedIndex = -1;

    private static Color[] modeColors = {
        Color.white,
        Color.yellow,
        Color.cyan
    };

    private void OnSceneGUI() {
        spline = target as BezierSpline;

        // Validate that the user didn't mangle the spline's points array
        if (spline.numControlPoints == 0) return;
        if (spline.numControlPoints < 4
            || (spline.numControlPoints - 4) % 3 != 0) {
            Debug.LogError("The number of points in this spline is invalid!");
            return;
        }

        // -- The tools used to draw in the scene view (Handles) operates in world space.
        // Therefore, we have to convert our object's local space to world space
        handleTransform = spline.transform;

        // By default, it also doesn't take into account the unity's pivot rotation setting (global vs local)
        handleRotation = Tools.pivotRotation == PivotRotation.Local ?  handleTransform.rotation : Quaternion.identity;

        Vector3 p0 = ShowPoint(0);
        for (int i = 1; i < spline.numControlPoints; i += 3) {
            Vector3 p1 = ShowPoint(i);
            Vector3 p2 = ShowPoint(i + 1);
            Vector3 p3 = ShowPoint(i + 2);

            Handles.color = Color.grey;
            Handles.DrawLine(p0, p1);
            Handles.DrawLine(p2, p3);
        
            Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);

            p0 = p3;
        }

        ShowDirections();
    }

    // Draw the line, and record any changes performed via the GUI
    private Vector3 ShowPoint(int i) {
        Vector3 point = handleTransform.TransformPoint(spline.GetControlPoint(i));

        // Scale the points' cube size with screen size
        float screenSize = HandleUtility.GetHandleSize(point);

        Handles.color = modeColors[(int) spline.GetControlPointMode(i)];

        // Draw cube buttons on each point
        if (Handles.Button(point, handleRotation, screenSize * handleSize, screenSize * pickSize, Handles.DotHandleCap)) {
            selectedIndex = i;
        }

        // If we've selected the current point, then draw a Position Handle
        if (selectedIndex == i) {
            EditorGUI.BeginChangeCheck();
            point = Handles.DoPositionHandle(point, handleRotation);

            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(spline, "Move Point");
                EditorUtility.SetDirty(spline);

                spline.SetControlPoint(i, handleTransform.InverseTransformPoint(point));
            }
        }


        return point;
    }

    private void ShowDirections() {
        Handles.color = Color.green;
        Vector3 point = spline.GetPoint(0f);
        Handles.DrawLine(point, point + spline.GetDirection(0f) * directionScale);

        for (int i = 1; i <= lineSteps; i++) {
            point = spline.GetPoint(i / lineSteps);
            Handles.DrawLine(point, point + spline.GetDirection(i / lineSteps) * directionScale);
        }
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        spline = target as BezierSpline;
        if (GUILayout.Button("Add Curve")) {
            Undo.RecordObject(spline, "Add Curve");
            spline.AddCurve();
            EditorUtility.SetDirty(spline);
        }

        if (GUILayout.Button("Remove Curve")) {
            Undo.RecordObject(spline, "Remove Curve");
            spline.RemoveCurve();
            EditorUtility.SetDirty(spline);
        }
    }
}
