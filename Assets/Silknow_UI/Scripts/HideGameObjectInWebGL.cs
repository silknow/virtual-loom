using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideGameObjectInWebGL : MonoBehaviour
{
    #if UNITY_WEBGL && !UNITY_EDITOR
    private void Awake()
    {
        gameObject.SetActive(false);  
    }
    #endif
}
