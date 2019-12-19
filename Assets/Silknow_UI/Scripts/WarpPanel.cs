using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class WarpPanel : MonoBehaviour
{

    public ScriptableYarn defaultYarn;
   
    public Color outputColor
    {
        get => outputColorImage.color;
        set => outputColorImage.color = value;
    }

   



 
    [SerializeField]
    private Image outputColorImage;
    
  
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public ScriptableYarn GetScriptableYarn()
    {
        ScriptableYarn instance = defaultYarn.Clone() as ScriptableYarn;
        instance.color = outputColor;
        return instance;
    }
}
