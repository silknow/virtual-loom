using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARTEC.Curves {

/* Class to create a curve from a set of keypoints */
public class Curve : MonoBehaviour {

	public enum CurveInterpolation {
		Lineal,
		Hermite,
		CatmullRom
	}

	public class CurveSample {
		public Vector3 Pos;
		public Quaternion Rot;
		public float U;
		//public float D;
		public float Distance;
		public int Interval;
		public float DistanceToKnot;
	}

	public float speedMultiplier = 0.0f;
	public CurveInterpolation interpolation = CurveInterpolation.CatmullRom;
	public int samples = 10;
	private CurveControlPoint[]controlPoints;
	private List<CurveSample> _points;
	private CurveSample _prevPoint;

	private List<float> _knotsDistance;
	private List<int> _knotsIndexes;

	private float d = 0;


	public List<CurveSample> GetPoints()
	{
		return _points;
	}

	public void AddControlPoint(Vector3 pos, Quaternion rot, bool changeUpDown)
	{
		var go= (GameObject)Resources.Load("CPGizmo");
		var cp = (GameObject)Instantiate(go,transform);	        
		cp.transform.localPosition = pos;
		cp.transform.localRotation = rot;

		CurveControlPoint ccp = cp.GetComponent<CurveControlPoint>();
		ccp.changeUpDown = changeUpDown;
	}

	public void UpdateCurve()
	{
		if (_points == null)
		{
			_points = new List<CurveSample>();
			
		}

		if (_points == null)
			_points = new List<CurveSample>();

		if (_knotsDistance == null)
		{
			_knotsDistance = new List<float>();
			_knotsIndexes = new List<int>();
		}

		controlPoints = GetComponentsInChildren<CurveControlPoint>();
		// Clear points list
		_points.Clear();
		_knotsDistance.Clear();
		_knotsIndexes.Clear();

		// For each interval
		for (var i = 0; i < controlPoints.Length - 1; i++)
		{
			d = 0;

			for (var u = 0; u < samples; u++)
			{
				_prevPoint = GetSample(i, (float) u / (float) samples);
				_prevPoint.Distance = 0;
				_prevPoint.Interval = i;
				//d = _prevPoint.D = Mathf.Min(Vector3.Distance(_prevPoint.Pos, controlPoints[i].transform.position),Vector3.Distance(_prevPoint.Pos, controlPoints[i+1].transform.position));
				_points.Add(_prevPoint);
			}
		}

		// Last point
		if (controlPoints.Length > 1)
		{
			var last = new CurveSample();
			last.Pos = controlPoints[controlPoints.Length - 1].transform.position;
			last.Rot = controlPoints[controlPoints.Length - 1].transform.rotation;
			last.U = 1.0f;
			//last.D = 0.0f;
			last.DistanceToKnot = 0.0f;
			_points.Add(last);
		}

		// Compute Distance between points
		for (int i = 1; i < _points.Count; i++)
		{
			_points[i].Distance = _points[i - 1].Distance + (_points[i].Pos - _points[i - 1].Pos).magnitude;
			// If it is the first point in the interval, update the distance of the control point
			if (_points[i].U < 0.000001f)
				controlPoints[_points[i].Interval].distance = _points[i].Distance;
		}

		controlPoints[controlPoints.Length - 1].distance = _points[_points.Count - 1].Distance;

		// Compute knots, create a knot before each up/down change
		_knotsDistance.Add(0.0f);
		_knotsIndexes.Add(0);
		for (var i = 1; i < controlPoints.Length; i++)
		{
			if (controlPoints[i].changeUpDown)
			{
				float kDistance = controlPoints[i].distance * 0.5f + controlPoints[i - 1].distance * 0.5f;
				_knotsDistance.Add(kDistance);
				_knotsIndexes.Add(i);
				
			}
		}

		_knotsDistance.Add(controlPoints[controlPoints.Length - 1].distance);
		_knotsIndexes.Add(controlPoints.Length - 1);

		
		 // Update the distance to the closest knot
		int j = 1;
		for (int i=1;i<_points.Count;i++)
		{
			_points[i].DistanceToKnot = Mathf.Min(_knotsDistance[j] - _points[i].Distance,
				_knotsDistance[j - 1] - _points[i].Distance);
			if (_points[i].Interval > _knotsIndexes[j])
				j++;
		}

	}

	#region INTERPOLATION FUNCTIONS

	private CurveSample GetSample(int i, float u) {
		switch (interpolation) {
			case CurveInterpolation.Lineal: return Lineal (i, u);
			case CurveInterpolation.CatmullRom: return CatmullRom(i,u);
			case CurveInterpolation.Hermite: return Hermite(i,u);
			default: return new CurveSample();
		}
	}

	private CurveSample Lineal(int i, float u) {
		var sample = new CurveSample();

		var t1 = controlPoints[i].transform;
		var t2 = controlPoints[i+1].transform;

		sample.Pos = Vector3.Lerp(t1.position, t2.position, u);
		sample.Rot = Quaternion.Slerp(t1.rotation, t2.rotation, u);
		sample.U = u;

		return sample;
	}

	private CurveSample CatmullRom(int i, float u) {
		var sample = new CurveSample();

		//-----------
		// When u==0, just return the first point of the interval
		if (u <= 0) {
			sample.Pos = controlPoints[i].transform.position;
			sample.Rot = controlPoints[i].transform.rotation;
			sample.U = 0;
			return sample;
		}

		//----------
		// Position
		var p1 = controlPoints[i].transform.position;
		var p0 = p1;
		if (i > 0)
			p0 = controlPoints[i].transform.position;
		var p2 = controlPoints[i+1].transform.position;
		var p3 = p2;
		if (i+2 < controlPoints.Length)
			p3 = controlPoints[i+2].transform.position;

		var tens = speedMultiplier;
		var u2 = u*u;
		var u3 = u2 * u;
		sample.Pos = (-tens*u + 2*tens*u2 - tens*u3) * p0 + 
					 (1+(tens-3)*u2 + (2-tens)*u3) *p1 + 
					 (tens*u + (3-2*tens)*u2 + (tens-2)*u3) * p2 + 
					 (tens*u3-tens*u2) * p3;
		
		//----------
		// Rotation
		var t1 = controlPoints[i].transform;
		var t2 = controlPoints[i+1].transform;
		sample.Rot = _prevPoint.Rot;

		// Keep the forward tangent to the curve
		var tg = (sample.Pos - _prevPoint.Pos);
		var up = Vector3.Cross(tg, Vector3.Lerp(t1.right, t2.right, u));

		if ((up.magnitude > 0) && (tg.magnitude > 0))
			sample.Rot = Quaternion.LookRotation(tg, up);
		
		sample.U = u;

		return sample;
	}

	private CurveSample Hermite(int i, float u) {
		var sample = new CurveSample();

		//-----------
		// When u==0, just return the first point of the interval
		if (u <= 0) {
			sample.Pos = controlPoints[i].transform.position;
			sample.Rot = controlPoints[i].transform.rotation;
			sample.U = 0;
			return sample;
		}

		//----------
		// Position
		var p0 = controlPoints[i].transform.position;
		var p1 = controlPoints[i+1].transform.position;
		var v0 = controlPoints[i].transform.forward * speedMultiplier;
		var v1 = controlPoints[i+1].transform.forward * speedMultiplier;

		var u2 = u*u;
		var u3 = u2 * u;
		sample.Pos = (1 - 3*u2 + 2*u3) * p0 + 
					 u2*(3-2*u)*p1 + 
					 u * (u-1)*(u-1) * v0 + 
					 u2 * (u-1) * v1;

		//----------
		// Rotation
		var t1 = controlPoints[i].transform;
		var t2 = controlPoints[i+1].transform;
		sample.Rot = _prevPoint.Rot;

		// Keep the forward tangent to the curve
		var tg = (sample.Pos - _prevPoint.Pos);
		tg.Normalize();
		var up = Vector3.Cross(tg, Vector3.Lerp(t1.right, t2.right, u));
		//var up = Vector3.Cross(tg, t1.right);

		//Debug.Log(sample.Pos.x);
		
		if ((up.magnitude > 0) && (tg.magnitude > 0))
			sample.Rot = Quaternion.LookRotation(tg, up);
		else 
			Debug.Log("error magnitude tg up");

		sample.U = u;
		return sample;
	}

	#endregion
}

}
