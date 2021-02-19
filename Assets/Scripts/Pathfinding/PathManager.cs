using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple class to find a random path through a bi-directional graph. Path runs until we reach the end of connected nodes (bad), or we reach the end path node.
/// TODO: It's very easy to create a circular graph with this approach. Should refactor.
///
/// A side effect of creating a path this way (while using pre-defined splines in PathNode), is that the total spline path from start to end is not continuous (in direction, and probably in velocity too).
/// </summary>
public class PathManager : MonoBehaviour {
    public PathNode startNode;
    public List<PathNode> selectedPath = new List<PathNode>();

    // -- Debug
    private Gradient pathGradient;

    private void Awake() {
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
