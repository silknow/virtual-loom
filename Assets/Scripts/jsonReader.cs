using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Honeti;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Networking;

public class jsonReader : MonoBehaviour
{
    private static jsonReader _instance = null;
    public static jsonReader instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<jsonReader>();
            }
            return _instance;
        }
    }
    public string jsonString;
    public VLConfig config;
    public Text FileName;
    public Text msg;
    public Text dimension;
    public Text technique;
    public Text weaving;
    public Text materials;
    public Image color;
    public RawImage texture;
    public Text debug;
    public Dropdown language;
    // Start is called before the first frame update
    void Start()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        WebGLInput.captureAllKeyboardInput = false;
#endif
#if UNITY_EDITOR
        if (jsonString == "")
            jsonString = JsonUtility.ToJson(config);
        Init(jsonString);
#endif 
// #if UNITY_STANDALONE && !UNITY_EDITOR
//     if (jsonString=="")
//     {
//         GetConfigFileFromParam();
//         Init(jsonString);
//     }
// #endif
        if (debug != null)
            debug.text = jsonString;    
        ImaginationOverflow.UniversalDeepLinking.DeepLinkManager.Instance.LinkActivated += Instance_LinkActivated;
    }

    private void Instance_LinkActivated(ImaginationOverflow.UniversalDeepLinking.LinkActivation s)
    {
        jsonString = UnityWebRequest.UnEscapeURL(s.QueryString["data"]);

        Init();

        
    }
    private  void GetConfigFileFromParam()
    {
        var args = System.Environment.GetCommandLineArgs();
        if (args.Length > 1)
        {
            if (args[1].Contains("vloom:\\"))
            {
                //DeepLink
                jsonString = UnityWebRequest.UnEscapeURL(args[1].Substring(44));
            }
            else
            {
                using (FileStream fs = File.Open(args[1], FileMode.Open))
                {
                    byte[] b = new byte[1024];
                    UTF8Encoding temp = new UTF8Encoding(true);

                    while (fs.Read(b,0,b.Length) > 0)
                    {
                        jsonString+=temp.GetString(b);
                    }
                }
            }
        }

    }
    public static IEnumerator GetText(string uri, System.Action<Texture2D> callback)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(uri))
        {
            yield return uwr.SendWebRequest();
            Texture2D res = null;
            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.Log(uwr.error);
            }
            else 
            res = ((DownloadHandlerTexture) uwr.downloadHandler).texture;
            if (callback != null)
                callback(res);
        }
    }

    void TextureLoaded(Texture2D text)
    {
        Debug.Log("TextureLoaded "+text);
        if (texture && text)
        {
            texture.texture = text;
            texture.GetComponent<AspectRatioFitter>().aspectRatio = 1.0f*texture.texture.width / texture.texture.height;
        }
    }

    void Init()
    {
        Debug.Log("Init()");
        Init(jsonString);
    }
    // Update is called once per frame
    void Init(string m)
    {
        Debug.Log("Init("+m+")");
        if (msg)
            msg.text = m;
        try
        {
            config = JsonUtility.FromJson<VLConfig>(m);
            Debug.Log("Config created: Image "+config.imgUri);
            if (m.Contains("backgroundColor:"))
                config.backgroundColorReceived = true;
            if (config != null && config.endpoint!="")
                PlayerPrefs.SetString("endpoint",config.endpoint);
            if (dimension)
            {
                dimension.color = Color.white;
                dimension.text = "Dimension " + config.dimension.ToString();
            }

            if (technique)
            {
                technique.color = Color.white;
                technique.text = "Technique " + config.technique.ToString();
            }

            if (weaving)
            {
                weaving.color = Color.white;
                weaving.text = "Weaving technique " + config.weaving.ToString();
            }

            if (materials)
            {
                materials.color = Color.white;
                materials.text = "Materials ";
                foreach (var matName in config.materials)
                    materials.text += matName.ToString() + ",";
                materials.text = materials.text.Substring(0, materials.text.Length - 1);
            }

            if (config != null) 
                I18N.instance.setLanguage(config.language);
            if (language)
                language.value = (int) I18N.instance.gameLang;
            if (color && config.backgroundColorReceived)
                color.color = config.backgroundColor;
            if (config != null && config.imgUri != "")
            {
                if (WizardController.instance)
                    WizardController.instance.LoadFromJson(config);
                if (texture)
                    StartCoroutine(GetText(config.imgUri, TextureLoaded));
            }
        }
        catch (System.ArgumentException ae)
        {
            Debug.LogError(ae.Message);
            if (debug)
                debug.text = "Parsing error: " + ae.Message;
        }
    }
}
