using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple bigraph node.
/// Each spline that connects this node to the next should only have one curve. This is a soft limitation, imposed in PathManager. 
///     It's just easier to evaluate the total spline path this way.
/// </summary>
public class PathNode : MonoBehaviour {
    public List<PathNode> connectedNodes = new List<PathNode>();
    public List<BezierSpline> connectedNodePaths = new List<BezierSpline>();
    public bool isEndNode;

    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(transform.position,1f);
        foreach (PathNode connectedNode in connectedNodes) {
            // -- Draw straight line to each connected node
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, connectedNode.transform.position);
        }
    }

    public PathNode GetRandomConnectedNode() {
        if (isEndNode) {
            return null;
        } else if (connectedNodes.Count < 1) {
            Debug.LogWarning("Reached the end of this bigraph, but the last node (" + gameObject.name + ") isn't marked as an end node.");
            return null;
        }

        return connectedNodes[Random.Range(0, connectedNodes.Count)];
    }
}
