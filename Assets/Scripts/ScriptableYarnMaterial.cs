using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using Color = UnityEngine.Color;
using Object = System.Object;

[CreateAssetMenu(fileName = "Yarn", menuName = "Virtual Loom/YarnMaterial", order = 2)]
public class ScriptableYarnMaterial : ScriptableObject, ICloneable
{
    [JsonProperty("name")]
    public new string name
    {
        get => ((UnityEngine.Object) this).name;
        set => ((UnityEngine.Object) this).name = value;
    }

    [JsonProperty("tiling")]
    public float tiling
    {
        get => material.GetTextureScale("_MainTex").x;
        set
        {
            Vector2 v=material.GetTextureScale("_MainTex");
            v.x = value;
            material.SetTextureScale("_MainTex",v);
        }
    }

    [JsonProperty("smoothness")]
    public float smoothness
    {
        get => material.GetFloat("_Smoothness");
        set => material.SetFloat("_Smoothness", value);
    }
    [JsonProperty("metallic")]
    public float metallic
    {
        get => material.GetFloat("_Metallic");
        set => material.SetFloat("_Metallic", value);
    }
    [JsonProperty("smoothnessInMetal")]
    public float smoothnessInMetal
    {
        get => material.GetFloat("_SmoothnessInMetal");
        set => material.SetFloat("_SmoothnessInMetal", value);
    }
    [JsonProperty("metallicInMetal")]
    public float metallicInMetal
    {
        get => material.GetFloat("_MetallicInMetal");
        set => material.SetFloat("_MetallicInMetal", value);
    }
    [JsonProperty("normalScale")]
    public float normalScale
    {
        get => material.GetFloat("_NormalScale");
        set => material.SetFloat("_NormalScale", value);
    }
    [JsonProperty("normalTiling")]
    public float normalTiling
    {
        get => material.GetFloat("_NormalScale");
        set => material.SetFloat("_NormalScale", value);
    }
    [JsonProperty("noiseMapScale")]
    public float noiseMapScale
    {
        get => material.GetTextureScale("_NoiseMap").x;
        set
        {
            Vector2 v=material.GetTextureScale("_NoiseMap");
            v.x = value;
            material.SetTextureScale("_NoiseMap",v);
        }
    }
    [JsonProperty("normalMapScale")]
    public float normalMapScale
    {
        get => material.GetTextureScale("_NormalMap").x;
        set
        {
            Vector2 v=material.GetTextureScale("_NormalMap");
            v.x = value;
            material.SetTextureScale("_NormalMap",v);
        }
    }
    [JsonProperty("metalTwist")]
    public float metalTwist
    {
        get => material.GetFloat("_MetalTwist");
        set => material.SetFloat("_MetalTwist", value);
    }
    [JsonProperty("metalRepeat")]
    public float metalRepeat
    {
        get => material.GetFloat("_MetalRepeat");
        set => material.SetFloat("_MetalRepeat", value);
    }
    [JsonProperty("metallicBand")]
    public float metallicBand
    {
        get => material.GetFloat("_MetallicBand");
        set => material.SetFloat("_MetallicBand", value);
    }

    [JsonIgnore]
    public Material _material;
    [JsonIgnore]
    public Material material
    {
        get => _material ? _material : new Material(Shader.Find("Custom/Hilo"));
        set => _material = value;
    }
    public object Clone()
    {
        var result=ScriptableObject.CreateInstance<ScriptableYarnMaterial>();
        result.tiling = tiling;
        result.smoothness = smoothness;
        result.metallic = metallic;
        result.smoothnessInMetal = smoothnessInMetal;
        result.metallicInMetal = metallicInMetal;
        result.normalScale = normalScale;
        result.normalTiling = normalTiling;
        result.noiseMapScale = noiseMapScale;
        result.normalMapScale = normalMapScale;
        result.metalTwist = metalTwist;
        result.metalRepeat = metalRepeat;
        result.metallicBand = metallicBand;
        return result;
    }
}
