using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClippingPlane : MonoBehaviour
{

	static public ClippingPlane instance;
	//material we pass the values to
	public List<Material> matList;

	private void Awake()
	{
		instance = this;
		matList = new List<Material>();
	}

	//execute every frame
	void Update () {
		//create plane
		Plane plane = new Plane(transform.up, transform.position);
		//transfer values from plane to vector4
		Vector4 planeRepresentation = new Vector4(plane.normal.x, plane.normal.y, plane.normal.z, plane.distance);
		//pass vector to shader
		foreach (var mat in matList)
		{
			mat.SetVector("_Plane", planeRepresentation);
		}
	}
}

