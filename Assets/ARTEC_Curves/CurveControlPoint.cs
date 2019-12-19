using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ARTEC.Curves
{
    [ExecuteInEditMode]
    public class CurveControlPoint : MonoBehaviour
        {
            public float scale=0.001f;
            public bool changeUpDown = false;
            public float distance = 0;
        void Update()
        {
            /*transform.localScale = Vector3.one * scale *
                                   Vector3.Distance(Camera.current.transform.position, transform.position);*/
        }
        
    }
}