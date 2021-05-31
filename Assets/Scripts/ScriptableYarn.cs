using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using Color = UnityEngine.Color;
using Object = System.Object;

[CreateAssetMenu(fileName = "Yarn", menuName = "Virtual Loom/Yarn", order = 1)]
public class ScriptableYarn : ScriptableObject, ICloneable
{
    [JsonProperty("name")]
    public new string name
    {
        get => ((UnityEngine.Object) this).name;
        set => ((UnityEngine.Object) this).name = value;
    }

    public string translatedName;
    [JsonIgnore]
    public Color color = Color.white;
    [JsonProperty("r")]
    public new float r
    {
        get => color.r;
        set => color.r = value;
    }
    [JsonProperty("g")]
    public new float g
    {
        get => color.g;
        set => color.g = value;
    }
    [JsonProperty("b")]
    public new float b
    {
        get => color.b;
        set => color.b = value;
    }
    [JsonIgnore]
    public Color fixedColor;
    [JsonProperty("fr")]
    public new float fr
    {
        get => fixedColor.r;
        set => fixedColor.r = value;
    }
    [JsonProperty("fg")]
    public new float fg
    {
        get => fixedColor.g;
        set => fixedColor.g = value;
    }
    [JsonProperty("fb")]
    public new float fb
    {
        get => fixedColor.b;
        set => fixedColor.b = value;
    }
    public float twist = 0 ;
    public float threadSize = 1;
    public float threadAspect = 1;
    public float threadCompression = 1;
    public int threadsNumber = 1;
    public float narrowingDistance = 1;
    [JsonIgnore]
    public Material material;

    [JsonProperty("material")]
    public string mat
    {
        get => material.name;
        set
        {
            value = value;
        }
    }
    public bool isMetallic = false;
    public object Clone()
    {
        var result=ScriptableObject.CreateInstance<ScriptableYarn>();
        result.translatedName = translatedName;
        result.color = color;
        result.twist = twist;
        result.threadSize = threadSize;
        result.threadAspect = threadAspect;
        result.threadCompression = threadCompression;
        result.threadsNumber = threadsNumber;
        result.narrowingDistance = narrowingDistance;
        result.material = material;
        result.fixedColor = fixedColor;
        result.isMetallic = isMetallic;
        return result;
    }
}
