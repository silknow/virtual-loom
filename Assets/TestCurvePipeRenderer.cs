using System.Collections;
using System.Collections.Generic;
using ARTEC.Curves;
using UnityEngine;

public class TestCurvePipeRenderer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Curve>().UpdateCurve();
        GetComponent<CurvePipeRenderer>().UpdateMesh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
