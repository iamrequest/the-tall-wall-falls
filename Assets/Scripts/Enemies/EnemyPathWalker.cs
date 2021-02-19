using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Note: Due to how I'm calculating the spline, the current setup for moving an enemy along a path is to move a child transform along the path.
///     Next steps would be to include this transform's offset when calculating and moving along the spline, but I don't have time for that 
/// </summary>
[RequireComponent(typeof(PathManager))]
public class EnemyPathWalker : MonoBehaviour {
    [HideInInspector]
    public PathManager pathManager;
    public bool isWalkingPath;
    public UnityEvent onPathComplete;

    // Probably should make this transform move instead, but this is much easier
    [Tooltip("The target transform that will move along the path")]
    public Transform targetTransform;

    [Range(0.1f, 120f)]
    public float pathWalkDuration;
    private float elapsedPathWalkTime;
    private Vector3 previousPosition;

    [Range(0f, 1f)]
    public float turnSpeed;

    private void Awake() {
        pathManager = GetComponent<PathManager>();
    }

    /// <summary>
    /// Call after the start node has been assigned
    /// </summary>
    public void Setup() {
        pathManager.GetRandomPath();
        pathManager.CalculatePathSpline();
        targetTransform.position = pathManager.startNode.transform.position;

        // Look at the end node. Rough heuristic, will smooth out in a few frames
        targetTransform.rotation = Quaternion.LookRotation(pathManager.selectedPath[pathManager.selectedPath.Count - 1].transform.position - targetTransform.position, Vector3.up);

        elapsedPathWalkTime = 0f;
        isWalkingPath = true;
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.B)) {
            Setup();
        }
    }

    private void FixedUpdate() {
        if (isWalkingPath) {
            WalkPath();
        }
    }

    private void WalkPath() {
        elapsedPathWalkTime += Time.fixedDeltaTime;

        previousPosition = targetTransform.position;

        // TODO: Consider smoothing out the spline by moving between the average of predicted points
        targetTransform.position = pathManager.selectedPathSpline.GetPoint(elapsedPathWalkTime / pathWalkDuration);

        // -- Lerp to face the movement dir
        // Zero check to avoid LookRotation() with no foward direction
        if (previousPosition - targetTransform.position != Vector3.zero) {
            targetTransform.rotation = Quaternion.Slerp(targetTransform.rotation, Quaternion.LookRotation(targetTransform.position - previousPosition, Vector3.up), turnSpeed);
        }

        // -- Check if we've completed the path
        if (elapsedPathWalkTime >= pathWalkDuration) {
            isWalkingPath = false;
            onPathComplete.Invoke();
        }
    }
}
