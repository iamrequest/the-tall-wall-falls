using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurve : MonoBehaviour {
    public Vector3[] points;

    public void Reset() {
        points = new Vector3[] {
            new Vector3(1f, 0, 0),
            new Vector3(2f, 0, 0),
            new Vector3(3f, 0, 0),
        };
    }

    public Vector3 GetPoint (float t) {
		return transform.TransformPoint(GetPoint(points[0], points[1], points[2], t));
	}

    public Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t) {
        // Inefficient - We can do better!
        //return Vector3.Lerp(
        //    Vector3.Lerp(p0, p1, t),
        //    Vector3.Lerp(p1, p2, t),
        //    t);

        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;

        // B(t) = (1 - t) ((1 - t) P0 + t P1) + 
        //        t ((1 - t) P1 + t P2)
        //
        // This can be rewritten as:
        // B(t) = (1 - t)^2 * P0 + 
        //        2 (1 - t) t * P1 +
        //        t^2 * P2
        return oneMinusT * oneMinusT * p0 +
            2f * oneMinusT * t * p1 +
            t * t * p2;
    }

    public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, float t) {
        // B'(t) = 2 (1 - t) (P1 - P0) + 2 t (P2 - P1)
        return 
            2f * (1f - t) * (p1 - p0) +
            2f * t * (p2 - p1);
    }

    public Vector3 GetVelocity(float t) {
        return transform.TransformPoint(BezierCurve.GetFirstDerivative(points[0], points[1], points[2], t)) -
            transform.position;
    }
}
