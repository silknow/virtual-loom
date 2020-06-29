using System.Collections;
using System.Collections.Generic;
using ARTEC.Curves;

using UnityEngine;
//using UnityEngine.Experimental.PlayerLoop;

public class Yarn : MonoBehaviour
{
    public ScriptableYarn attributes;

    protected static Dictionary<string, Dictionary<Color, Material>> _dictionary=null;

    public Material m;
    public Bounds bounds; 
    private CurvePipeRenderer cpr;
    // Start is called before the first frame update
    public void UpdateMesh()
    {
        Curve curve = GetComponent<Curve>();
        
        curve.UpdateCurve();
        bounds = new Bounds();
        cpr = GetComponentInChildren<CurvePipeRenderer>();
        GameObject go;
        if (cpr == null)
        {
            go = Instantiate(Resources.Load("Thread"), transform) as GameObject;
            cpr = go.GetComponent<CurvePipeRenderer>();
        }
        else
            go = cpr.gameObject;

        cpr.curve = curve;
        /*go.GetComponent<Renderer>().material=attributes.material;
        m = go.GetComponent<Renderer>().materials[0];
        m.color = attributes.color;
        m.SetFloat("_Twist", attributes.twist);
        m.SetInt("_Threads",attributes.threadsNumber);*/
        cpr.r1 = attributes.threadSize/ attributes.threadAspect;
        cpr.r2 = attributes.threadSize ;
        cpr.minR1 = cpr.r1 / attributes.threadCompression;
        cpr.minR2 = cpr.r2 / attributes.threadCompression;
        cpr.narrowingDistance = attributes.narrowingDistance;
        go.GetComponent<Renderer>().sharedMaterial=FindOrCreateMaterial();
        cpr.UpdateMesh();
        bounds = cpr.bounds;
    }

    public static void CleanMaterials()
    {
        if (_dictionary!=null)
            _dictionary.Clear();
        
        //PABLO - DESCOMENTAR
        //ClippingPlane.instance.matList.Clear();
    }
    Material FindOrCreateMaterial()
    {
        Material material;
        //If main dictionary doesn't exist, create a new one
        if (_dictionary==null) 
            _dictionary= new Dictionary<string, Dictionary<Color, Material>>();
        
        
        Dictionary<Color, Material> dictMaterial;
        //If scriptableYarn dictionary doesn't exist, create a new one
        if (_dictionary.TryGetValue(attributes.name, out dictMaterial) == false)
        {
            dictMaterial = new Dictionary<Color, Material>();
            _dictionary[attributes.name] = dictMaterial;
        }

        if (dictMaterial.TryGetValue(attributes.color, out material) == false)
        {
            material=Instantiate(attributes.material);
            material.color = attributes.color;
            material.SetColor("_MetalColor",attributes.fixedColor);
            material.SetFloat("_Twist", attributes.twist);
            material.SetInt("_Threads",attributes.threadsNumber);
            dictMaterial[attributes.color] = material;
        }
        return material;
    }
    public void Start()
    {
        UpdateMesh();
    }
}
