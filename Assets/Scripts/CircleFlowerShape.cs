using System;
using System.Collections.Generic;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;

[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
public class CircleFlowerShape 
{
    public List<Vector2> shape;

    private int nCircles, samples;
    // Update is called once per frame
    public void CalculateShape(float R1,float R2,int nCircles,int samples)
    {
        this.nCircles = nCircles;
        this.samples = samples;
        shape = new List<Vector2>();
        nCircles = Math.Max(1, nCircles);
        float angleArc;
        var d = R1 * Mathf.Sin(Mathf.PI / nCircles);
        R2 = Mathf.Max(R2, d);
        {
            var h = Mathf.Sqrt( R2*R2-d*d);
            var beta = Mathf.Rad2Deg*Mathf.Atan2(h, d);
            float gamma = 360-(nCircles - 2) * 180 / nCircles;
            angleArc = gamma-2*beta;
        }

        shape = new List<Vector2>();
        
        for (int i=0;i<nCircles;i++)
            DrawArc(i,Quaternion.Euler(0,360.0f/nCircles*i,0)*Vector3.forward*R1,R2,angleArc);
        //Repeat first point
        shape.Add(shape[0]);
        shape.Reverse();
    }
    void DrawArc(int j,Vector3 center, float r, float angle)
    {
        for (int i = 0; i < samples; i++)
        {
            Vector3 p = center + Quaternion.Euler(0, (360.0f / nCircles) * j, 0) *
                        Quaternion.Euler(0, angle * (-0.5f), 0) *
                        Quaternion.Euler(0.0f, i * angle / (samples - 1), 0.0f) * Vector3.forward * r;
            shape.Add(new Vector2(p.x,p.z));
        }
    }
}
