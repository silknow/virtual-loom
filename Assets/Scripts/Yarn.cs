using System.Collections;
using System.Collections.Generic;
using ARTEC.Curves;

using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public class Yarn : MonoBehaviour
{
    public ScriptableYarn attributes;
    

    public Material m;

    private CurvePipeRenderer cpr;
    // Start is called before the first frame update
    void Start()
    {
        Curve curve = GetComponent<Curve>();
        if (!GetComponentInParent<Patch>())
            curve.UpdateCurve();
        {
            GameObject go = Instantiate(Resources.Load("Thread"),transform) as GameObject;
            cpr = go.GetComponent<CurvePipeRenderer>();
            cpr.curve = curve;
            go.GetComponent<Renderer>().material=attributes.material;
            m = go.GetComponent<Renderer>().materials[0];
            m.color = attributes.color;
            m.SetFloat("_Twist", attributes.twist);
            m.SetInt("_Threads",attributes.threadsNumber);
            cpr.r1 *= attributes.threadSize * attributes.threadsNumber;
            cpr.r2 *= attributes.threadSize * attributes.threadsNumber;
            //cpr.nCircles = attributes.threadsNumber;
            //if (cpr.desfase == 360.0f) cpr.desfase =0.0f;
            //go.GetComponent<Renderer>().sharedMaterial.color= new Color(attributes.color.ScR,attributes.color.ScG,attributes.color.ScB,attributes.color.ScA);
            cpr.UpdateMesh();
        }

    }

    public void Update()
    {
        if (m != null && cpr != null)
        {
            m.color = attributes.color;
            m.SetFloat("_Twist", attributes.twist);
            m.SetInt("_Threads", attributes.threadsNumber);
            cpr.r1 *= attributes.threadSize * attributes.threadsNumber;
            cpr.r2 *= attributes.threadSize * attributes.threadsNumber;
        }
    }
    public void UpdateMesh()
    {
        Curve curve=GetComponent<Curve>();
        curve.UpdateCurve();
        foreach (var cpr in curve.GetComponentsInChildren<CurvePipeRenderer>())
        {
            cpr.UpdateMesh();
        }
    }
}
