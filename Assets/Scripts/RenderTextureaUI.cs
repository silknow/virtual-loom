using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RenderTextureaUI : MonoBehaviour
{
    public Camera cam;

    private RawImage raw;
    // Start is called before the first frame update
    private void Start()
    {
        raw = GetComponent<RawImage>();

    }

    // Update is called once per frame
    void Update()
    {
        raw.texture = cam.targetTexture;
        raw.SetNativeSize();
    }
}
