using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple class to find a random path through a bi-directional graph. Path runs until we reach the end of connected nodes (bad), or we reach the end path node.
/// TODO: It's very easy to create a circular graph with this approach. Should refactor.
///
/// A side effect of creating a path this way (while using pre-defined splines in PathNode), is that the total spline path from start to end is not continuous (in direction, and probably in velocity too).
/// </summary>
[RequireComponent(typeof(BezierSpline))]
public class PathManager : MonoBehaviour {
    [HideInInspector]
    public BezierSpline selectedPathSpline;

    public PathNode startNode;
    public List<PathNode> selectedPath = new List<PathNode>();
    public BezierControlPointMode controlPointMode;
    public Transform tempAppendedTransform;
    public GameObject finalNodePathGameObject;

    // -- Debug
    private Gradient pathGradient = new Gradient();

    // Necessary for custom inspector
    private void OnValidate() {
        if (selectedPathSpline == null) {
            selectedPathSpline = GetComponent<BezierSpline>();
        }
    }

    private void Awake() {
        selectedPathSpline = GetComponent<BezierSpline>();

        // Green to red gradient, no alpha
        pathGradient.SetKeys(new GradientColorKey[] { new GradientColorKey(Color.green, 0f), new GradientColorKey(Color.red, 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 0f) });
    }

    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
    }

    public void GetRandomPath() {
        selectedPath.Clear();

        PathNode newNode = startNode;
        int maxPathLength = 10;

        while (selectedPath.Count < maxPathLength && newNode != null) {
            selectedPath.Add(newNode);
            newNode = newNode.GetRandomConnectedNode();
        }

        if (selectedPath.Count >= maxPathLength) {
            Debug.LogWarning("Max path length reached, there may be a circular sub graph somewhere");
        }
    }

    public void AppendNode(Vector3 position) {
        // Create the new PathNode on a child gameobject to avoid clutter
        DestroyImmediate(finalNodePathGameObject);
        finalNodePathGameObject = new GameObject();
        finalNodePathGameObject.transform.parent = transform;

        PathNode appendedNode = finalNodePathGameObject.AddComponent<PathNode>();
        BezierSpline appendedSpline = finalNodePathGameObject.AddComponent<BezierSpline>();
        Vector3 previousNodeToPosition = position - selectedPath[selectedPath.Count - 1].transform.position;
        Vector3 thisToPosition = position - transform.position;

        // Update end node markers
        selectedPath[selectedPath.Count - 1].isEndNode = false;
        appendedNode.isEndNode = true;

        // Add the new node/spline
        selectedPath[selectedPath.Count - 1].connectedNodes.Add(appendedNode);
        selectedPath[selectedPath.Count - 1].connectedNodePaths.Add(appendedSpline);
        selectedPath.Add(appendedNode);

        // -- TODO: This is broken
        appendedSpline.SetControlPoint(appendedSpline.numControlPoints - 4, selectedPath[selectedPath.Count - 2].transform.position);
        appendedSpline.SetControlPoint(appendedSpline.numControlPoints - 1, position);

        //appendedSpline.SetControlPoint(appendedSpline.numControlPoints - 3, position - transform.position);
        //appendedSpline.SetControlPoint(appendedSpline.numControlPoints - 2, position - transform.position);
    }

    /// <summary>
    /// Given each sub-spline from one node to the next in our path, stitch them all together to form one large bezier spline.
    /// </summary>
    public void CalculatePathSpline() {
        // Quick way to remove all previous curves and points.
        // One curve is created during Reset(), so we have to clear that out too
        selectedPathSpline.Reset();
        selectedPathSpline.RemoveCurve();

        for(int currentNodeIndex = 0; currentNodeIndex < selectedPath.Count - 1; currentNodeIndex++) {
            selectedPathSpline.AddCurve();

            // -- Set the control point mode.
            // Due to the way that I set up the node positions, free mode is the only one that looks good. I could use Aligned or Mirrored if I added more inbetween nodes, but my setup for creating nodes was kinda garbage
            //  Free mode only is fine for now
            selectedPathSpline.SetControlPointMode(3 * currentNodeIndex, controlPointMode);
            selectedPathSpline.SetControlPointMode(3 * currentNodeIndex + 3, controlPointMode);

            // Find the index of the sub-spline we want, in the node.
            // We have to do this search, since we store multiple sub-splines in each node
            int index = selectedPath[currentNodeIndex].connectedNodes.IndexOf(selectedPath[currentNodeIndex + 1]);
            BezierSpline subSpline = selectedPath[currentNodeIndex].connectedNodePaths[index];

            Vector3 toNode = selectedPath[currentNodeIndex].transform.position - transform.position;

            // -- Copy over each control point of the spline
            // Need to do this in this weird order, because we have to set the control points after the pivot points.
            // BezierSpline.SetControlPoints should probably get refactored to include an overload method to avoid this, but oh well
            selectedPathSpline.SetControlPoint(3*currentNodeIndex + 0, toNode + subSpline.GetControlPoint(0));
            selectedPathSpline.SetControlPoint(3*currentNodeIndex + 1, toNode + subSpline.GetControlPoint(1));
            selectedPathSpline.SetControlPoint(3*currentNodeIndex + 3, toNode + subSpline.GetControlPoint(3));
            selectedPathSpline.SetControlPoint(3*currentNodeIndex + 2, toNode + subSpline.GetControlPoint(2));
        }
    }




    // -- Debug --
    private void ValidateGraph() {
        // If we get no warning in the inspector, the bigraph is probably valid
        // This is a greasy validation check, liable to fail, but oh well
        for (int i = 0; i < 500; i++) {
            GetRandomPath();
        }
    }

    private void OnDrawGizmosSelected() {
        // -- Simple visualization of the selected path, using wireframe spheres, over a gradient
        int currentNodeIndex = 0;

        foreach (PathNode currentNode in selectedPath) {
            currentNodeIndex++;
            Gizmos.color = pathGradient.Evaluate((float)currentNodeIndex / (float)selectedPath.Count);
            Gizmos.DrawWireSphere(currentNode.transform.position, 3f);
        }
    }
}
