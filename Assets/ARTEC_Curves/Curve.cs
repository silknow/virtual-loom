using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace ARTEC.Curves {

/* Class to create a curve from a set of keypoints */
public class Curve : MonoBehaviour {

	public enum CurveInterpolation {
		Lineal,
		Hermite,
		CatmullRom
	}

	[Serializable]
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
	private  List<CurveControlPoint> _controlPoints = null;
	
	public List<CurveSample> _points;
	private CurveSample _prevPoint;

	public List<float> _knotsDistance;
	//public List<int> _knotsIndexes;

	private float d = 0;


	public List<CurveSample> GetPoints()
	{
		return _points;
	}

	public void AddControlPoint(Vector3 pos, Quaternion rot, bool changeUpDown,string name=null)
	{
		CurveControlPoint ccp=new CurveControlPoint();
		ccp.changeUpDown = changeUpDown;
		ccp.pos = pos;
		ccp.rot = rot;
		if (_controlPoints ==null) 
			_controlPoints = new List<CurveControlPoint>();
		_controlPoints.Add(ccp);
		
	}
	
	public void UpdateCurve()
	{
		if (_points == null)
			_points = new List<CurveSample>();

		if (_knotsDistance == null)
		{
			_knotsDistance = new List<float>();
			//_knotsIndexes = new List<int>();
		}
		if (_controlPoints ==null) 
			_controlPoints = new List<CurveControlPoint>();
		if (GetComponentInParent<Patch>() == null)
		{
			_controlPoints.Clear();
			CurveControlPointMonoBehaviour[] ccpmb = GetComponentsInChildren<CurveControlPointMonoBehaviour>();
			for (int i = 0; i < ccpmb.Length; i++)
				_controlPoints.Add(ccpmb[i].ccp);
		}

		// Clear points list
		_points.Clear();
		_knotsDistance.Clear();
		//_knotsIndexes.Clear();

		// For each interval
		for (var i = 0; i < _controlPoints.Count - 1; i++)
		{
			d = 0;

			for (var u = 0; u < samples; u++)
			{
				_prevPoint = GetSample(i, (float) u / (float) samples);
				_prevPoint.Distance = 0;
				//d = _prevPoint.D = Mathf.Min(Vector3.Distance(_prevPoint.Pos, controlPoints[i].transform.position),Vector3.Distance(_prevPoint.Pos, controlPoints[i+1].transform.position));
				_points.Add(_prevPoint);
			}
		}

		// Last point
		if (_controlPoints.Count > 1)
		{
			var last = new CurveSample();
			last.Pos = _controlPoints[_controlPoints.Count - 1].pos;//transform.position;
			last.Rot = _controlPoints[_controlPoints.Count - 1].rot;//transform.rotation;
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
			if (i%samples==0)
				_controlPoints[i/samples].distance = _points[i].Distance;
		}

		if (_controlPoints.Count>0 && _points.Count>0)
			_controlPoints[_controlPoints.Count - 1].distance = _points[_points.Count - 1].Distance;
		else
		{
			//Debug.LogError("_controlPoints:"+_controlPoints.Count+" and _points:"+_points.Count);
			return;
		}
		// Compute knots, create a knot before each up/down change
		_knotsDistance.Add(0.0f);
		//_knotsIndexes.Add(0);
		for (var i = 1; i < _controlPoints.Count; i++)
		{
			if (_controlPoints[i].changeUpDown)
			{
				float kDistance = _controlPoints[i].distance * 0.5f + _controlPoints[i - 1].distance * 0.5f;
				_knotsDistance.Add(kDistance);
				//_knotsIndexes.Add(i);
				
			}
		}

		_knotsDistance.Add(_controlPoints[_controlPoints.Count - 1].distance);
		//_knotsIndexes.Add(controlPoints.Length - 1);

		
		 // Update the distance to the closest knot
		int j = 1;
		for (int i=1;i<_points.Count;i++)
		{
			_points[i].DistanceToKnot = Mathf.Min(Mathf.Abs(_knotsDistance[j] - _points[i].Distance),
				Mathf.Abs(_knotsDistance[j - 1] - _points[i].Distance));
			if (_points[i].Distance > _knotsDistance[j])
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

		var t1 = _controlPoints[i];
		var t2 = _controlPoints[i+1];

		sample.Pos = Vector3.Lerp(t1.pos, t2.pos, u);
		sample.Rot = Quaternion.Slerp(t1.rot, t2.rot, u);
		sample.U = u;

		return sample;
	}

	private CurveSample CatmullRom(int i, float u) {
		var sample = new CurveSample();

		//-----------
		// When u==0, just return the first point of the interval
		if (u <= 0) {
			sample.Pos = _controlPoints[i].pos;
			sample.Rot = _controlPoints[i].rot;
			sample.U = 0;
			return sample;
		}

		//----------
		// Position
		var p1 = _controlPoints[i].pos;
		var p0 = p1;
		if (i > 0)
			p0 = _controlPoints[i].pos;
		var p2 = _controlPoints[i+1].pos;
		var p3 = p2;
		if (i+2 < _controlPoints.Count)
			p3 = _controlPoints[i+2].pos;

		var tens = speedMultiplier;
		var u2 = u*u;
		var u3 = u2 * u;
		sample.Pos = (-tens*u + 2*tens*u2 - tens*u3) * p0 + 
					 (1+(tens-3)*u2 + (2-tens)*u3) *p1 + 
					 (tens*u + (3-2*tens)*u2 + (tens-2)*u3) * p2 + 
					 (tens*u3-tens*u2) * p3;
		
		//----------
		// Rotation
		var t1 = _controlPoints[i];
		var t2 = _controlPoints[i+1];
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
			sample.Pos = _controlPoints[i].pos;
			sample.Rot = _controlPoints[i].rot;
			sample.U = 0;
			return sample;
		}

		//----------
		// Position
		var p0 = _controlPoints[i].pos;
		var p1 = _controlPoints[i+1].pos;
		var v0 = _controlPoints[i].forward * speedMultiplier;
		var v1 = _controlPoints[i+1].forward * speedMultiplier;

		var u2 = u*u;
		var u3 = u2 * u;
		sample.Pos = (1 - 3*u2 + 2*u3) * p0 + 
					 u2*(3-2*u)*p1 + 
					 u * (u-1)*(u-1) * v0 + 
					 u2 * (u-1) * v1;

		//----------
		// Rotation
		var t1 = _controlPoints[i];
		var t2 = _controlPoints[i+1];
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
