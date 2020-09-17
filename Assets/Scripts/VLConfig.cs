using System;
using System.Collections;
using System.Collections.Generic;
using Honeti;
using UnityEngine;

[System.Serializable] 
public class VLConfig
{
    
    public string language;
    public string imgUri;
    public Vector2 dimension;
    public string technique;
    public string weaving;
    public Color backgroundColor;
    public bool backgroundColorReceived=false;
    public List<string> materials;
    public string endpoint;
}
