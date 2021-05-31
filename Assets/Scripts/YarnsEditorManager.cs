using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Michsky.UI.ModernUIPack;
using Newtonsoft.Json;
using UnityEngine;

public class YarnsEditorManager : MonoBehaviour
{

    public CustomDropdown dropdownYarns;
    public CustomDropdown dropdownYarnMaterials;
    public ScriptableYarn[] originalYarns;
    public ScriptableYarnMaterial[] originalYarnMaterials;
    public static YarnsEditorManager instance;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
            instance = this;
        else 
            Destroy(this);
        DontDestroyOnLoad(this);
    }

    public void Start()
    {
        LoadAllYarns();
        LoadAllYarnMaterials();
    }



    public void SaveYarns()
    {
        foreach (ScriptableYarn sy in originalYarns)
        {
            SaveYarn(sy);
        }
        SaveYarnMaterials();
    }
    public void SaveYarn(ScriptableYarn yarn)
    {
        if (!Directory.Exists(Application.persistentDataPath + "/yarns"))
            Directory.CreateDirectory(Application.persistentDataPath + "/yarns");
        
        JsonSerializerSettings jss = new JsonSerializerSettings();
        jss.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        try
        {
            string json = JsonConvert.SerializeObject(yarn, jss);
            File.WriteAllText(Application.persistentDataPath + "/yarns/" + yarn.name + ".yarn", json);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }

    }
    
    public void SaveYarnMaterials()
    {
        foreach (ScriptableYarnMaterial sym in originalYarnMaterials)
        {
            SaveYarnMaterial(sym);
        }
    }
    public void SaveYarnMaterial(ScriptableYarnMaterial yarnMaterial)
    {
        if (!Directory.Exists(Application.persistentDataPath + "/yarnMaterials"))
            Directory.CreateDirectory(Application.persistentDataPath + "/yarnMaterials");
        
        JsonSerializerSettings jss = new JsonSerializerSettings();
        jss.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        try
        {
            string json = JsonConvert.SerializeObject(yarnMaterial, jss);
            File.WriteAllText(Application.persistentDataPath + "/yarnMaterials/" + yarnMaterial.name + ".yarnMaterial", json);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }

    }

    public void LoadAllYarns()
    {
        string [] fileEntries = Directory.GetFiles(Application.persistentDataPath + "/yarns","*.yarn");
        foreach (string fileName in fileEntries)
        {
            ScriptableYarn sy = LoadYarn(fileName);
            dropdownYarns.CreateNewItem(sy.name, null);
        }
    }
    public void LoadAllYarnMaterials()
    {
        string [] fileEntries = Directory.GetFiles(Application.persistentDataPath + "/yarnMaterials","*.yarnMaterial");
        foreach (string fileName in fileEntries)
        {
            ScriptableYarnMaterial sym = LoadYarnMaterial(fileName);
            dropdownYarnMaterials.CreateNewItem(sym.name, null);
        }
    }
    public ScriptableYarn LoadYarn(string fileName)
    {
        ScriptableYarn scriptableYarn = null;
        if (File.Exists(fileName))
        {
            string json = File.ReadAllText(fileName);
            scriptableYarn = JsonConvert.DeserializeObject<ScriptableYarn>(json);
        }

        return scriptableYarn;
    }
    
    public ScriptableYarnMaterial LoadYarnMaterial(string fileName)
    {
        ScriptableYarnMaterial scriptableYarnMaterial = null;
        if (File.Exists(fileName))
        {
            string json = File.ReadAllText(fileName);
            scriptableYarnMaterial = JsonConvert.DeserializeObject<ScriptableYarnMaterial>(json);
        }

        return scriptableYarnMaterial;
    }
}
