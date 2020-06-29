using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ARTEC.Curves
{
    public class CurveControlPoint// : MonoBehaviour
    {

        public Vector3 pos;
        public Quaternion rot;
        public bool changeUpDown = false;
        public float distance = 0;

        public Vector3 right => rot*Vector3.right;
        public Vector3 forward=> rot*Vector3.forward;
    }
}