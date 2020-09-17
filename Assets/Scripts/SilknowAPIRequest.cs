using System;
using System.Collections;
using System.Collections.Generic;
using Honeti;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SilknowAPIRequest: MonoBehaviour 
{
    
    public string server = "http://grlc.eurecom.fr/api-git/silknow/api/thesaurus";
    public string URI = "http://data.silknow.org/vocabulary/168";
    public string endpoint = "http://data.silknow.org/sparql";
    public JSonSilknowAPIResponse jsonResponse;
    public string response;
    public Text debugText;
    public Coroutine coroutine { get; private set; }
    public object result;
    private static SilknowAPIRequest _instance = null;

    /// <summary>
    /// I18N components instance.
    /// </summary>
    public void Start()
    {
        if (!PlayerPrefs.HasKey("endpoint"))
            PlayerPrefs.SetString("endpoint",endpoint);
    }
    public void UpdateTranslation(string URI,string attribute, Hashtable table,string key)
    {
        string endpoint = PlayerPrefs.GetString("endpoint");
        string uri = server + "?id=" + UnityWebRequest.EscapeURL(URI) + "&endpoint"+UnityWebRequest.EscapeURL(endpoint)+"&lang=" + I18N.instance.gameLang.ToString().ToLower();

        StartCoroutine(getTranslation(uri));
    }

    IEnumerator getTranslation(string uri)
    {
        
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                response = "Error: " + webRequest.error;
            }
            else
            {
                response = webRequest.downloadHandler.text;
                response = response.Substring(1, response.Length - 3);
                jsonResponse=JsonUtility.FromJson<JSonSilknowAPIResponse>(response);
                response = "Received: " + response;
                if (debugText != null)
                    debugText.text = jsonResponse.description;
            }
        }
        
    }

    
}
