using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Color = UnityEngine.Color;

[CreateAssetMenu(fileName = "Yarn", menuName = "Virtual Loom/Yarn", order = 1)]
public class ScriptableYarn : ScriptableObject, ICloneable
{
    public Color color = Color.white;
    public Color fixedColor;
    public float twist = 0 ;
    public float threadSize = 1;
    public int threadsNumber = 1;
    public Material material;
    public bool isMetallic = false;
    public object Clone()
    {
        var result=ScriptableObject.CreateInstance<ScriptableYarn>();
        result.color = color;
        result.twist = twist;
        result.threadSize = threadSize;
        result.threadsNumber = threadsNumber;
        result.material = material;
        result.fixedColor = fixedColor;
        result.isMetallic = isMetallic;
        return result;
    }
}
