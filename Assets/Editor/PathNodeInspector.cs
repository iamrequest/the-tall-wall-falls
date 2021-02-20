using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathNode))]
public class PathNodeInspector : Editor {
    private PathNode pathNode;

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        pathNode = target as PathNode;

        if (GUILayout.Button("Instantiate Curves")) {
            Undo.RecordObject(pathNode, "Instantiate Curves");

            // -- Clear the old paths
            foreach (BezierSpline path in pathNode.connectedNodePaths) {
                DestroyImmediate(path);
            }
            pathNode.connectedNodePaths.Clear();

            // -- Create new ones
            foreach (PathNode connectedNode in pathNode.connectedNodes) {
                InstantiatePathSpline(connectedNode.transform);
            }

            EditorUtility.SetDirty(pathNode);
        }
    }

    private void OnSceneGUI() {
        // TODO: This isn't rendering properly, I don't need it though, so I won't be fixing it.
        /*
        pathNode = target as PathNode;

        // -- Draw the splines to each connected node
        foreach (BezierSpline path in pathNode.connectedNodePaths) {
            Handles.color = Color.cyan;
            //Gizmos.DrawLine(transform.position, connectedNode.transform.position);
            Handles.DrawLine(path.GetControlPoint(0), path.GetControlPoint(1));
            Handles.DrawLine(path.GetControlPoint(2), path.GetControlPoint(3));

            Handles.DrawBezier(path.GetControlPoint(0), path.GetControlPoint(3), path.GetControlPoint(1), path.GetControlPoint(2),
                Color.red, null, 5f);
        }
        */
    }



    private void InstantiatePathSpline(Transform target) {
        BezierSpline bezierSpline = pathNode.gameObject.AddComponent<BezierSpline>();
        Vector3 toTarget = target.position - pathNode.transform.position;

        // Need to do this in this weird order, because we have to set the control points after the pivot points.
        // BezierSpline.SetControlPoints should probably get refactored to include an overload method to avoid this, but oh well
        bezierSpline.SetControlPoint(0, Vector3.zero);
        bezierSpline.SetControlPoint(1, toTarget * 0.25f);
        bezierSpline.SetControlPoint(3, toTarget);
        bezierSpline.SetControlPoint(2, toTarget * 0.75f);

        pathNode.connectedNodePaths.Add(bezierSpline);
    }
}
