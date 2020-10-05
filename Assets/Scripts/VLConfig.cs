
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

[System.Serializable] 
public class VLConfig
{

    public string language;
    public string imgUri;
    public Vector2 dimension;
    [JsonProperty("technique")]
    [JsonConverter(typeof(SingleOrArrayConverter<string>))]
    public List<string> technique;
    [JsonProperty("weaving")]
    [JsonConverter(typeof(SingleOrArrayConverter<string>))]
    public List<string> weaving;
    public Color backgroundColor;
    public bool backgroundColorReceived=false;
    public List<string> materials;
    public string endpoint;
}


