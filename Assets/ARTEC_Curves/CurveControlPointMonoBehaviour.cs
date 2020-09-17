using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using ARTEC.Curves;
public class CurveControlPointMonoBehaviour : MonoBehaviour
{
    public CurveControlPoint ccp;
    // Start is called before the first frame update
    void Start()
    {
        ccp = new CurveControlPoint();
        ccp.pos = transform.localPosition;
        ccp.rot = transform.localRotation;
    }

    void Update()
    {
        ccp.pos = transform.localPosition;
        ccp.rot = transform.localRotation;
    }
}
