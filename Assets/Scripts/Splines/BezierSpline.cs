using System;
using UnityEngine;

// Aligned: Control points that border a shared point between 2 splines will maintain opposite directions
//  This is continuous
// Mirror: Control points that border a shared point between 2 splines will maintain opposite directions and magnitude
//  This is continuous, and the spline maintains a constant acceleration (all derivatives on the spline are the same)
public enum BezierControlPointMode {
    Free, Aligned, Mirror
}

public class BezierSpline : MonoBehaviour {
    [SerializeField]
    private Vector3[] points;

    [SerializeField]
    private BezierControlPointMode[] modes;

    public int numCurves {
        get {
            return (points.Length - 1) / 3;
        }
    }

    public int numControlPoints {
        get {
            return points.Length;
        }
    }
    public Vector3 GetControlPoint(int i) {
        return points[i];
    }
    public void SetControlPoint(int i, Vector3 point) {
        // If we're moving a middle point (ie: spline edge), then shift the bordering control points along with it
        if (i % 3 == 0) {
            Vector3 delta = point - points[i];

            if (i > 0) {
                points[i - 1] += delta;
            }

            if (i + 1 < points.Length) {
                points[i + 1] += delta;
            }
        }
        points[i] = point;
        EnforceMode(i);
    }

    public BezierControlPointMode GetControlPointMode(int i) {
        return modes[(i + 1) / 3];
    }
    public void SetControlPointMode(int i, BezierControlPointMode mode) {
        modes[(i + 1) / 3] = mode;
        EnforceMode(i);
    }

    private void EnforceMode(int i) {
        int modeIndex = (i + 1) / 3;
        BezierControlPointMode mode = modes[modeIndex];

        // Nothing to enforce for free mode, and for when we're at the end of the spline
        if (mode == BezierControlPointMode.Free ||
            modeIndex == 0 ||
            modeIndex == modes.Length - 1) {

            return;
        }

        // -- Figure out what points we need to enfoce
        int middleIndex = modeIndex * 3;
        int fixedIndex, enforcedIndex;
        if (i <= middleIndex) {
            fixedIndex = middleIndex - 1;
            enforcedIndex = middleIndex + 1;
        } else {
            fixedIndex = middleIndex + 1;
            enforcedIndex = middleIndex - 1;
        }

        Vector3 middle = points[middleIndex];
        Vector3 enforcedTangent = middle - points[fixedIndex];
        if (mode == BezierControlPointMode.Aligned) {
            enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, points[enforcedIndex]);
        }

        points[enforcedIndex] = middle + enforcedTangent;
    }



    public void Reset() {
        points = new Vector3[] {
            new Vector3(1f, 0, 0),
            new Vector3(2f, 0, 0),
            new Vector3(3f, 0, 0),
            new Vector3(4f, 0, 0),
        };

        modes = new BezierControlPointMode[] {
            BezierControlPointMode.Free,
            BezierControlPointMode.Free
        };
    }

    public Vector3 GetPointLocalSpace(float t) {
        int i;
        if (t >= 1f) {
            t = 1f;
            i = points.Length - 4;
        } else {
            t = Mathf.Clamp01(t) * numCurves;
            i = (int)t;
            t -= i;
            i *= 3;
        }

		return GetPointOnCurve(points[i], points[i + 1], points[i + 2], points[i + 3], t);
    }

    public Vector3 GetPoint (float t) {
        int i;
        if (t >= 1f) {
            t = 1f;
            i = points.Length - 4;
        } else {
            t = Mathf.Clamp01(t) * numCurves;
            i = (int)t;
            t -= i;
            i *= 3;
        }

		return transform.TransformPoint(GetPointOnCurve(points[i], points[i + 1], points[i + 2], points[i + 3], t));
	}

    private Vector3 GetPointOnCurve(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
        // Inefficient - We can do better!
        //return Vector3.Lerp(
        //    Vector3.Lerp(p0, p1, t),
        //    Vector3.Lerp(p1, p2, t),
        //    t);

        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;

        // -- Quadratic
        // B(t) = (1 - t) ((1 - t) P0 + t P1) + 
        //        t ((1 - t) P1 + t P2)
        //
        // This can be rewritten as:
        // B(t) = (1 - t)^2 * P0 + 
        //        2 (1 - t) t * P1 +
        //        t^2 * P2
        //return oneMinusT * oneMinusT * p0 +
        //    2f * oneMinusT * t * p1 +
        //    t * t * p2;

        // -- Cubic
        return
			oneMinusT * oneMinusT * oneMinusT * p0 +
			3f * oneMinusT * oneMinusT * t * p1 +
			3f * oneMinusT * t * t * p2 +
			t * t * t * p3;
    }

    public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
        // -- This is for quadratic curves
        // B'(t) = 2 (1 - t) (P1 - P0) + 2 t (P2 - P1)
        //return 
        //    2f * (1f - t) * (p1 - p0) +
        //    2f * t * (p2 - p1);

        // -- This is for cubic curves
		t = Mathf.Clamp01(t);
		float oneMinusT = 1f - t;
		return
			3f * oneMinusT * oneMinusT * (p1 - p0) +
			6f * oneMinusT * t * (p2 - p1) +
			3f * t * t * (p3 - p2);
    }

    public Vector3 GetVelocity(float t) {
        return transform.TransformPoint(BezierCurve.GetFirstDerivative(points[0], points[1], points[2], t)) -
            transform.position;
    }

    public Vector3 GetDirection(float t) {
        return GetVelocity(t).normalized;
    }

    // Append a new bezier curve - the first point in the new curve is the last point in the prev curve
    //  So, we only need to create 3 new points to complete a 4-point curve
    public void AddCurve() {
        Vector3 point;
        int numPointsToAdd;

        // Get the last point (or create a new one at [0,0,0] if none exist)
        if (points.Length == 0) {
            point = new Vector3(0, 0, 0);
            numPointsToAdd = 4;
            Array.Resize(ref modes, 1);
        } else {
            point = points[points.Length - 1];
            numPointsToAdd = 3;
        }

        // Append the new points
        Array.Resize(ref points, points.Length + numPointsToAdd);
        for (int i = 0; i < numPointsToAdd; i++) {
            point.x += 1f;
            points[points.Length + (i - numPointsToAdd)] = point;
        }

        // Append a new mode
        Array.Resize(ref modes, modes.Length + 1);
        modes[modes.Length - 1] = modes[modes.Length - 2];
        EnforceMode(points.Length - 4);
    }

    public void RemoveCurve() {
        if (points.Length <= 4) {
            points = new Vector3[] { };
            modes = new BezierControlPointMode[] { };
        } else {
            Array.Resize(ref points, points.Length - 3);
            Array.Resize(ref modes, modes.Length - 1);
        }
    }
}
