using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;
using Freya;

[RequireComponent(typeof(Polyline))]
[RequireComponent(typeof(BezierSpline))]
public class RopeRenderer : MonoBehaviour {
    public Transform projectile;
    private Polyline polyline;
    private BezierSpline bezierSpline;

    [Header("Spline Rendering")]
    [Range(4, 64)]
    public int numDrawnPoints = 4;

    [Range(0f, 2f)]
    public float ropeThickness = .1f;
    public Gradient ropeColor;
    [Range(0f, 1f)]
    public float ropeColorT;


    [Header("Spline Offset")]
    [Range(0f, 1f)]
    public float offsetTime;
    public AnimationCurve offsetCurve;

    [Range(0f, 1f)]
    public float controlPointForwardOffset1, controlPointForwardOffset2;
    [Range(-2f, 2f)]
    public float controlPointRightOffset1, controlPointRightOffset2;

    private Vector3 controlPoint1, controlPoint2;


    private void OnValidate() {
        polyline = GetComponent<Polyline>();
        bezierSpline = GetComponent<BezierSpline>();
    }
    private void Awake() {
        polyline = GetComponent<Polyline>();
        bezierSpline = GetComponent<BezierSpline>();
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.rotation * (transform.position + controlPoint1), .05f);
        Gizmos.DrawWireSphere(transform.rotation * (transform.position + controlPoint2), .05f);

        if (polyline == null || bezierSpline == null) {
            Awake();
        }
    }

    private void Update() {
        UpdateControlPoints();
        UpdateRopeRender();
    }

    public void UpdateRopeRender() {
        UpdatePolyline();
        polyline.Color = ropeColor.Evaluate(ropeColorT);
    }

    // TODO: Is jitter related to offset time being a thing?
    public void UpdateControlPoints() {
        // Find our basis vector
        Vector3 forward = Quaternion.Inverse(transform.rotation) * (projectile.position - transform.position);
        float totalLength = forward.magnitude;

        forward.Normalize();
        Vector3 right = Vector3.Cross(transform.up, forward);

        // Calculate control point 1, plus the manually defined offset
        controlPoint1 = forward * totalLength * controlPointForwardOffset1;
        controlPoint1 += right * controlPointRightOffset1 * offsetCurve.Evaluate(offsetTime);

        // Calculate control point 2, plus the manually defined offset
        controlPoint2 = forward * totalLength * controlPointForwardOffset2;
        controlPoint2 += right * controlPointRightOffset2 * offsetCurve.Evaluate(offsetTime);

        // Add sway to each control point, relative to the player's hand motion since starting the pull
        // TODO: Replace localposition
        //controlPoint1 -= transform.localPosition * (1 - controlPointForwardOffset1) * offsetCurve.Evaluate(offsetTime);
        //controlPoint2 -= transform.localPosition * controlPointForwardOffset2 * offsetCurve.Evaluate(offsetTime);

        // -- Set the control points in the spline
        bezierSpline.SetControlPoint(0, Vector3.zero);
        bezierSpline.SetControlPoint(1, controlPoint1);
        bezierSpline.SetControlPoint(2, controlPoint2);
        bezierSpline.SetControlPoint(3, forward * totalLength);
    }

    private void UpdatePolyline() {
        // Update the list of points, if numDrawnPoints has been updated
        // This can likely be moved to Awake() once testing is done
        if (polyline.points.Count != numDrawnPoints) {
            polyline.points.Clear();
            for (int i = 0; i < numDrawnPoints; i++) {
                polyline.AddPoint(Vector3.zero);
            }
        }

        for (int i = 0; i < numDrawnPoints; i++) {
            polyline.SetPointPosition(i, bezierSpline.GetPointLocalSpace((float) i / (numDrawnPoints - 1) ));
            polyline.SetPointThickness(i, ropeThickness);
        }
    }
}
