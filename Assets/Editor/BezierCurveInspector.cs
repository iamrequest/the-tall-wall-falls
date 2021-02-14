using UnityEngine;
using UnityEditor;

// Source: https://catlikecoding.com/unity/tutorials/curves-and-splines/
[CustomEditor(typeof(BezierCurve))]
public class BezierCurveInspector : Editor {
    private BezierCurve curve;
    private Transform handleTransform;
    private Quaternion handleRotation;

    private const int lineSteps = 10;

    private void OnSceneGUI() {
        curve = target as BezierCurve;

        // -- The tools used to draw in the scene view (Handles) operates in world space.
        // Therefore, we have to convert our object's local space to world space
        handleTransform = curve.transform;

        // By default, it also doesn't take into account the unity's pivot rotation setting (global vs local)
        // handleRotation = Tools.pivotRotation == PivotRotation.Local ?  handleTransform.rotation : Quaternion.identity;
        if (Tools.pivotRotation == PivotRotation.Local) {
            handleRotation = handleTransform.rotation;
        } else {
            handleRotation = Quaternion.identity;
        }

        Vector3 p0 = ShowPoint(0);
        Vector3 p1 = ShowPoint(1);
        Vector3 p2 = ShowPoint(2);
        Vector3 p3 = ShowPoint(3);

        Handles.color = Color.grey;
        Handles.DrawLine(p0, p1);
        Handles.DrawLine(p2, p3);

        Handles.color = Color.white;
        Vector3 lineStart = curve.GetPoint(0f);
        //for (int i = 1; i <= lineSteps; i++) {
        //    Vector3 lineEnd = curve.GetPoint(i / (float)lineSteps);

        //    Handles.DrawLine(lineStart, lineEnd);

        //    lineStart = lineEnd;
        //}

        Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);
    }

    // Draw the line, and record any changes performed via the GUI
    private Vector3 ShowPoint(int i) {
        Vector3 point = handleTransform.TransformPoint(curve.points[i]);

        EditorGUI.BeginChangeCheck();
        point = Handles.DoPositionHandle(point, handleRotation);

        if (EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(curve, "Move Point");
            EditorUtility.SetDirty(curve);

            curve.points[i] = handleTransform.InverseTransformPoint(point);
        }

        return point;
    }
}
